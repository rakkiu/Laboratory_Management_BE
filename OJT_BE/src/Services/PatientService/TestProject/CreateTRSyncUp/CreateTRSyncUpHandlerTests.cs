//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using FluentAssertions;
//using Moq;
//using NUnit.Framework;
//using PatientService.Application.UseCases.TestOrderUC.TRSyncUp.Command.CreateTRSyncUp;
//using PatientService.Domain.Entities.TestOrder;
//using PatientService.Domain.Interfaces.TestOrderService;

//namespace TestProject.CreateTRSyncUp;

//[TestFixture]
//public class CreateTRSyncUpHandlerTests
//{
//    private Mock<ITestResultRepository> _testResultRepositoryMock = null!;
//    private CreateTRSyncUptHandler _handler = null!;

//    [SetUp]
//    public void Setup()
//    {
//        _testResultRepositoryMock = new Mock<ITestResultRepository>();

//        _handler = new CreateTRSyncUptHandler(_testResultRepositoryMock.Object);
//    }

//    [Test]
//    public async Task Handle_ShouldCreateTestResult_WhenValidInput()
//    {
//        // Arrange
//        var testOrderId = Guid.NewGuid();
//        var command = new CreateTRSyncUpCommand
//        {
//            TestOrderId = testOrderId,
//            TestName = "Blood Glucose",
//            Value = "95.5",
//            ReferenceRange = "70 - 100",
//            Interpretation = "Normal range",
//            InstrumentUsed = "Glucometer X200",
//            Flag = "Normal"
//        };

//        _testResultRepositoryMock
//            .Setup(r => r.TestOrderExistsAsync(testOrderId))
//            .ReturnsAsync(true);

//        _testResultRepositoryMock
//            .Setup(r => r.AddTestResultAsync(It.IsAny<TestResult>()))
//            .Returns(Task.CompletedTask);

//        _testResultRepositoryMock
//            .Setup(r => r.SaveChangesAsync())
//            .Returns(Task.CompletedTask);

//        // Act
//        var resultId = await _handler.Handle(command, CancellationToken.None);

//        // Assert
//        resultId.Should().NotBe(Guid.Empty);

//        _testResultRepositoryMock.Verify(
//            r => r.TestOrderExistsAsync(testOrderId),
//            Times.Once);

//        _testResultRepositoryMock.Verify(
//            r => r.AddTestResultAsync(It.Is<TestResult>(tr =>
//                tr.ResultId != Guid.Empty &&
//                tr.TestOrderId == testOrderId &&
//                tr.TestName == "Blood Glucose" &&
//                tr.Value == "95.5" &&
//                tr.ReferenceRange == "70 - 100" &&
//                tr.Interpretation == "Normal range" &&
//                tr.InstrumentUsed == "Glucometer X200" &&
//                tr.Flag == "Normal"
//            )),
//            Times.Once);

//        _testResultRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
//    }

//    [Test]
//    public void Handle_ShouldThrowKeyNotFoundException_WhenTestOrderNotFound()
//    {
//        // Arrange
//        var testOrderId = Guid.NewGuid();
//        var command = new CreateTRSyncUpCommand
//        {
//            TestOrderId = testOrderId,
//            TestName = "Blood Glucose",
//            Value = "95.5",
//            Flag = "Normal"
//        };

//        _testResultRepositoryMock
//            .Setup(r => r.TestOrderExistsAsync(testOrderId))
//            .ReturnsAsync(false);

//        // Act & Assert
//        var ex = Assert.ThrowsAsync<KeyNotFoundException>(
//            () => _handler.Handle(command, CancellationToken.None));

//        ex.Message.Should().Be($"Test order with ID '{testOrderId}' not found or has been deleted.");

//        _testResultRepositoryMock.Verify(
//            r => r.TestOrderExistsAsync(testOrderId),
//            Times.Once);

//        _testResultRepositoryMock.Verify(
//            r => r.AddTestResultAsync(It.IsAny<TestResult>()),
//            Times.Never);

//        _testResultRepositoryMock.Verify(
//            r => r.SaveChangesAsync(),
//            Times.Never);
//    }

//    [Test]
//    public async Task Handle_ShouldCreateTestResult_WithOnlyRequiredFields()
//    {
//        // Arrange
//        var testOrderId = Guid.NewGuid();
//        var command = new CreateTRSyncUpCommand
//        {
//            TestOrderId = testOrderId,
//            TestName = "Complete Blood Count",
//            Value = "12.5",
//            Flag = "Normal"
//        };

//        TestResult? capturedResult = null;

//        _testResultRepositoryMock
//            .Setup(r => r.TestOrderExistsAsync(testOrderId))
//            .ReturnsAsync(true);

//        _testResultRepositoryMock
//            .Setup(r => r.AddTestResultAsync(It.IsAny<TestResult>()))
//            .Callback<TestResult>(tr => capturedResult = tr)
//            .Returns(Task.CompletedTask);

//        _testResultRepositoryMock
//            .Setup(r => r.SaveChangesAsync())
//            .Returns(Task.CompletedTask);

//        // Act
//        var resultId = await _handler.Handle(command, CancellationToken.None);

//        // Assert
//        capturedResult.Should().NotBeNull();
//        capturedResult!.TestOrderId.Should().Be(testOrderId);
//        capturedResult.TestName.Should().Be("Complete Blood Count");
//        capturedResult.Value.Should().Be("12.5");
//        capturedResult.Flag.Should().Be("Normal");
//        capturedResult.ReferenceRange.Should().BeNull();
//        capturedResult.Interpretation.Should().BeNull();
//        capturedResult.InstrumentUsed.Should().BeNull();
//    }

//    [Test]
//    public async Task Handle_ShouldSetCorrectTestName_FromCommand()
//    {
//        // Arrange
//        var testOrderId = Guid.NewGuid();
//        var command = new CreateTRSyncUpCommand
//        {
//            TestOrderId = testOrderId,
//            TestName = "Hemoglobin A1C",
//            Value = "5.8",
//            Flag = "Normal"
//        };

//        TestResult? capturedResult = null;

//        _testResultRepositoryMock
//            .Setup(r => r.TestOrderExistsAsync(testOrderId))
//            .ReturnsAsync(true);

//        _testResultRepositoryMock
//            .Setup(r => r.AddTestResultAsync(It.IsAny<TestResult>()))
//            .Callback<TestResult>(tr => capturedResult = tr)
//            .Returns(Task.CompletedTask);

//        _testResultRepositoryMock
//            .Setup(r => r.SaveChangesAsync())
//            .Returns(Task.CompletedTask);

//        // Act
//        await _handler.Handle(command, CancellationToken.None);

//        // Assert
//        capturedResult.Should().NotBeNull();
//        capturedResult!.TestName.Should().Be("Hemoglobin A1C");
//    }

//    [Test]
//    public async Task Handle_ShouldSetCorrectValue_FromCommand()
//    {
//        // Arrange
//        var testOrderId = Guid.NewGuid();
//        var command = new CreateTRSyncUpCommand
//        {
//            TestOrderId = testOrderId,
//            TestName = "Cholesterol",
//            Value = "185.5",
//            Flag = "Normal"
//        };

//        TestResult? capturedResult = null;

//        _testResultRepositoryMock
//            .Setup(r => r.TestOrderExistsAsync(testOrderId))
//            .ReturnsAsync(true);

//        _testResultRepositoryMock
//            .Setup(r => r.AddTestResultAsync(It.IsAny<TestResult>()))
//            .Callback<TestResult>(tr => capturedResult = tr)
//            .Returns(Task.CompletedTask);

//        _testResultRepositoryMock
//            .Setup(r => r.SaveChangesAsync())
//            .Returns(Task.CompletedTask);

//        // Act
//        await _handler.Handle(command, CancellationToken.None);

//        // Assert
//        capturedResult.Should().NotBeNull();
//        capturedResult!.Value.Should().Be("185.5");
//    }

//    [Test]
//    public async Task Handle_ShouldSetCorrectReferenceRange_FromCommand()
//    {
//        // Arrange
//        var testOrderId = Guid.NewGuid();
//        var command = new CreateTRSyncUpCommand
//        {
//            TestOrderId = testOrderId,
//            TestName = "Blood Pressure",
//            Value = "120/80",
//            ReferenceRange = "90/60 - 120/80",
//            Flag = "Normal"
//        };

//        TestResult? capturedResult = null;

//        _testResultRepositoryMock
//            .Setup(r => r.TestOrderExistsAsync(testOrderId))
//            .ReturnsAsync(true);

//        _testResultRepositoryMock
//            .Setup(r => r.AddTestResultAsync(It.IsAny<TestResult>()))
//            .Callback<TestResult>(tr => capturedResult = tr)
//            .Returns(Task.CompletedTask);

//        _testResultRepositoryMock
//            .Setup(r => r.SaveChangesAsync())
//            .Returns(Task.CompletedTask);

//        // Act
//        await _handler.Handle(command, CancellationToken.None);

//        // Assert
//        capturedResult.Should().NotBeNull();
//        capturedResult!.ReferenceRange.Should().Be("90/60 - 120/80");
//    }

//    [Test]
//    public async Task Handle_ShouldSetCorrectInterpretation_FromCommand()
//    {
//        // Arrange
//        var testOrderId = Guid.NewGuid();
//        var command = new CreateTRSyncUpCommand
//        {
//            TestOrderId = testOrderId,
//            TestName = "Liver Function Test",
//            Value = "45",
//            Interpretation = "Slightly elevated, monitor closely",
//            Flag = "High"
//        };

//        TestResult? capturedResult = null;

//        _testResultRepositoryMock
//            .Setup(r => r.TestOrderExistsAsync(testOrderId))
//            .ReturnsAsync(true);

//        _testResultRepositoryMock
//            .Setup(r => r.AddTestResultAsync(It.IsAny<TestResult>()))
//            .Callback<TestResult>(tr => capturedResult = tr)
//            .Returns(Task.CompletedTask);

//        _testResultRepositoryMock
//            .Setup(r => r.SaveChangesAsync())
//            .Returns(Task.CompletedTask);

//        // Act
//        await _handler.Handle(command, CancellationToken.None);

//        // Assert
//        capturedResult.Should().NotBeNull();
//        capturedResult!.Interpretation.Should().Be("Slightly elevated, monitor closely");
//    }

//    [Test]
//    public async Task Handle_ShouldSetCorrectInstrumentUsed_FromCommand()
//    {
//        // Arrange
//        var testOrderId = Guid.NewGuid();
//        var command = new CreateTRSyncUpCommand
//        {
//            TestOrderId = testOrderId,
//            TestName = "Blood Count",
//            Value = "5000",
//            InstrumentUsed = "Hematology Analyzer Pro",
//            Flag = "Normal"
//        };

//        TestResult? capturedResult = null;

//        _testResultRepositoryMock
//            .Setup(r => r.TestOrderExistsAsync(testOrderId))
//            .ReturnsAsync(true);

//        _testResultRepositoryMock
//            .Setup(r => r.AddTestResultAsync(It.IsAny<TestResult>()))
//            .Callback<TestResult>(tr => capturedResult = tr)
//            .Returns(Task.CompletedTask);

//        _testResultRepositoryMock
//            .Setup(r => r.SaveChangesAsync())
//            .Returns(Task.CompletedTask);

//        // Act
//        await _handler.Handle(command, CancellationToken.None);

//        // Assert
//        capturedResult.Should().NotBeNull();
//        capturedResult!.InstrumentUsed.Should().Be("Hematology Analyzer Pro");
//    }

//    [Test]
//    public async Task Handle_ShouldSetCorrectFlag_FromCommand()
//    {
//        // Arrange
//        var testOrderId = Guid.NewGuid();
//        var command = new CreateTRSyncUpCommand
//        {
//            TestOrderId = testOrderId,
//            TestName = "Glucose",
//            Value = "65",
//            Flag = "Low"
//        };

//        TestResult? capturedResult = null;

//        _testResultRepositoryMock
//            .Setup(r => r.TestOrderExistsAsync(testOrderId))
//            .ReturnsAsync(true);

//        _testResultRepositoryMock
//            .Setup(r => r.AddTestResultAsync(It.IsAny<TestResult>()))
//            .Callback<TestResult>(tr => capturedResult = tr)
//            .Returns(Task.CompletedTask);

//        _testResultRepositoryMock
//            .Setup(r => r.SaveChangesAsync())
//            .Returns(Task.CompletedTask);

//        // Act
//        await _handler.Handle(command, CancellationToken.None);

//        // Assert
//        capturedResult.Should().NotBeNull();
//        capturedResult!.Flag.Should().Be("Low");
//    }

//    [Test]
//    public async Task Handle_ShouldSetDefaultFlagAsNormal_WhenNotProvided()
//    {
//        // Arrange
//        var testOrderId = Guid.NewGuid();
//        var command = new CreateTRSyncUpCommand
//        {
//            TestOrderId = testOrderId,
//            TestName = "Test",
//            Value = "100"
//        };

//        TestResult? capturedResult = null;

//        _testResultRepositoryMock
//            .Setup(r => r.TestOrderExistsAsync(testOrderId))
//            .ReturnsAsync(true);

//        _testResultRepositoryMock
//            .Setup(r => r.AddTestResultAsync(It.IsAny<TestResult>()))
//            .Callback<TestResult>(tr => capturedResult = tr)
//            .Returns(Task.CompletedTask);

//        _testResultRepositoryMock
//            .Setup(r => r.SaveChangesAsync())
//            .Returns(Task.CompletedTask);

//        // Act
//        await _handler.Handle(command, CancellationToken.None);

//        // Assert
//        capturedResult.Should().NotBeNull();
//        capturedResult!.Flag.Should().Be("Normal");
//    }

//    [Test]
//    public async Task Handle_ShouldSetCreatedAt_ToCurrentUtcTime()
//    {
//        // Arrange
//        var beforeTest = DateTime.UtcNow.AddSeconds(-1);
//        var testOrderId = Guid.NewGuid();
//        var command = new CreateTRSyncUpCommand
//        {
//            TestOrderId = testOrderId,
//            TestName = "Test",
//            Value = "100",
//            Flag = "Normal"
//        };

//        TestResult? capturedResult = null;

//        _testResultRepositoryMock
//            .Setup(r => r.TestOrderExistsAsync(testOrderId))
//            .ReturnsAsync(true);

//        _testResultRepositoryMock
//            .Setup(r => r.AddTestResultAsync(It.IsAny<TestResult>()))
//            .Callback<TestResult>(tr => capturedResult = tr)
//            .Returns(Task.CompletedTask);

//        _testResultRepositoryMock
//            .Setup(r => r.SaveChangesAsync())
//            .Returns(Task.CompletedTask);

//        // Act
//        await _handler.Handle(command, CancellationToken.None);
//        var afterTest = DateTime.UtcNow.AddSeconds(1);

//        // Assert
//        capturedResult.Should().NotBeNull();
//        capturedResult!.CreatedAt.Should().BeAfter(beforeTest);
//        capturedResult.CreatedAt.Should().BeBefore(afterTest);
//    }

//    [Test]
//    public async Task Handle_ShouldGenerateUniqueResultId_ForEachCall()
//    {
//        // Arrange
//        var testOrderId = Guid.NewGuid();
//        var command = new CreateTRSyncUpCommand
//        {
//            TestOrderId = testOrderId,
//            TestName = "Test",
//            Value = "100",
//            Flag = "Normal"
//        };

//        var capturedResultIds = new List<Guid>();

//        _testResultRepositoryMock
//            .Setup(r => r.TestOrderExistsAsync(testOrderId))
//            .ReturnsAsync(true);

//        _testResultRepositoryMock
//            .Setup(r => r.AddTestResultAsync(It.IsAny<TestResult>()))
//            .Callback<TestResult>(tr => capturedResultIds.Add(tr.ResultId))
//            .Returns(Task.CompletedTask);

//        _testResultRepositoryMock
//            .Setup(r => r.SaveChangesAsync())
//            .Returns(Task.CompletedTask);

//        // Act
//        await _handler.Handle(command, CancellationToken.None);
//        await _handler.Handle(command, CancellationToken.None);
//        await _handler.Handle(command, CancellationToken.None);

//        // Assert
//        capturedResultIds.Should().HaveCount(3);
//        capturedResultIds.Should().OnlyHaveUniqueItems();
//        capturedResultIds.Should().NotContain(Guid.Empty);
//    }

//    [Test]
//    public async Task Handle_ShouldReturnCreatedResultId_WhenSuccessful()
//    {
//        // Arrange
//        var testOrderId = Guid.NewGuid();
//        var command = new CreateTRSyncUpCommand
//        {
//            TestOrderId = testOrderId,
//            TestName = "Test",
//            Value = "100",
//            Flag = "Normal"
//        };

//        Guid? capturedResultId = null;

//        _testResultRepositoryMock
//            .Setup(r => r.TestOrderExistsAsync(testOrderId))
//            .ReturnsAsync(true);

//        _testResultRepositoryMock
//            .Setup(r => r.AddTestResultAsync(It.IsAny<TestResult>()))
//            .Callback<TestResult>(tr => capturedResultId = tr.ResultId)
//            .Returns(Task.CompletedTask);

//        _testResultRepositoryMock
//            .Setup(r => r.SaveChangesAsync())
//            .Returns(Task.CompletedTask);

//        // Act
//        var returnedId = await _handler.Handle(command, CancellationToken.None);

//        // Assert
//        returnedId.Should().NotBe(Guid.Empty);
//        returnedId.Should().Be((Guid)capturedResultId);
//    }

//    [Test]
//    public async Task Handle_ShouldSetCorrectTestOrderId_FromCommand()
//    {
//        // Arrange
//        var testOrderId = Guid.NewGuid();
//        var command = new CreateTRSyncUpCommand
//        {
//            TestOrderId = testOrderId,
//            TestName = "Test",
//            Value = "100",
//            Flag = "Normal"
//        };

//        TestResult? capturedResult = null;

//        _testResultRepositoryMock
//            .Setup(r => r.TestOrderExistsAsync(testOrderId))
//            .ReturnsAsync(true);

//        _testResultRepositoryMock
//            .Setup(r => r.AddTestResultAsync(It.IsAny<TestResult>()))
//            .Callback<TestResult>(tr => capturedResult = tr)
//            .Returns(Task.CompletedTask);

//        _testResultRepositoryMock
//            .Setup(r => r.SaveChangesAsync())
//            .Returns(Task.CompletedTask);

//        // Act
//        await _handler.Handle(command, CancellationToken.None);

//        // Assert
//        capturedResult.Should().NotBeNull();
//        capturedResult!.TestOrderId.Should().Be(testOrderId);
//    }

//    [Test]
//    public async Task Handle_ShouldAcceptHighFlag_WhenValueAboveNormal()
//    {
//        // Arrange
//        var testOrderId = Guid.NewGuid();
//        var command = new CreateTRSyncUpCommand
//        {
//            TestOrderId = testOrderId,
//            TestName = "Cholesterol",
//            Value = "250",
//            ReferenceRange = "125 - 200",
//            Flag = "High"
//        };

//        TestResult? capturedResult = null;

//        _testResultRepositoryMock
//            .Setup(r => r.TestOrderExistsAsync(testOrderId))
//            .ReturnsAsync(true);

//        _testResultRepositoryMock
//            .Setup(r => r.AddTestResultAsync(It.IsAny<TestResult>()))
//            .Callback<TestResult>(tr => capturedResult = tr)
//            .Returns(Task.CompletedTask);

//        _testResultRepositoryMock
//            .Setup(r => r.SaveChangesAsync())
//            .Returns(Task.CompletedTask);

//        // Act
//        await _handler.Handle(command, CancellationToken.None);

//        // Assert
//        capturedResult.Should().NotBeNull();
//        capturedResult!.Flag.Should().Be("High");
//        capturedResult.Value.Should().Be("250");
//    }

//    [Test]
//    public async Task Handle_ShouldAcceptLowFlag_WhenValueBelowNormal()
//    {
//        // Arrange
//        var testOrderId = Guid.NewGuid();
//        var command = new CreateTRSyncUpCommand
//        {
//            TestOrderId = testOrderId,
//            TestName = "Hemoglobin",
//            Value = "9.5",
//            ReferenceRange = "12 - 16",
//            Flag = "Low"
//        };

//        TestResult? capturedResult = null;

//        _testResultRepositoryMock
//            .Setup(r => r.TestOrderExistsAsync(testOrderId))
//            .ReturnsAsync(true);

//        _testResultRepositoryMock
//            .Setup(r => r.AddTestResultAsync(It.IsAny<TestResult>()))
//            .Callback<TestResult>(tr => capturedResult = tr)
//            .Returns(Task.CompletedTask);

//        _testResultRepositoryMock
//            .Setup(r => r.SaveChangesAsync())
//            .Returns(Task.CompletedTask);

//        // Act
//        await _handler.Handle(command, CancellationToken.None);

//        // Assert
//        capturedResult.Should().NotBeNull();
//        capturedResult!.Flag.Should().Be("Low");
//        capturedResult.Value.Should().Be("9.5");
//    }

//    [Test]
//    public async Task Handle_ShouldHandleNumericValues_Correctly()
//    {
//        // Arrange
//        var testOrderId = Guid.NewGuid();
//        var command = new CreateTRSyncUpCommand
//        {
//            TestOrderId = testOrderId,
//            TestName = "White Blood Cell Count",
//            Value = "7500",
//            ReferenceRange = "4000 - 11000",
//            Flag = "Normal"
//        };

//        TestResult? capturedResult = null;

//        _testResultRepositoryMock
//            .Setup(r => r.TestOrderExistsAsync(testOrderId))
//            .ReturnsAsync(true);

//        _testResultRepositoryMock
//            .Setup(r => r.AddTestResultAsync(It.IsAny<TestResult>()))
//            .Callback<TestResult>(tr => capturedResult = tr)
//            .Returns(Task.CompletedTask);

//        _testResultRepositoryMock
//            .Setup(r => r.SaveChangesAsync())
//            .Returns(Task.CompletedTask);

//        // Act
//        await _handler.Handle(command, CancellationToken.None);

//        // Assert
//        capturedResult.Should().NotBeNull();
//        capturedResult!.Value.Should().Be("7500");
//    }

//    [Test]
//    public async Task Handle_ShouldHandleTextValues_Correctly()
//    {
//        // Arrange
//        var testOrderId = Guid.NewGuid();
//        var command = new CreateTRSyncUpCommand
//        {
//            TestOrderId = testOrderId,
//            TestName = "COVID-19 Test",
//            Value = "Negative",
//            Flag = "Normal"
//        };

//        TestResult? capturedResult = null;

//        _testResultRepositoryMock
//            .Setup(r => r.TestOrderExistsAsync(testOrderId))
//            .ReturnsAsync(true);

//        _testResultRepositoryMock
//            .Setup(r => r.AddTestResultAsync(It.IsAny<TestResult>()))
//            .Callback<TestResult>(tr => capturedResult = tr)
//            .Returns(Task.CompletedTask);

//        _testResultRepositoryMock
//            .Setup(r => r.SaveChangesAsync())
//            .Returns(Task.CompletedTask);

//        // Act
//        await _handler.Handle(command, CancellationToken.None);

//        // Assert
//        capturedResult.Should().NotBeNull();
//        capturedResult!.Value.Should().Be("Negative");
//    }
//}
