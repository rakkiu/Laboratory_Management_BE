using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using PatientService.Application.UseCases.TestOrderUC.TestOrders.Commands.DeleteTestOrder;
using PatientService.Domain.Entities.TestOrder;
using PatientService.Domain.Interfaces;
using PatientService.Domain.Interfaces.TestOrderService;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace TestProject.DeleteTestOrder
{
    [TestFixture]
    public class DeleteTestOrderHandlerTests
    {
        private Mock<ITestOrderRepository> _mockTestOrderRepo = null!;
        private Mock<IAuditLogRepository> _mockAuditLogRepo = null!;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor = null!;
        private DeleteTestOrderHandler _handler = null!;

        [SetUp]
        public void SetUp()
        {
            _mockTestOrderRepo = new Mock<ITestOrderRepository>();
            _mockAuditLogRepo = new Mock<IAuditLogRepository>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            // Setup default HttpContext with a user claim
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                new Claim("userId", "test-user-id")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            };

            _mockHttpContextAccessor
                .Setup(x => x.HttpContext)
                .Returns(httpContext);

            _handler = new DeleteTestOrderHandler(
                _mockTestOrderRepo.Object, 
                _mockAuditLogRepo.Object,
                _mockHttpContextAccessor.Object);
        }

        /// <summary>
        /// Test Case 1: Test order không tồn tại → trả về false
        /// </summary>
        [Test]
        public async Task Handle_TestOrderNotFound_ReturnsFalse()
        {
            // Arrange
            var command = new DeleteTestOrderCommand
            {
                TestOrderId = Guid.NewGuid(),
                DeletedBy = "lab.user"
            };

            _mockTestOrderRepo
                .Setup(r => r.GetByIdWithResultsAsync(command.TestOrderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((TestOrder?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.False);
            _mockAuditLogRepo.Verify(r => r.AddLogAsync(It.IsAny<TestOrderAuditLog>()), Times.Never);
            _mockTestOrderRepo.Verify(r => r.SaveChangeAsync(), Times.Never);
        }

        /// <summary>
        /// Test Case 2: Test order đã bị xóa trước đó → trả về false
        /// </summary>
        [Test]
        public async Task Handle_TestOrderAlreadyDeleted_ReturnsFalse()
        {
            // Arrange
            var existingTestOrder = new TestOrder
            {
                TestOrderId = Guid.NewGuid(),
                PatientName = "John Doe",
                Status = "Completed",
                IsDeleted = true, // Already deleted
                CreatedBy = "doctor.smith",
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            };

            var command = new DeleteTestOrderCommand
            {
                TestOrderId = existingTestOrder.TestOrderId,
                DeletedBy = "lab.user"
            };

            _mockTestOrderRepo
                .Setup(r => r.GetByIdWithResultsAsync(command.TestOrderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingTestOrder);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.False);
            _mockAuditLogRepo.Verify(r => r.AddLogAsync(It.IsAny<TestOrderAuditLog>()), Times.Never);
            _mockTestOrderRepo.Verify(r => r.SaveChangeAsync(), Times.Never);
        }

        /// <summary>
        /// Test Case 3: Test order tồn tại → xóa thành công và tạo audit log
        /// </summary>
        [Test]
        public async Task Handle_TestOrderExists_SoftDeletesAndCreatesAuditLog()
        {
            // Arrange
            var existingTestOrder = new TestOrder
            {
                TestOrderId = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                PatientName = "Jane Smith",
                Status = "Pending",
                IsDeleted = false,
                CreatedBy = "doctor.jones",
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            };

            var command = new DeleteTestOrderCommand
            {
                TestOrderId = existingTestOrder.TestOrderId,
                DeletedBy = "lab.manager"
            };

            _mockTestOrderRepo
                .Setup(r => r.GetByIdWithResultsAsync(command.TestOrderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingTestOrder);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(existingTestOrder.IsDeleted, Is.True);

            // Verify audit log was created with UserId from HttpContext
            _mockAuditLogRepo.Verify(r => r.AddLogAsync(It.Is<TestOrderAuditLog>(log =>
                log.TestOrderId == existingTestOrder.TestOrderId &&
                log.UserId == "test-user-id" && // UserId from HttpContext, not DeletedBy
                log.ActionType == "DELETE_TEST_ORDER" &&
                log.ChangedFields!.Contains(existingTestOrder.PatientName)
            )), Times.Once);

            // Verify SaveChanges was called
            _mockTestOrderRepo.Verify(r => r.SaveChangeAsync(), Times.Once);
        }

        /// <summary>
        /// Test Case 4: Kiểm tra UserId được lấy từ HttpContext
        /// </summary>
        [Test]
        public async Task Handle_ShouldUseUserIdFromHttpContext()
        {
            // Arrange
            var customUserId = "custom-user-123";
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, customUserId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            };

            _mockHttpContextAccessor
                .Setup(x => x.HttpContext)
                .Returns(httpContext);

            var testOrder = new TestOrder
            {
                TestOrderId = Guid.NewGuid(),
                PatientName = "Test Patient",
                Status = "Pending",
                IsDeleted = false,
                CreatedBy = "doctor",
                CreatedAt = DateTime.UtcNow
            };

            var command = new DeleteTestOrderCommand
            {
                TestOrderId = testOrder.TestOrderId,
                DeletedBy = "ignored-value"
            };

            _mockTestOrderRepo
                .Setup(r => r.GetByIdWithResultsAsync(command.TestOrderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(testOrder);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert - UserId should come from HttpContext, not command.DeletedBy
            _mockAuditLogRepo.Verify(r => r.AddLogAsync(It.Is<TestOrderAuditLog>(log =>
                log.UserId == customUserId
            )), Times.Once);
        }

        /// <summary>
        /// Test Case 5: Khi HttpContext không có User claim → sử dụng "Unknown"
        /// </summary>
        [Test]
        public async Task Handle_WhenNoUserClaim_ShouldUseUnknown()
        {
            // Arrange
            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal() // No claims
            };

            _mockHttpContextAccessor
                .Setup(x => x.HttpContext)
                .Returns(httpContext);

            var testOrder = new TestOrder
            {
                TestOrderId = Guid.NewGuid(),
                PatientName = "Test Patient",
                Status = "Pending",
                IsDeleted = false,
                CreatedBy = "doctor",
                CreatedAt = DateTime.UtcNow
            };

            var command = new DeleteTestOrderCommand
            {
                TestOrderId = testOrder.TestOrderId,
                DeletedBy = "lab.user"
            };

            _mockTestOrderRepo
                .Setup(r => r.GetByIdWithResultsAsync(command.TestOrderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(testOrder);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert - Should use "Unknown" when no user claim is found
            _mockAuditLogRepo.Verify(r => r.AddLogAsync(It.Is<TestOrderAuditLog>(log =>
                log.UserId == "Unknown"
            )), Times.Once);
        }
    }
}