
using PatientService.Application.Dtos.CommentsDto;
using PatientService.Application.Interfaces;
using PatientService.Application.UseCases.CommentsUC.Add;
using PatientService.Domain.Entities.TestOrder;
using PatientService.Domain.Interfaces;

namespace PatientService.Application.CommentsUC.Comments.Add
{
    /// <summary>
    /// Handles the <see cref="AddCommentCommand"/> to create a new comment
    /// associated with a <see cref="TestOrder"/>.
    /// </summary>
    public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, CommentResponseDto>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IIamUserClient _iamUserClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddCommentCommandHandler"/> class.
        /// </summary>
        /// <param name="commentRepository">The repository for comment data operations.</param>
        /// <param name="testOrderRepository">The repository for TestOrder query operations.</param>
        public AddCommentCommandHandler(ICommentRepository commentRepository, IIamUserClient iamUserClient)
        {
            _commentRepository = commentRepository;
            _iamUserClient = iamUserClient;
        }

        /// <summary>
        /// Handles the incoming command to add a new comment to a Test Order.
        /// </summary>
        /// <param name="request">The <see cref="AddCommentCommand"/> containing comment details.</param>
        /// <param name="cancellationToken">A token for cancelling the operation.</param>
        /// <returns>
        /// A <see cref="Task"/> that results in the newly created <see cref="CommentResponseDto"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown if the comment content is null or empty.
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        /// Thrown if the specified <see cref="AddCommentCommand.TestOrderId"/> does not exist.
        /// </exception>
        public async Task<CommentResponseDto> Handle(AddCommentCommand request, CancellationToken cancellationToken)
        {
            // --- Validation ---
            if (string.IsNullOrWhiteSpace(request.Content))
            {
                throw new ArgumentException("Comment content cannot be empty.");
            }

            // --- NEW: resolve CreatedBy GUID to full name via IAM ---
            string? createdByName = null;
            // createdBy stored in testOrder.CreatedBy is a GUID string (per your note) — ensure parseable
            if (!string.IsNullOrWhiteSpace(request.UserName) && Guid.TryParse(request.UserName, out var createdByGuid))
            {
                createdByName = await _iamUserClient.GetUserFullNameAsync(createdByGuid, cancellationToken);
            }

            // fallback: if no name found, use original stored id or "Unknown"
            var createdByDisplay = !string.IsNullOrWhiteSpace(createdByName)
                ? createdByName
                : (!string.IsNullOrWhiteSpace(request.UserName) ? request.UserName : "Unknown");

            // --- Create Entity ---
            var comment = new Comment
            {
                CommentId = Guid.NewGuid(),
                TestOrderId = request.TestOrderId,
                Content = request.Content, // The original, plain-text content
                UserName = request.CurrentUserName,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // --- Persist to Database ---
            await _commentRepository.AddCommentAsync(comment);
            await _commentRepository.SaveChangeAsync();

            // --- Map to DTO and Return ---
            // We map the entity to a DTO for the API response.
            return new CommentResponseDto
            {
                CommentId = comment.CommentId,
                TestOrderId = comment.TestOrderId,
                ResultId = comment.ResultId, 
                UserName = createdByName,
                Content = comment.Content, 
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt
            };
        }
    }
}