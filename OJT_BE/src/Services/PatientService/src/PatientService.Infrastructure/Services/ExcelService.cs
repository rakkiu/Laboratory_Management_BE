using OfficeOpenXml;
using OfficeOpenXml.Style;
using PatientService.Application.Interfaces;
using PatientService.Application.Models.TestOrderReportDto;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace PatientService.Infrastructure.Services
{
    public class ExcelService : IExcelService
    {
        private readonly IIamUserClient _iamUserClient;

        public ExcelService(IIamUserClient iamUserClient)
        {
            _iamUserClient = iamUserClient;
        }

        public async Task<byte[]> ExportTestOrdersAsync(IEnumerable<TestOrderExportDto> testOrders, string fileName)
        {
            ExcelPackage.License.SetNonCommercialPersonal("Local Development");

            // ================================================================
            // 1️⃣ Trích danh sách GUID duy nhất từ CreatedBy
            // ================================================================
            var createdByGuids = testOrders
                .Where(o => !string.IsNullOrWhiteSpace(o.CreatedBy) &&
                            Guid.TryParse(o.CreatedBy, out _))
                .Select(o => Guid.Parse(o.CreatedBy))
                .Distinct()
                .ToList();

            // ================================================================
            // 2️⃣ Resolve tên người dùng qua gRPC
            // ================================================================
            var createdByMap = new Dictionary<Guid, string?>();

            foreach (var guid in createdByGuids)
            {
                var fullName = await _iamUserClient.GetUserFullNameAsync(guid);
                createdByMap[guid] = fullName;
            }

            string ResolveCreatedBy(string? raw)
            {
                if (string.IsNullOrWhiteSpace(raw)) return "Unknown";

                if (Guid.TryParse(raw, out var g))
                {
                    return createdByMap.TryGetValue(g, out var name) &&
                           !string.IsNullOrWhiteSpace(name)
                        ? name
                        : raw; // fallback nếu IAM không trả ra tên
                }

                return raw;
            }

            // ================================================================
            // 3️⃣ Tạo Excel
            // ================================================================
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Danh sách phiếu xét nghiệm");

            string[] headers = {
                "Mã phiếu xét nghiệm",
                "Họ tên bệnh nhân",
                "Giới tính",
                "Ngày sinh",
                "Số điện thoại",
                "Trạng thái",
                "Người tạo",
                "Ngày tạo",
                "Thiết bị",
                "Ngày thực hiện"
            };

            // ====== Header style ======
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cells[1, i + 1];
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Font.Size = 12;
                cell.Style.Font.Color.SetColor(Color.White);
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(52, 152, 219));
                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                cell.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Gray);
            }

            // ====== Body ======
            int row = 2;

            foreach (var o in testOrders)
            {
                string genderDisplay = o.Gender?.ToLower() switch
                {
                    "male" => "Nam",
                    "female" => "Nữ",
                    _ => "Khác"
                };

                worksheet.Cells[row, 1].Value = o.Id.ToString();
                worksheet.Cells[row, 2].Value = o.PatientName;
                worksheet.Cells[row, 3].Value = genderDisplay;
                
                if (o.DateOfBirth.HasValue)
                {
                    worksheet.Cells[row, 4].Value = o.DateOfBirth.Value;
                    worksheet.Cells[row, 4].Style.Numberformat.Format = "dd-mm-yyyy";
                }
                else
                {
                    worksheet.Cells[row, 4].Value = "";
                }

                worksheet.Cells[row, 5].Value = o.PhoneNumber;
                worksheet.Cells[row, 6].Value = o.Status;

                // ⭐⭐⭐ Resolve CreatedBy bằng gRPC ⭐⭐⭐
                worksheet.Cells[row, 7].Value = ResolveCreatedBy(o.CreatedBy);

                worksheet.Cells[row, 8].Value = o.CreatedOn.ToString("dd/MM/yyyy HH:mm");
                worksheet.Cells[row, 9].Value = o.RunBy;
                worksheet.Cells[row, 10].Value = o.RunOn?.ToString("dd/MM/yyyy HH:mm");

                // Style row
                for (int col = 1; col <= headers.Length; col++)
                {
                    var cell = worksheet.Cells[row, col];
                    cell.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.LightGray);
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }

                row++;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            for (int col = 1; col <= headers.Length; col++)
            {
                worksheet.Column(col).Width = worksheet.Column(col).Width * 1.15;
            }

            worksheet.View.FreezePanes(2, 1);
            worksheet.Row(1).Height = 25;

            return await Task.FromResult(package.GetAsByteArray());
        }
    }
}
