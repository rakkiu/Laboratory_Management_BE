using MediatR;
using System;

namespace PatientService.Application.UseCases.CommentsUC.Delete
{
    /// <summary>
    /// Represents a MediatR command to delete an existing comment.
    /// This command does not return any value (it is an <see cref="IRequest"/>).
    /// </summary>
    public class DeleteCommentCommand : IRequest
    {
        /// <summary>
        /// Gets or sets the unique identifier of the comment to be deleted.
        /// </summary>
        public Guid CommentId { get; set; }

        /// <summary>
        /// Gets or sets the display name of the user performing the delete action.
        /// </summary>
        /// <remarks>
        /// This property is typically set by the controller or handler
        /// after retrieving the user's identity, and is used for auditing.
        /// </remarks>
        public string CurrentUserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the role of the user performing the delete action.
        /// </summary>
        /// <remarks>
        /// This property can be used to allow users with specific roles (e.g., "Admin")
        /// to delete comments they do not own.
        /// </remarks>
        public string CurrentUserRole { get; set; } = string.Empty;
    }
}