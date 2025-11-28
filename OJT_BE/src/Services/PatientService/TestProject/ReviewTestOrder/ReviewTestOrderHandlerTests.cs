using Moq;
using NUnit.Framework;
using PatientService.Application.UseCases.TestOrderUC.TestOrders.Commands.ReviewTestOrder;
using PatientService.Domain.Entities.TestOrder;
using PatientService.Domain.Interfaces;
using PatientService.Domain.Interfaces.TestOrderService;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestProject.ReviewTestOrder
{
    [TestFixture]
    public class ReviewTestOrderHandlerTests
    {
        private Mock<ITestOrderRepository> _mockTestOrderRepo = null!;
        private Mock<IAuditLogRepository> _mockAuditLogRepo = null!;
        private ReviewTestOrderHandler _handler = null!;

        [SetUp]
        public void SetUp()
        {
            _mockTestOrderRepo = new Mock<ITestOrderRepository>();
            _mockAuditLogRepo = new Mock<IAuditLogRepository>();
            _handler = new ReviewTestOrderHandler(_mockTestOrderRepo.Object, _mockAuditLogRepo.Object);
        }

        // 1. TestOrder không tồn tại → false
        [Test]
        public async Task Handle_TestOrderNotFound_ReturnsFalse()
        {
            var command = new ReviewTestOrderCommand
            {
                TestOrderId = Guid.NewGuid(),
                ReviewedBy = Guid.NewGuid()
            };

            _mockTestOrderRepo
                .Setup(r => r.GetByIdWithResultsAsync(command.TestOrderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((TestOrder?)null);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.That(result, Is.False);
            _mockAuditLogRepo.Verify(r => r.AddLogAsync(It.IsAny<TestOrderAuditLog>()), Times.Never);
            _mockTestOrderRepo.Verify(r => r.SaveChangeAsync(), Times.Never);
        }

        // 2. TestOrder bị xoá → false
        [Test]
        public async Task Handle_TestOrderDeleted_ReturnsFalse()
        {
            var testOrder = new TestOrder
            {
                TestOrderId = Guid.NewGuid(),
                IsDeleted = true
            };

            var command = new ReviewTestOrderCommand
            {
                TestOrderId = testOrder.TestOrderId,
                ReviewedBy = Guid.NewGuid()
            };

            _mockTestOrderRepo
                .Setup(r => r.GetByIdWithResultsAsync(testOrder.TestOrderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(testOrder);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.That(result, Is.False);
        }

        // 3. Review thành công → cập nhật ReviewedBy và ReviewedAt
        [Test]
        public async Task Handle_ReviewSuccess_UpdatesReviewFields()
        {
            var reviewerId = Guid.NewGuid();

            var testOrder = new TestOrder
            {
                TestOrderId = Guid.NewGuid(),
                IsDeleted = false
            };

            var command = new ReviewTestOrderCommand
            {
                TestOrderId = testOrder.TestOrderId,
                ReviewedBy = reviewerId
            };

            _mockTestOrderRepo
                .Setup(r => r.GetByIdWithResultsAsync(testOrder.TestOrderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(testOrder);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.That(result, Is.True);
            Assert.That(testOrder.ReviewedBy, Is.EqualTo(reviewerId.ToString()));
            Assert.That(testOrder.ReviewedAt, Is.Not.Null);

            _mockAuditLogRepo.Verify(r => r.AddLogAsync(It.IsAny<TestOrderAuditLog>()), Times.Once);
            _mockTestOrderRepo.Verify(r => r.SaveChangeAsync(), Times.Once);
        }

        // 4. AuditLog được tạo đúng
        [Test]
        public async Task Handle_ShouldCreateAuditLog()
        {
            var reviewerId = Guid.NewGuid();

            var testOrder = new TestOrder
            {
                TestOrderId = Guid.NewGuid(),
                IsDeleted = false
            };

            var command = new ReviewTestOrderCommand
            {
                TestOrderId = testOrder.TestOrderId,
                ReviewedBy = reviewerId
            };

            _mockTestOrderRepo
                .Setup(r => r.GetByIdWithResultsAsync(testOrder.TestOrderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(testOrder);

            TestOrderAuditLog? capturedLog = null;

            _mockAuditLogRepo
                .Setup(r => r.AddLogAsync(It.IsAny<TestOrderAuditLog>()))
                .Callback<TestOrderAuditLog>(log => capturedLog = log)
                .Returns(Task.CompletedTask);

            await _handler.Handle(command, CancellationToken.None);

            Assert.That(capturedLog, Is.Not.Null);
            Assert.That(capturedLog!.TestOrderId, Is.EqualTo(testOrder.TestOrderId));
            Assert.That(capturedLog.UserId, Is.EqualTo(reviewerId.ToString()));
            Assert.That(capturedLog.ActionType, Is.EqualTo("REVIEW_TEST_ORDER"));
        }
    }
}
