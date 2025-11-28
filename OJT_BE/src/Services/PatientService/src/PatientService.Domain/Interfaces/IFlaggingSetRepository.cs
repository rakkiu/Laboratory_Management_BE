using PatientService.Domain.Entities.TestOrder;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PatientService.Domain.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFlaggingSetRepository
    {
        /// <summary>
        /// Gets the by identifier asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<FlaggingSetConfig> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets the by test name asynchronous.
        /// </summary>
        /// <param name="testName">Name of the test.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<FlaggingSetConfig> GetByTestNameAsync(string testName, CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets all asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<IEnumerable<FlaggingSetConfig>> GetAllAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Adds the asynchronous.
        /// </summary>
        /// <param name="flaggingSetConfig">The flagging set configuration.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task AddAsync(FlaggingSetConfig flaggingSetConfig, CancellationToken cancellationToken = default);
        /// <summary>
        /// Updates the specified flagging set configuration.
        /// </summary>
        /// <param name="flaggingSetConfig">The flagging set configuration.</param>
        void Update(FlaggingSetConfig flaggingSetConfig);
    }
}