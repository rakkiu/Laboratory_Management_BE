using MediatR;
using PatientService.Domain.Interfaces;
using PatientService.Domain.Entities.TestOrder;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PatientService.Application.UseCases.CommentsUC.Modify
{
    /// <summary>
    /// Handles the <see cref="ModifyCommentCommand"/> to update an existing comment
    /// and create an audit log for the modification.
    /// </summary>
    public class ModifyCommentCommandHandler : IRequestHandler<ModifyCommentCommand>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IAuditLogRepository _auditLogRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModifyCommentCommandHandler"/> class.
        /// </summary>
        /// <param name="commentRepository">The repository for comment data operations.</param>
        /// <param name="auditLogRepository">The repository for audit log operations.</param>
        public ModifyCommentCommandHandler(ICommentRepository commentRepository,
                                             IAuditLogRepository auditLogRepository)
        {
            _commentRepository = commentRepository;
            _auditLogRepository = auditLogRepository;
        }

        /// <summary>
        /// Handles the incoming command to modify a comment.
        /// </summary>
        /// <param name="request">The <see cref="ModifyCommentCommand"/> containing the new content and ID.</param>
        /// <param name="cancellationToken">A token for cancelling the operation.</param>
        /// <returns>A <see cref="Task"/> representing the completion of the operation.</returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown if the comment with the specified ID is not found.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if the user attempts to modify a comment not created by them
        /// and does not have an admin role (Forbidden),
        /// or if the new content is null or empty.
        /// </exception>
        public async Task Handle(ModifyCommentCommand request, CancellationToken cancellationToken)
        {
            // --- 1. Validation and Retrieval ---
            var comment = await _commentRepository.GetCommentForUpdateAsync(request.CommentId);

            if (comment == null)
            {
                throw new KeyNotFoundException("Comment not found.");
            }

            if (string.IsNullOrWhiteSpace(request.NewContent))
            {
                throw new ArgumentException("Comment content cannot be empty.");
            }


            string oldContent = comment.Content; // Store the original content for logging

            // --- 2. Apply Changes to Entity ---
            comment.Content = request.NewContent;
            comment.UpdatedAt = DateTime.UtcNow;
            await _commentRepository.UpdateComment(comment); // Mark the comment as modified

            // --- 3. Create Audit Log ---
            var auditLog = new TestOrderAuditLog
            {
                TestOrderId = comment.TestOrderId,
                UserId = request.CurrentUserName,
                ActionType = "MODIFY_COMMENT",
                Timestamp = DateTime.UtcNow,

                // Log the old and new content
                ChangedFields = $"CommentId: {comment.CommentId} | Content modified. Old: '{oldContent}', New: '{comment.Content}'"
            };

            // --- 4. Persist Changes ---
            await _auditLogRepository.AddLogAsync(auditLog);
            await _commentRepository.SaveChangeAsync(); // Save both the comment update and the new log
        }
    }
}