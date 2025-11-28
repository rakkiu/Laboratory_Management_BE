using NUnit.Framework;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using PatientService.Application.UseCases.CommentsUC.Modify;
using PatientService.Domain.Entities.TestOrder;
using PatientService.Domain.Interfaces;

namespace PatientService.Tests.UseCases.CommentsUC
{
    public class ModifyCommentCommandHandlerTests
    {
        private Mock<ICommentRepository> _commentRepoMock;
        private Mock<IAuditLogRepository> _auditRepoMock;
        private ModifyCommentCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _commentRepoMock = new Mock<ICommentRepository>();
            _auditRepoMock = new Mock<IAuditLogRepository>();
            _handler = new ModifyCommentCommandHandler(_commentRepoMock.Object, _auditRepoMock.Object);
        }

        [Test]
        public void Handle_ShouldThrow_WhenCommentNotFound()
        {
            _commentRepoMock.Setup(x => x.GetCommentForUpdateAsync(It.IsAny<Guid>())).ReturnsAsync((Comment)null);

            var cmd = new ModifyCommentCommand { CommentId = Guid.NewGuid(), NewContent = "abc" };
            Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(cmd, CancellationToken.None));
        }

        [Test]
        public void Handle_ShouldThrow_WhenContentEmpty()
        {
            _commentRepoMock.Setup(x => x.GetCommentForUpdateAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Comment { CommentId = Guid.NewGuid(), UserName = "Tester" });

            var cmd = new ModifyCommentCommand { CommentId = Guid.NewGuid(), NewContent = "", CurrentUserName = "Tester" };
            Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(cmd, CancellationToken.None));
        }

       

        [Test]
        public async Task Handle_ShouldUpdate_Successfully()
        {
            var comment = new Comment { CommentId = Guid.NewGuid(), UserName = "Tester", Content = "old" };
            _commentRepoMock.Setup(x => x.GetCommentForUpdateAsync(It.IsAny<Guid>())).ReturnsAsync(comment);

            var cmd = new ModifyCommentCommand
            {
                CommentId = comment.CommentId,
                NewContent = "new",
                CurrentUserName = "Tester",
                CurrentUserRole = "USER"
            };

            await _handler.Handle(cmd, CancellationToken.None);

            _commentRepoMock.Verify(x => x.UpdateComment(It.Is<Comment>(c => c.Content == "new")), Times.Once);
            _auditRepoMock.Verify(x => x.AddLogAsync(It.IsAny<TestOrderAuditLog>()), Times.Once);
            _commentRepoMock.Verify(x => x.SaveChangeAsync(), Times.Once);
        }
    }
}
