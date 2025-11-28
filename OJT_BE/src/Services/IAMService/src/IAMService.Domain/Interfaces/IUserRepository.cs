namespace IAMService.Domain.Interfaces
{
    /// <summary>
    /// Defines repository operations for User entity.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Gets the list user.
        /// </summary>
        Task<IEnumerable<User>> GetListUser();

        /// <summary>
        /// Gets the by email asynchronous.
        /// </summary>
        Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);

        /// <summary>
        /// Gets the by identifier asynchronous.
        /// </summary>
        Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);

        /// <summary>
        /// Gets the by identifier with role asynchronous.
        /// </summary>
        Task<User?> GetByIdWithRoleAsync(Guid id, CancellationToken ct = default);

        /// <summary>
        /// Adds the asynchronous.
        /// </summary>
        Task AddAsync(User user, CancellationToken ct = default);

        /// <summary>
        /// Saves the changes asynchronous.
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken ct = default);

        /// <summary>
        /// Marks an existing user entity as modified.
        /// (Required for Soft Delete)
        /// </summary>
        void Update(User user);

        /// <summary>
        /// Marks an existing user entity for removal.
        /// (Required for Hard Delete)
        /// </summary>
        void Remove(User user);

        /// <summary>
        /// Gets users by role code.
        /// </summary>
        Task<List<User>> GetUsersByRoleCodeAsync(string roleCode, CancellationToken cancellationToken);

        /// <summary>
        /// Updates user (version 1 logic).
        /// </summary>
        void UpdateV1(User user);

        /// <summary>
        /// Gets the by identifier without decrypt asynchronous.
        /// </summary>
        Task<User?> GetByIdWithoutDecryptAsync(Guid id, CancellationToken ct = default);

        /// <summary>
        /// Gets the by email without decrypt asynchronous.
        /// </summary>
        Task<User?> GetByEmailWithoutDecryptAsync(string email, CancellationToken ct = default);

        /// <summary>
        /// Updates only password field without re-encrypting other fields.
        /// </summary>
        void UpdatePasswordOnly(User user);
    }
}
