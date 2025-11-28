using MediatR;
using PatientService.Domain.Interfaces;
using PatientService.Domain.Entities.TestOrder;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PatientService.Application.UseCases.CommentsUC.Delete
{
    /// <summary>
    /// Handles the <see cref="DeleteCommentCommand"/> to delete an existing comment
    /// and create an audit log for the action.
    /// </summary>
    public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IAuditLogRepository _auditLogRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCommentCommandHandler"/> class.
        /// </summary>
        /// <param name="commentRepository">The repository for comment data operations.</param>
        /// <param name="auditLogRepository">The repository for audit log operations.</param>
        public DeleteCommentCommandHandler(ICommentRepository commentRepository,
                                             IAuditLogRepository auditLogRepository)
        {
            _commentRepository = commentRepository;
            _auditLogRepository = auditLogRepository;
        }

        /// <summary>
        /// Handles the incoming command to delete a comment.
        /// </summary>
        /// <param name="request">The <see cref="DeleteCommentCommand"/> containing the ID of the comment to delete.</param>
        /// <param name="cancellationToken">A token for cancelling the operation.</param>
        /// <returns>A <see cref="Task"/> representing the completion of the operation.</returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown if the comment with the specified ID is not found.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if the user attempts to delete a comment not created by them
        /// and does not have an admin role (Forbidden).
        /// </exception>
        public async Task Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            // Retrieve the comment, ensuring it is tracked for deletion
            var comment = await _commentRepository.GetCommentForUpdateAsync(request.CommentId);

            if (comment == null)
            {
                throw new KeyNotFoundException("Comment not found.");
            }



            // 1. Create an audit log for the deletion
            var auditLog = new TestOrderAuditLog
            {
                TestOrderId = comment.TestOrderId,
                UserId = request.CurrentUserName,
                ActionType = "DELETE_COMMENT",
                Timestamp = DateTime.UtcNow,

                // Log the details of the deleted comment
                ChangedFields = $"Comment (ID: {comment.CommentId}) was deleted. | Last_Content: '{comment.Content}'"
            };

            // 2. Add the log to the context
            await _auditLogRepository.AddLogAsync(auditLog);

            // 3. Mark the comment for deletion
            await _commentRepository.DeleteCommentAsync(request.CommentId);

            // 4. Save both the audit log and the comment deletion in a single transaction
            await _commentRepository.SaveChangeAsync();
        }
    }
}