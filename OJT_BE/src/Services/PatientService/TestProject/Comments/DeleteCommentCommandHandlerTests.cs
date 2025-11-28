using NUnit.Framework;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using PatientService.Application.UseCases.CommentsUC.Delete;
using PatientService.Domain.Entities.TestOrder;
using PatientService.Domain.Interfaces;

namespace PatientService.Tests.UseCases.CommentsUC
{
    public class DeleteCommentCommandHandlerTests
    {
        private Mock<ICommentRepository> _commentRepoMock;
        private Mock<IAuditLogRepository> _auditRepoMock;
        private DeleteCommentCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _commentRepoMock = new Mock<ICommentRepository>();
            _auditRepoMock = new Mock<IAuditLogRepository>();
            _handler = new DeleteCommentCommandHandler(_commentRepoMock.Object, _auditRepoMock.Object);
        }

        [Test]
        public void Handle_ShouldThrow_WhenCommentNotFound()
        {
            _commentRepoMock.Setup(x => x.GetCommentForUpdateAsync(It.IsAny<Guid>())).ReturnsAsync((Comment)null);
            var cmd = new DeleteCommentCommand { CommentId = Guid.NewGuid() };

            Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(cmd, CancellationToken.None));
        }

       

        [Test]
        public async Task Handle_ShouldDelete_WhenAdmin()
        {
            var comment = new Comment { CommentId = Guid.NewGuid(), UserName = "Someone", TestOrderId = Guid.NewGuid(), Content = "C" };
            _commentRepoMock.Setup(x => x.GetCommentForUpdateAsync(It.IsAny<Guid>())).ReturnsAsync(comment);

            var cmd = new DeleteCommentCommand
            {
                CommentId = comment.CommentId,
                CurrentUserName = "Admin",
                CurrentUserRole = "ADMIN"
            };

            await _handler.Handle(cmd, CancellationToken.None);

            _auditRepoMock.Verify(x => x.AddLogAsync(It.IsAny<TestOrderAuditLog>()), Times.Once);
            _commentRepoMock.Verify(x => x.DeleteCommentAsync(comment.CommentId), Times.Once);
            _commentRepoMock.Verify(x => x.SaveChangeAsync(), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldDelete_WhenOwner()
        {
            var comment = new Comment { CommentId = Guid.NewGuid(), UserName = "Tester", TestOrderId = Guid.NewGuid(), Content = "Old" };
            _commentRepoMock.Setup(x => x.GetCommentForUpdateAsync(It.IsAny<Guid>())).ReturnsAsync(comment);

            var cmd = new DeleteCommentCommand
            {
                CommentId = comment.CommentId,
                CurrentUserName = "Tester",
                CurrentUserRole = "USER"
            };

            await _handler.Handle(cmd, CancellationToken.None);

            _auditRepoMock.Verify(x => x.AddLogAsync(It.IsAny<TestOrderAuditLog>()), Times.Once);
            _commentRepoMock.Verify(x => x.DeleteCommentAsync(comment.CommentId), Times.Once);
            _commentRepoMock.Verify(x => x.SaveChangeAsync(), Times.Once);
        }
    }
}
