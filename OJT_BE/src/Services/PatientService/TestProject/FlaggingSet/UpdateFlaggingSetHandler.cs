using Moq;
using NUnit.Framework;
using PatientService.Application.UseCases.FlaggingSet.Command.UpdateFlagging;
using PatientService.Domain.Entities.TestOrder;
using PatientService.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PatientService.Tests.UseCases.FlaggingSet
{
    [TestFixture]
    public class UpdateFlaggingSetHandlerTests
    {
        private Mock<IFlaggingSetRepository> _flaggingSetRepositoryMock = null!;
        private UpdateFlaggingSetHandler _handler = null!;

        [SetUp]
        public void Setup()
        {
            _flaggingSetRepositoryMock = new Mock<IFlaggingSetRepository>();
            _handler = new UpdateFlaggingSetHandler(_flaggingSetRepositoryMock.Object);
        }

        [Test]
        public async Task Handle_ExistingConfig_ShouldUpdateAndReturnDto()
        {
            // Arrange
            var configId = 1;
            var command = new UpdateFlaggingSetCommand
            {
                ConfigId = configId,
                TestName = "HGB",
                LowThreshold = 14.0f,
                HighThreshold = 18.0f,
                CriticalThreshold = 20.0f,
                Version = "1.2"
            };

            var originalUpdatedAt = DateTime.UtcNow.AddDays(-1);
            var existingConfig = new FlaggingSetConfig
            {
                ConfigId = configId,
                TestName = "Hemoglobin",
                Version = "1.1",
                UpdatedAt = originalUpdatedAt
            };

            FlaggingSetConfig? capturedConfig = null; // Biến để bắt đối tượng

            _flaggingSetRepositoryMock
                .Setup(r => r.GetByIdAsync(configId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingConfig);

            // Thiết lập Callback để bắt đối tượng được truyền vào phương thức Update
            _flaggingSetRepositoryMock
                .Setup(r => r.Update(It.IsAny<FlaggingSetConfig>()))
                .Callback<FlaggingSetConfig>(config => capturedConfig = config);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            // 1. Xác minh rằng phương thức Update đã được gọi một lần
            _flaggingSetRepositoryMock.Verify(r => r.Update(It.IsAny<FlaggingSetConfig>()), Times.Once);

            // 2. Xác minh các thuộc tính của đối tượng đã được bắt
            Assert.That(capturedConfig, Is.Not.Null);
            Assert.That(capturedConfig.ConfigId, Is.EqualTo(configId));
            Assert.That(capturedConfig.TestName, Is.EqualTo(command.TestName));
            Assert.That(capturedConfig.LowThreshold, Is.EqualTo(command.LowThreshold));
            Assert.That(capturedConfig.HighThreshold, Is.EqualTo(command.HighThreshold));
            Assert.That(capturedConfig.CriticalThreshold, Is.EqualTo(command.CriticalThreshold));
            Assert.That(capturedConfig.Version, Is.EqualTo(command.Version));
            Assert.That(capturedConfig.UpdatedAt, Is.GreaterThan(originalUpdatedAt));

            // 3. Xác minh DTO trả về là chính xác
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ConfigId, Is.EqualTo(command.ConfigId));
            Assert.That(result.TestName, Is.EqualTo(command.TestName));
            Assert.That(result.Version, Is.EqualTo(command.Version));
        }

        [Test]
        public async Task Handle_NonExistingConfig_ShouldReturnNull()
        {
            // Arrange
            var configId = 99;
            var command = new UpdateFlaggingSetCommand { ConfigId = configId };

            _flaggingSetRepositoryMock
                .Setup(r => r.GetByIdAsync(configId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((FlaggingSetConfig)null!);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            // 1. Verify repository methods were called
            _flaggingSetRepositoryMock.Verify(r => r.GetByIdAsync(configId, CancellationToken.None), Times.Once);
            
            // 2. Ensure Update was NOT called
            _flaggingSetRepositoryMock.Verify(r => r.Update(It.IsAny<FlaggingSetConfig>()), Times.Never);

            // 3. Assert the result is null
            Assert.That(result, Is.Null);
        }
    }
}