using Moq;
using NUnit.Framework;
using PatientService.Application.UseCases.FlaggingSet.Command.CreateFlagging;
using PatientService.Domain.Entities.TestOrder;
using PatientService.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PatientService.Tests.UseCases.FlaggingSet
{
    [TestFixture]
    public class CreateFlaggingSetHandlerTests
    {
        private Mock<IFlaggingSetRepository> _flaggingSetRepositoryMock = null!;
        private CreateFlaggingSetHandler _handler = null!;

        [SetUp]
        public void Setup()
        {
            _flaggingSetRepositoryMock = new Mock<IFlaggingSetRepository>();
            _handler = new CreateFlaggingSetHandler(_flaggingSetRepositoryMock.Object);
        }

        [Test]
        public async Task Handle_ValidRequest_ShouldAddConfigAndReturnDto()
        {
            // Arrange
            var command = new Application.UseCases.FlaggingSet.Command.CreateFlaggingSetCommand
            {
                TestName = "WBC",
                LowThreshold = 4.0f,
                HighThreshold = 10.0f,
                CriticalThreshold = 15.0f,
                Version = "1.1"
            };

            FlaggingSetConfig? capturedConfig = null;

            _flaggingSetRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<FlaggingSetConfig>(), It.IsAny<CancellationToken>()))
                .Callback<FlaggingSetConfig, CancellationToken>((config, token) => capturedConfig = config)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            // 1. Verify the repository was called
            _flaggingSetRepositoryMock.Verify(r => r.AddAsync(It.IsAny<FlaggingSetConfig>(), CancellationToken.None), Times.Once);

            // 2. Verify the captured entity passed to the repository is correct
            Assert.That(capturedConfig, Is.Not.Null);
            Assert.That(capturedConfig.TestName, Is.EqualTo(command.TestName));
            Assert.That(capturedConfig.LowThreshold, Is.EqualTo(command.LowThreshold));
            Assert.That(capturedConfig.HighThreshold, Is.EqualTo(command.HighThreshold));
            Assert.That(capturedConfig.CriticalThreshold, Is.EqualTo(command.CriticalThreshold));
            Assert.That(capturedConfig.Version, Is.EqualTo(command.Version));
            Assert.That(capturedConfig.UpdatedAt, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(1)));

            // 3. Verify the returned DTO is correct
            Assert.That(result, Is.Not.Null);
            Assert.That(result.TestName, Is.EqualTo(command.TestName));
            Assert.That(result.Version, Is.EqualTo(command.Version));
            Assert.That(result.LowThreshold, Is.EqualTo(command.LowThreshold));
            Assert.That(result.HighThreshold, Is.EqualTo(command.HighThreshold));
        }
    }
}