using System.Threading;
using System.Threading.Tasks;

namespace PatientService.Domain.Interfaces
{
    /// <summary>
    /// Defines the contract for a Unit of Work, which manages transactions
    //  and coordinates writing changes to the database.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Asynchronously saves all changes made in this unit of work to the database.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that represents the asynchronous save operation.
        /// The task result contains the number of state entries written to the database.
        /// </Treturns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}