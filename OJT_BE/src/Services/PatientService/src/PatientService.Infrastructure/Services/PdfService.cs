using iTextSharp.text;
using iTextSharp.text.pdf;
using PatientService.Application.Interfaces;
using PatientService.Application.Models.TestOrderReportDto;

namespace PatientService.Infrastructure.Services
{
    public class PdfService : IPdfService
    {
        private readonly IIamUserClient _iamUserClient;

        public PdfService(IIamUserClient iamUserClient)
        {
            _iamUserClient = iamUserClient;
        }

        public async Task<byte[]> GenerateTestOrderReportAsync(TestOrderPdfDto testOrder, string fileName)
        {
            using var stream = new MemoryStream();
            var document = new Document(PageSize.A4, 50, 50, 25, 25);
            var writer = PdfWriter.GetInstance(document, stream);
            document.Open();

            // Font Unicode
            string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arialuni.ttf");
            if (!File.Exists(fontPath))
                fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "times.ttf");

            var baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            var normalFont = new Font(baseFont, 11, Font.NORMAL);
            var boldFont = new Font(baseFont, 11, Font.BOLD);
            var titleFont = new Font(baseFont, 16, Font.BOLD);

            // ================= HEADER =================
            var headerTable = new PdfPTable(2) { WidthPercentage = 100 };
            headerTable.SetWidths(new float[] { 1f, 3f });

            string baseDir = AppContext.BaseDirectory;
            string logoPath = Path.Combine(baseDir, "wwwroot", "logo.png");
            if (!File.Exists(logoPath))
            {
                var infraDir = Directory.GetParent(baseDir);
                if (infraDir?.Parent?.Parent != null)
                {
                    logoPath = Path.Combine(infraDir.Parent.Parent.FullName,
                        "PatientService.Infrastructure", "wwwroot", "logo.png");
                }
            }

            if (File.Exists(logoPath))
            {
                var logo = Image.GetInstance(logoPath);
                logo.ScaleAbsolute(90f, 90f);
                headerTable.AddCell(new PdfPCell(logo)
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    PaddingBottom = 8f
                });
            }
            else
            {
                headerTable.AddCell(new PdfPCell(new Phrase("LOGO", boldFont))
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
            }

            // Flag colors
            BaseColor GetFlagColor(string? flag)
            {
                return flag switch
                {
                    "High" => new BaseColor(255, 204, 204),
                    "Low" => new BaseColor(255, 242, 204),
                    _ => BaseColor.White
                };
            }

            Font GetFontByFlag(string? flag)
            {
                return flag == "High" ? boldFont : normalFont;
            }

            // Thông tin trung tâm
            var infoFont = new Font(baseFont, 13, Font.NORMAL);
            var infoName = new Font(baseFont, 16, Font.BOLD);

            var centerInfo = new Paragraph
            {
                Alignment = Element.ALIGN_LEFT,
                Leading = 24f,
                IndentationLeft = 20f
            };
            centerInfo.Add(new Phrase("Trung tâm xét nghiệm máu OJT_TEAM3_NET08\n\n", infoName));
            centerInfo.Add(new Phrase("Số điện thoại: 0123456789\n", infoFont));
            centerInfo.Add(new Phrase("Địa chỉ: Đường 123, Q1, TP.HCM", infoFont));

            headerTable.AddCell(new PdfPCell(centerInfo)
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE
            });

            document.Add(headerTable);
            document.Add(new Paragraph("\n"));

            // ================= TIÊU ĐỀ =================
            var title = new Paragraph("CHI TIẾT PHIẾU XÉT NGHIỆM", titleFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 12f
            };
            document.Add(title);

            // ================= THÔNG TIN BỆNH NHÂN =================
            var infoTable = new PdfPTable(2) { WidthPercentage = 100 };
            infoTable.SpacingBefore = 8f;
            infoTable.SpacingAfter = 12f;
            infoTable.SetWidths(new float[] { 2.2f, 5.8f });

            var orangeLight = new BaseColor(200, 247, 197);
            var blueLight = new BaseColor(204, 229, 255);

            var labelCellStyle = new Func<string, PdfPCell>(text => new PdfPCell(new Phrase(text, boldFont))
            {
                BackgroundColor = orangeLight,
                Padding = 6f
            });

            var valueCellStyle = new Func<string, PdfPCell>(text => new PdfPCell(new Phrase(text ?? "", normalFont))
            {
                BackgroundColor = blueLight,
                Padding = 6f
            });

            string genderDisplay = testOrder.Gender?.ToLower() switch
            {
                "male" => "Nam",
                "female" => "Nữ",
                _ => "Khác"
            };

            void AddInfoRow(string label, string? value)
            {
                infoTable.AddCell(labelCellStyle(label));
                infoTable.AddCell(valueCellStyle(value ?? ""));
            }

            AddInfoRow("Mã phiếu", testOrder.Id.ToString());
            AddInfoRow("Họ tên bệnh nhân", testOrder.PatientName);
            AddInfoRow("Giới tính", genderDisplay);
            AddInfoRow("Ngày sinh", testOrder.DateOfBirth?.ToString("dd/MM/yyyy"));
            AddInfoRow("Số điện thoại", testOrder.PhoneNumber);
            AddInfoRow("Người tạo", testOrder.CreatedBy);
            AddInfoRow("Ngày tạo", testOrder.CreatedOn.ToString("dd/MM/yyyy HH:mm"));
            AddInfoRow("Thông tin máy xét nghiệm", $"{testOrder.RunBy ?? ""} - {testOrder.InstrumentUsed ?? ""}");
            AddInfoRow("Ngày thực hiện:", testOrder.RunOn?.ToString("dd/MM/yyyy HH:mm") ?? "");

            document.Add(infoTable);

            // ================= KẾT QUẢ XÉT NGHIỆM =================
            document.Add(new Paragraph("KẾT QUẢ XÉT NGHIỆM\n\n", boldFont));

            var resultTable = new PdfPTable(4) { WidthPercentage = 100 };
            resultTable.SetWidths(new float[] { 3f, 2f, 3f, 3f });

            string[] headers = { "Tên xét nghiệm", "Giá trị", "Khoảng tham chiếu", "Kết luận" };
            foreach (var h in headers)
            {
                resultTable.AddCell(new PdfPCell(new Phrase(h, boldFont))
                {
                    BackgroundColor = orangeLight,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 6f
                });
            }

            foreach (var r in testOrder.Results ?? Enumerable.Empty<TestResultDto>())
            {
                if (r.Details == null || r.Details.Count == 0)
                {
                    resultTable.AddCell(new PdfPCell(new Phrase(r.TestName, normalFont)) { Padding = 6f });
                    resultTable.AddCell(new PdfPCell(new Phrase("", normalFont)) { Padding = 6f });
                    resultTable.AddCell(new PdfPCell(new Phrase("", normalFont)) { Padding = 6f });
                    resultTable.AddCell(new PdfPCell(new Phrase("", normalFont)) { Padding = 6f });
                    continue;
                }

                foreach (var d in r.Details)
                {
                    BaseColor rowColor = GetFlagColor(d.Flag);
                    Font rowFont = GetFontByFlag(d.Flag);

                    string fullName = d.Type != null ? $"{r.TestName} ({d.Type})" : r.TestName;

                    resultTable.AddCell(new PdfPCell(new Phrase(fullName, rowFont))
                    {
                        Padding = 6f,
                        BackgroundColor = rowColor
                    });

                    resultTable.AddCell(new PdfPCell(new Phrase(d.Value.ToString(), rowFont))
                    {
                        Padding = 6f,
                        BackgroundColor = rowColor,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    });

                    resultTable.AddCell(new PdfPCell(new Phrase(d.ReferenceRange ?? "", rowFont))
                    {
                        Padding = 6f,
                        BackgroundColor = rowColor,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    });

                    string flagText = d.Flag switch
                    {
                        "High" => "Cao",
                        "Low" => "Thấp",
                        "Normal" => "Bình thường",
                        _ => ""
                    };

                    resultTable.AddCell(new PdfPCell(new Phrase(flagText, rowFont))
                    {
                        Padding = 6f,
                        BackgroundColor = rowColor,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    });
                }
            }

            document.Add(resultTable);

            // ================= NHẬN XÉT =================
            if (testOrder.Comments?.Any() == true)
            {
                document.Add(new Paragraph("\nNHẬN XÉT", boldFont)
                {
                    SpacingBefore = 10f
                });

                foreach (var c in testOrder.Comments)
                {
                    document.Add(new Paragraph("- " + c, normalFont)
                    {
                        IndentationLeft = 15f,
                        SpacingAfter = 2f
                    });
                }
            }

            // ================= NGÀY + CHỮ KÝ (GIỮ CÙNG TRANG) =================

            PdfPTable footerWrapper = new PdfPTable(1)
            {
                WidthPercentage = 100,
                KeepTogether = true
            };

            PdfPCell wrapperCell = new PdfPCell
            {
                Border = Rectangle.NO_BORDER
            };

            // ----- Ngày tháng -----
            var today = DateTime.Now;
            var dateText = $"Ngày {today:dd} tháng {today:MM} năm {today:yyyy}";
            var dateParagraph = new Paragraph(dateText, new Font(baseFont, 11, Font.ITALIC))
            {
                Alignment = Element.ALIGN_RIGHT,
                IndentationRight = 55f,
                SpacingAfter = 14f
            };

            wrapperCell.AddElement(dateParagraph);

            // ----- Bảng chữ ký -----
            var footerTable = new PdfPTable(2) { WidthPercentage = 100 };
            footerTable.SetWidths(new float[] { 1f, 1f });
            footerTable.AddCell(new PdfPCell(new Phrase(" ", boldFont))
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            // Lấy tên người duyệt
            if (Guid.TryParse(testOrder.ReviewedBy, out var reviewerId))
            {
                var reviewerFullName = await _iamUserClient.GetUserFullNameAsync(reviewerId);
                if (!string.IsNullOrWhiteSpace(reviewerFullName))
                    testOrder.ReviewedBy = reviewerFullName;
            }

            var reviewerName = testOrder.ReviewedBy ?? "................";

            // Cột phải ký tên
            var rightCellParagraph = new Paragraph
            {
                Alignment = Element.ALIGN_CENTER,
                Leading = 22f
            };

            rightCellParagraph.Add(new Phrase("TRƯỞNG PHÒNG XÉT NGHIỆM\n\n\n\n\n\n\n\n", boldFont));
            rightCellParagraph.Add(new Phrase(reviewerName, boldFont));

            footerTable.AddCell(new PdfPCell(rightCellParagraph)
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            wrapperCell.AddElement(footerTable);

            footerWrapper.AddCell(wrapperCell);

            document.Add(footerWrapper);


            // ================= END =================
            document.Close();
            writer.Close();
            return await Task.FromResult(stream.ToArray());
        }
    }
}
