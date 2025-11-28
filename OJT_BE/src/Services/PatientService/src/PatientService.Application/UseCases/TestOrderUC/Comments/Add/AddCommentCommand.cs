using MediatR;
using System;
using PatientService.Application.Dtos.CommentsDto;

namespace PatientService.Application.UseCases.CommentsUC.Add
{
    /// <summary>
    /// Represents a MediatR command to add a new comment to a specific Test Order.
    /// Upon successful execution, this command returns a <see cref="CommentResponseDto"/>.
    /// </summary>
    public class AddCommentCommand : IRequest<CommentResponseDto>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Test Order to which
        /// this comment will be associated.
        /// </summary>
        public Guid TestOrderId { get; set; }

        /// <summary>
        /// Gets or sets the text content of the new comment.
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display name of the user creating the comment.
        /// </summary>
        /// <remarks>
        /// This property is typically set by the controller or handler
        /// after retrieving the user's identity.
        /// </remarks>
        public string CurrentUserName { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;
        public Guid? ResultId { get; set; }
    }
}