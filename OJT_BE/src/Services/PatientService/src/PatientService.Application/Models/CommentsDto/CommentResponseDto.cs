
namespace PatientService.Application.Dtos.CommentsDto
{
    /// <summary>
    /// Data Transfer Object (DTO) representing a comment to be returned by the API.
    /// This defines the JSON structure that the client will receive.
    /// </summary>
    public class CommentResponseDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the comment.
        /// </summary>
        public Guid CommentId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the parent Test Order.
        /// </summary>
        public Guid TestOrderId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the associated Test Result.
        /// This is nullable, as a comment can belong to an order without a specific result.
        /// </summary>
        public Guid? ResultId { get; set; } // Nullable: A comment may be on the order itself

        /// <summary>
        /// Gets or sets the display name of the user who created the comment.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the text content of the comment.
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the timestamp when the comment was originally created (in UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the comment was last updated (in UTC).
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}