using PatientService.Domain.Entities.TestOrder;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PatientService.Domain.Interfaces
{
    /// <summary>
    /// Defines the contract for a repository that handles <see cref="Comment"/> operations.
    /// </summary>
    public interface ICommentRepository
    {
        /// <summary>
        /// Asynchronously adds a new <see cref="Comment"/> to the database.
        /// </summary>
        /// <param name="comment">The comment entity to add.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddCommentAsync(Comment comment);

        /// <summary>
        /// Asynchronously deletes a comment from the database by its ID.
        /// </summary>
        /// <param name="commentId">The unique identifier of the comment to delete.</param>
        /// <returns>
        /// A <see cref="Task"/> resulting in <c>true</c> if the comment was found and deleted,
        /// otherwise <c>false</c>.
        /// </returns>
        Task<bool> DeleteCommentAsync(Guid commentId);

        /// <summary>
        /// Asynchronously retrieves a comment by its ID for read-only purposes.
        /// </summary>
        /// <param name="commentId">The unique identifier of the comment.</param>
        /// <returns>
        /// A <see cref="Task"/> resulting in the <see cref="Comment"/> if found,
        /// otherwise <c>null</c>.
        /// </returns>
        Task<Comment?> GetCommentByIdAsync(Guid commentId);

        /// <summary>
        /// Asynchronously retrieves a comment by its ID, tracked by the context for updates.
        /// </summary>
        /// <param name="commentId">The unique identifier of the comment.</param>
        /// <returns>
        /// A <see cref="Task"/> resulting in the tracked <see cref="Comment"/> if found,
        /// otherwise <c>null</c>.
        /// </returns>
        Task<Comment?> GetCommentForUpdateAsync(Guid commentId);

        /// <summary>
        /// Marks an existing <see cref="Comment"/> entity as modified in the context.
        /// </summary>
        /// <param name="comment">The comment entity with updated values.</param>
        /// <returns>A <see cref="Task"/> resulting in <c>true</c>.</returns>
        /// <remarks>
        /// This method does not save changes. <see cref="SaveChangeAsync"/> must be called separately.
        /// </remarks>
        Task<bool> UpdateComment(Comment comment);

        /// <summary>
        /// Asynchronously saves all changes made in this context to the database.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> that represents the asynchronous save operation.
        /// </returns>
        Task SaveChangeAsync();

        /// <summary>
        /// Asynchronously retrieves all comments from the database for read-only purposes.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> resulting in a <see cref="List{Comment}"/> of all comments.
        /// </returns>
        Task<List<Comment>> GetAllCommentsAsync();
    }
}