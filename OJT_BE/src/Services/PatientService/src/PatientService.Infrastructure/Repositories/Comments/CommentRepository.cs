using Microsoft.EntityFrameworkCore;
using PatientService.Domain.Entities.TestOrder;
using PatientService.Domain.Interfaces;
using PatientService.Infrastructure.Data;

namespace PatientService.Infrastructure.Repositories
{
    /// <summary>
    /// Implements the <see cref="ICommentRepository"/> interface.
    /// Handles data operations for Comment entities using Entity Framework Core.
    /// </summary>
    public class CommentRepository : ICommentRepository
    {
        /// <summary>
        /// The database context for interacting with patient data.
        /// </summary>
        private readonly PatientDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentRepository"/> class.
        /// </summary>
        /// <param name="context">The <see cref="PatientDbContext"/> injected by DI.</param>
        public CommentRepository(PatientDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Asynchronously adds a new <see cref="Comment"/> to the database.
        /// </summary>
        /// <param name="comment">The comment entity to add.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task AddCommentAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
        }

        /// <summary>
        /// Asynchronously deletes a comment from the database by its ID.
        /// </summary>
        /// <param name="commentId">The unique identifier of the comment to delete.</param>
        /// <returns>
        /// A <see cref="Task"/> resulting in <c>true</c> if the comment was found and deleted,
        /// otherwise <c>false</c>.
        /// </returns>
        public async Task<bool> DeleteCommentAsync(Guid commentId)
        {
            var loadedComment = await _context.Comments.FindAsync(commentId);
            if (loadedComment is null) { return false; }
            _context.Comments.Remove(loadedComment);
            return true;
        }

        /// <summary>
        /// Asynchronously retrieves a comment by its ID for read-only purposes.
        /// </summary>
        /// <param name="commentId">The unique identifier of the comment.</param>
        /// <returns>
        /// A <see cref="Task"/> resulting in the <see cref="Comment"/> if found,
        /// otherwise <c>null</c>.
        /// </returns>
        /// <remarks>
        /// This method uses <c>AsNoTracking()</c> for better performance on read operations.
        /// </remarks>
        public async Task<Comment?> GetCommentByIdAsync(Guid commentId)
        {
            var comment = await _context.Comments
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CommentId == commentId);

            return comment;
        }

        /// <summary>
        /// Asynchronously retrieves a comment by its ID, tracked by the context for updates.
        /// </summary>
        /// <param name="commentId">The unique identifier of the comment.</param>
        /// <returns>
        /// A <see cref="Task"/> resulting in the tracked <see cref="Comment"/> if found,
        /// otherwise <c>null</c>.
        /// </returns>
        public async Task<Comment?> GetCommentForUpdateAsync(Guid commentId)
        {
            return await _context.Comments.FirstOrDefaultAsync(c => c.CommentId == commentId);
        }

        /// <summary>
        /// Marks an existing <see cref="Comment"/> entity as modified in the context.
        /// </summary>
        /// <param name="comment">The comment entity with updated values.</param>
        /// <returns>A <see cref="Task"/> resulting in <c>true</c>.</returns>
        /// <remarks>
        /// This method does not save changes to the database.
        /// <see cref="SaveChangeAsync"/> must be called separately.
        /// </remarks>
        public Task<bool> UpdateComment(Comment comment)
        {
            _context.Comments.Update(comment);
            return Task.FromResult(true);
        }

        /// <summary>
        /// Asynchronously saves all changes made in this context to the database.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> that represents the asynchronous save operation.
        /// </returns>
        public Task SaveChangeAsync()
        {
            return _context.SaveChangesAsync();
        }

        /// <summary>
        /// Asynchronously retrieves all comments from the database for read-only purposes.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> resulting in a <see cref="List{Comment}"/> of all comments.
        /// </returns>
        /// <remarks>
        /// This method uses <c>AsNoTracking()</c> for better performance.
        /// </Hremarks>
        public async Task<List<Comment>> GetAllCommentsAsync()
        {
            var comments = await _context.Comments
                .AsNoTracking()
                .ToListAsync();

            return comments;
        }
    }
}