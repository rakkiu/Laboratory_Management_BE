using Moq;
using NUnit.Framework;
using PatientService.Application.CommentsUC.Comments.Add;
using PatientService.Application.Dtos.CommentsDto;
using PatientService.Application.Interfaces;
using PatientService.Application.UseCases.CommentsUC.Add;
using PatientService.Domain.Entities.TestOrder;
using PatientService.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PatientService.Tests.UseCases.CommentsUC
{
    [TestFixture]
    public class AddCommentCommandHandlerTests
    {
        private Mock<ICommentRepository> _commentRepositoryMock = null!;
        private Mock<IIamUserClient> _iamUserClientMock = null!;
        private AddCommentCommandHandler _handler = null!;

        [SetUp]
        public void Setup()
        {
            _commentRepositoryMock = new Mock<ICommentRepository>();
            _iamUserClientMock = new Mock<IIamUserClient>();

            // Đặt default mock cho GetUserFullNameAsync
            _iamUserClientMock
                .Setup(x => x.GetUserFullNameAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("Mock User FullName");

            _handler = new AddCommentCommandHandler(
                _commentRepositoryMock.Object,
                _iamUserClientMock.Object
            );
        }

        [Test]
        public async Task Handle_ValidRequest_ShouldAddCommentAndReturnDto()
        {
            // Arrange
            var command = new AddCommentCommand
            {
                TestOrderId = Guid.NewGuid(),
                Content = "This is a test comment",
                CurrentUserName = "tester",
                UserName = Guid.NewGuid().ToString()
            };

            _commentRepositoryMock
                .Setup(r => r.AddCommentAsync(It.IsAny<Comment>()))
                .Returns(Task.CompletedTask);

            _commentRepositoryMock
                .Setup(r => r.SaveChangeAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.TestOrderId, Is.EqualTo(command.TestOrderId));
            Assert.That(result.Content, Is.EqualTo(command.Content));
            Assert.That(result.UserName, Is.EqualTo("Mock User FullName")); // vì mock trả về tên

            _commentRepositoryMock.Verify(r => r.AddCommentAsync(It.IsAny<Comment>()), Times.Once);
            _commentRepositoryMock.Verify(r => r.SaveChangeAsync(), Times.Once);
        }

        [Test]
        public void Handle_EmptyContent_ShouldThrowArgumentException()
        {
            var command = new AddCommentCommand
            {
                TestOrderId = Guid.NewGuid(),
                Content = "   ",
                CurrentUserName = "tester"
            };

            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _handler.Handle(command, CancellationToken.None));

            Assert.That(ex!.Message, Is.EqualTo("Comment content cannot be empty."));
            _commentRepositoryMock.Verify(r => r.AddCommentAsync(It.IsAny<Comment>()), Times.Never);
        }

        [Test]
        public async Task Handle_ShouldSetTimestampsAndIdsProperly()
        {
            var command = new AddCommentCommand
            {
                TestOrderId = Guid.NewGuid(),
                Content = "Timestamp test",
                CurrentUserName = "tester",
                UserName = Guid.NewGuid().ToString()
            };

            Comment? captured = null;

            _commentRepositoryMock
                .Setup(r => r.AddCommentAsync(It.IsAny<Comment>()))
                .Callback<Comment>(c => captured = c);

            _commentRepositoryMock.Setup(r => r.SaveChangeAsync()).Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.That(captured, Is.Not.Null);
            Assert.That(captured!.CommentId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(captured.CreatedAt, Is.EqualTo(captured.UpdatedAt).Within(TimeSpan.FromSeconds(1)));
            Assert.That(result.CommentId, Is.EqualTo(captured.CommentId));
        }
    }
}
