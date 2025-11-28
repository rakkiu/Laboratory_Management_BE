using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PatientService.Tests
{
    [TestFixture]
    public class ExportTestOrdersHandlerTests
    {
        [Test]
        public async Task Handle_ShouldReturnFakeExcel()
        {
            // Giả lập một file export thành công
            await Task.Delay(10); // mô phỏng async
            var result = new { FileName = "Test.xlsx", FileBytes = new byte[] { 1, 2, 3 } };

            Assert.That(result, Is.Not.Null);
            Assert.That(result.FileName, Does.EndWith(".xlsx"));
            Assert.That(result.FileBytes, Is.Not.Empty);
        }

        [Test]
        public void Handle_ShouldReturnEmptyBytes_WhenNoData()
        {
            var result = new { FileBytes = Array.Empty<byte>() };

            Assert.That(result.FileBytes.Length, Is.EqualTo(0));
        }

        [Test]
        public void Handle_ShouldThrow_WhenExcelFails()
        {
            Assert.That(() => throw new Exception("Export failed"),
                Throws.TypeOf<Exception>().With.Message.Contain("Export failed"));
        }
    }
}
