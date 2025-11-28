namespace PatientService.Application.Dtos.CommentsDto
{
    /// <summary>
    /// Data Transfer Object (DTO) used when adding a new comment.
    /// This represents the payload from a client request (e.g., API body).
    /// </summary>
    public class AddCommentRequestDto
    {
        /// <summary>
        /// Gets or sets the text content of the comment.
        /// </summary>
        /// <value>
        /// The comment text. Defaults to <see cref="string.Empty"/>.
        /// </value>
        public string Content { get; set; } = string.Empty;

        public string CreatedBy { get; set; } = string.Empty;

    }
}