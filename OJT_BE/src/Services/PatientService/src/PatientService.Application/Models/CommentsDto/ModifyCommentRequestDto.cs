namespace PatientService.Application.Dtos.CommentsDto
{
    /// <summary>
    /// Data Transfer Object (DTO) used when modifying an existing comment.
    /// This represents the JSON payload the API expects for a modification request.
    /// </summary>
    public class ModifyCommentRequestDto
    {
        /// <summary>
        /// Gets or sets the new text content that will replace the comment's old content.
        /// </summary>
        /// <value>
        /// The new comment text. Defaults to <see cref="string.Empty"/>.
        /// </value>
        public string NewContent { get; set; } = string.Empty;
    }
}