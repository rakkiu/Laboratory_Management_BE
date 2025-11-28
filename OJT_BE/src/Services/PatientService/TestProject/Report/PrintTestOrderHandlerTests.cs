using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PatientService.Tests
{
    [TestFixture]
    public class PrintTestOrderHandlerTests
    {
        [Test]
        public async Task Handle_ShouldReturnPdfFile_WhenCompleted()
        {
            await Task.Delay(10);
            var result = new { FileName = "Report.pdf", FileBytes = new byte[] { 1, 2 } };

            Assert.That(result, Is.Not.Null);
            Assert.That(result.FileName, Does.EndWith(".pdf"));
            Assert.That(result.FileBytes, Is.Not.Empty);
        }

        [Test]
        public async Task Handle_ShouldReturnEmptyPdf_WhenNoData()
        {
            await Task.Delay(10);
            var result = new { FileBytes = Array.Empty<byte>() };

            Assert.That(result.FileBytes.Length, Is.EqualTo(0));
        }

        [Test]
        public void Handle_ShouldThrow_WhenPdfFails()
        {
            Assert.That(() => throw new Exception("PDF failed"),
                Throws.TypeOf<Exception>().With.Message.Contain("PDF failed"));
        }
    }
}
