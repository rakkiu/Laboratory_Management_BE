using MediatR;
using System;

namespace PatientService.Application.UseCases.CommentsUC.Modify
{
    /// <summary>
    /// Represents a MediatR command to modify an existing comment.
    /// This command does not return any value (it is an <see cref="IRequest"/>).
    /// </summary>
    public class ModifyCommentCommand : IRequest
    {
        /// <summary>
        /// Gets or sets the unique identifier of the comment to be modified.
        /// </summary>
        public Guid CommentId { get; set; }

        /// <summary>
        /// Gets or sets the new text content that will replace the comment's old content.
        /// </summary>
        public string NewContent { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display name of the user performing the modification.
        /// </summary>
        /// <remarks>
        /// This property is typically set by the controller or handler
        /// after retrieving the user's identity, and is used for auditing or authorization.
        /// </remarks>
        public string CurrentUserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the role of the user performing the modification.
        /// </summary>
        /// <remarks>
        /// This property can be used to allow users with specific roles (e.g., "Admin")
        /// to modify comments they do not own.
        /// </remarks>
        public string CurrentUserRole { get; set; } = string.Empty;
    }
}