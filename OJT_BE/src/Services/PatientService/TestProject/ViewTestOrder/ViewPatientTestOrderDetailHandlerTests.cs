using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using PatientService.Application.Models.TestOrderDto;
using PatientService.Application.UseCases.TestOrderUC.ViewDetailPatientTestOrder.Queries;
using PatientService.Domain.Interfaces.TestOrderService;
using PatientService.Application.Interfaces;

namespace UnitTests.TestOrder
{
    [TestFixture]
    public class ViewPatientTestOrderDetailHandlerTests
    {
        private Mock<ITestOrderRepository> _repositoryMock;
        private Mock<IIamUserClient> _iamUserClientMock;
        private ViewPatientTestOrderDetailHandler _handler;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<ITestOrderRepository>();
            _iamUserClientMock = new Mock<IIamUserClient>();

            // Default: return null for any GetUserFullNameAsync
            _iamUserClientMock
                .Setup(x => x.GetUserFullNameAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((string)null);

            _handler = new ViewPatientTestOrderDetailHandler(
                _repositoryMock.Object,
                _iamUserClientMock.Object
            );
        }

        // =========================================================
        // 1. ORDER NOT FOUND
        // =========================================================
        [Test]
        public async Task Handle_OrderNotFound_ReturnsNotFoundMessage()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetDetailAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((PatientService.Domain.Entities.TestOrder.TestOrder)null);

            var query = new ViewPatientTestOrderDetailQuery(Guid.NewGuid());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Message.Should().Be("Test order not found.");
            result.TestOrder.Should().BeNull();
            result.TestResults.Should().BeEmpty();
            result.Comments.Should().BeEmpty();
        }

        // =========================================================
        // 2. ORDER EXISTS BUT NOT COMPLETED
        // =========================================================
        [Test]
        public async Task Handle_OrderExists_NotCompleted_NoTestResults()
        {
            // Arrange
            var order = new PatientService.Domain.Entities.TestOrder.TestOrder
            {
                TestOrderId = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                PatientName = "John Doe",
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _repositoryMock
                .Setup(r => r.GetDetailAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var query = new ViewPatientTestOrderDetailQuery(order.TestOrderId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.TestOrder.Should().NotBeNull();
            result.TestOrder!.PatientName.Should().Be("John Doe");

            result.TestResults.Should().BeEmpty();
            result.Comments.Should().BeEmpty();

            result.Message.Should().Be("Test order is pending. No test results available yet.");
        }

        // =========================================================
        // 3. ORDER COMPLETED WITH RESULTS + COMMENTS
        // =========================================================
        [Test]
        public async Task Handle_OrderCompleted_ReturnsFlattenedResultsAndComments()
        {
            // Arrange
            var resultId = Guid.NewGuid();
            var createdUser = Guid.NewGuid();

            var order = new PatientService.Domain.Entities.TestOrder.TestOrder
            {
                TestOrderId = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                PatientName = "Alice",
                Status = "Complete",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdUser.ToString(),

                TestResults = new List<PatientService.Domain.Entities.TestOrder.TestResult>
                {
                    new PatientService.Domain.Entities.TestOrder.TestResult
                    {
                        ResultId = resultId,
                        TestName = "CBC",
                        Interpretation = "Normal",
                        InstrumentUsed = "Machine A",
                        CreatedAt = DateTime.UtcNow,

                        TestResultDetails = new List<PatientService.Domain.Entities.TestOrder.TestResultDetail>
                        {
                            new PatientService.Domain.Entities.TestOrder.TestResultDetail
                            {
                                TestResultDetailId = Guid.NewGuid(),
                                ResultId = resultId,
                                Type = "HGB",
                                Value = 15.2,
                                Flag = "Normal",
                                ReferenceRange = "14 - 18"
                            }
                        }
                    }
                },

                Comments = new List<PatientService.Domain.Entities.TestOrder.Comment>
                {
                    new PatientService.Domain.Entities.TestOrder.Comment
                    {
                        CommentId = Guid.NewGuid(),
                        UserName = "doctor.a",
                        Content = "All good",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                }
            };

            _repositoryMock
                .Setup(r => r.GetDetailAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            // Mock full name for created user
            _iamUserClientMock
                .Setup(x => x.GetUserFullNameAsync(createdUser, It.IsAny<CancellationToken>()))
                .ReturnsAsync("Dr. Created User");

            var query = new ViewPatientTestOrderDetailQuery(order.TestOrderId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.TestOrder.Should().NotBeNull();
            result.TestOrder.CreatedBy.Should().Be("Dr. Created User");

            result.TestResults.Should().HaveCount(1);
            result.Comments.Should().HaveCount(1);

            var testResult = result.TestResults[0];
            testResult.TestName.Should().Be("HGB");
            testResult.Value.Should().Be("15.2");
            testResult.Flag.Should().Be("Normal");
            testResult.ReferenceRange.Should().Be("14 - 18");

            result.Message.Should().Be("Test order is complete. Test results retrieved.");
        }
    }
}
