using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PatientService.Application.Interfaces;
using PatientService.Application.Models.Audit;
using PatientService.Domain.Entities.Patient;

namespace PatientService.Application.Behaviors
{
    /// <summary>
    /// MediatR Pipeline Behavior for automatic audit logging
    /// Runs around handlers: Pre -&gt; Handler -&gt; Post (with audit)
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <seealso cref="MediatR.IPipelineBehavior&lt;TRequest, TResponse&gt;" />
    public class AuditBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        /// <summary>
        /// The database context
        /// </summary>
        private readonly IApplicationDbContext _dbContext;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<AuditBehavior<TRequest, TResponse>> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditBehavior{TRequest, TResponse}"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="logger">The logger.</param>
        public AuditBehavior(
            IApplicationDbContext dbContext,
            ILogger<AuditBehavior<TRequest, TResponse>> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// Pipeline handler. Perform any additional behavior and await the <paramref name="next" /> delegate as necessary
        /// </summary>
        /// <param name="request">Incoming request</param>
        /// <param name="next">Awaitable delegate for the next action in the pipeline. Eventually this delegate represents the handler.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Awaitable task returning the <typeparamref name="TResponse" />
        /// </returns>
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            // Check if this command needs auditing
            if (request is not IAuditableCommand auditableCommand)
            {
                // Not auditable, just pass through
                return await next();
            }

            _logger.LogInformation("Starting audit behavior for {CommandType}", typeof(TRequest).Name);

            // ?? PRE-PROCESSING: Capture original values (for UPDATE)
            var auditEntries = await BeforeHandlerExecution();

            // ?? EXECUTE HANDLER (business logic)
            // Handler MUST NOT call SaveChangesAsync()
            TResponse response;
            try
            {
                response = await next();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing handler for {CommandType}", typeof(TRequest).Name);
                throw;
            }

            // ?? POST-PROCESSING: Capture changes and save with audit
            await AfterHandlerExecution(auditEntries, auditableCommand);

            _logger.LogInformation("Completed audit behavior for {CommandType}", typeof(TRequest).Name);

            return response;
        }

        /// <summary>
        /// Before handler execution: Capture entities that are being tracked
        /// </summary>
        /// <returns></returns>
        private async Task<List<AuditEntry>> BeforeHandlerExecution()
        {
            var auditEntries = new List<AuditEntry>();

            // Detect changes before handler runs (for UPDATE scenarios)
            _dbContext.ChangeTracker.DetectChanges();

            foreach (var entry in _dbContext.ChangeTracker.Entries<PatientMedicalRecord>())
            {
                // Skip unmodified or detached entities
                if (entry.State == EntityState.Unchanged || entry.State == EntityState.Detached)
                    continue;

                var auditEntry = new AuditEntry(entry, entry.State.ToString());
                auditEntry.EntityId = entry.Entity.RecordId;

                // For Modified entities, capture OLD values
                if (entry.State == EntityState.Modified)
                {
                    foreach (var property in entry.Properties)
                    {
                        if (property.IsModified)
                        {
                            auditEntry.ChangedFields.Add(property.Metadata.Name);
                            auditEntry.OldValues[property.Metadata.Name] = property.OriginalValue;
                        }
                    }
                }

                auditEntries.Add(auditEntry);
            }

            return await Task.FromResult(auditEntries);
        }

        /// <summary>
        /// After handler execution: Save business changes + audit logs in single transaction
        /// </summary>
        /// <param name="auditEntries">The audit entries.</param>
        /// <param name="command">The command.</param>
        /// <exception cref="System.InvalidOperationException">IApplicationDbContext must be implemented by a DbContext</exception>
        private async Task AfterHandlerExecution(
            List<AuditEntry> auditEntries,
            IAuditableCommand command)
        {
            // Get the underlying DbContext for transaction support
            var dbContext = _dbContext as DbContext;
            if (dbContext == null)
            {
                throw new InvalidOperationException("IApplicationDbContext must be implemented by a DbContext");
            }

            // Start explicit transaction to ensure atomicity
            await using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                // 1?? Detect changes after handler execution
                _dbContext.ChangeTracker.DetectChanges();

                // 2?? Capture NEW audit entries and update existing ones
                foreach (var entry in _dbContext.ChangeTracker.Entries<PatientMedicalRecord>())
                {
                    if (entry.State == EntityState.Unchanged || entry.State == EntityState.Detached)
                        continue;

                    var existingAudit = auditEntries.FirstOrDefault(a => a.Entry == entry);

                    if (existingAudit != null)
                    {
                        // Update existing audit entry with NEW values
                        foreach (var property in entry.Properties)
                        {
                            if (entry.State == EntityState.Modified && property.IsModified)
                            {
                                existingAudit.NewValues[property.Metadata.Name] = property.CurrentValue;
                            }
                            else if (entry.State == EntityState.Added)
                            {
                                existingAudit.NewValues[property.Metadata.Name] = property.CurrentValue;
                            }
                        }
                    }
                    else
                    {
                        // New audit entry (not captured in pre-processing)
                        var auditEntry = new AuditEntry(entry, entry.State.ToString())
                        {
                            EntityId = entry.Entity.RecordId
                        };

                        foreach (var property in entry.Properties)
                        {
                            if (entry.State == EntityState.Added)
                            {
                                auditEntry.NewValues[property.Metadata.Name] = property.CurrentValue;
                            }
                            else if (entry.State == EntityState.Modified && property.IsModified)
                            {
                                auditEntry.ChangedFields.Add(property.Metadata.Name);
                                auditEntry.OldValues[property.Metadata.Name] = property.OriginalValue;
                                auditEntry.NewValues[property.Metadata.Name] = property.CurrentValue;
                            }
                            else if (entry.State == EntityState.Deleted)
                            {
                                auditEntry.OldValues[property.Metadata.Name] = property.OriginalValue;
                            }
                        }

                        auditEntries.Add(auditEntry);
                    }
                }

                // 3?? SAVE BUSINESS CHANGES FIRST (this commits entity changes)
                await _dbContext.SaveChangesAsync();

                // 4?? CREATE AUDIT LOG ENTRIES
                var auditLogs = auditEntries
                    .Where(a => a.EntityName == nameof(PatientMedicalRecord))
                    .Select(audit => new PatientRecordAuditLog
                    {
                        AuditId = Guid.NewGuid(),
                        RecordId = audit.EntityId,
                        Action = MapEntityStateToAction(audit.Action),
                        PerformedBy = command.PerformedBy,
                        Timestamp = DateTime.UtcNow,
                        ChangedFields = audit.ChangedFields.Count > 0 
                            ? audit.SerializeChangedFields() 
                            : null,
                        OldValues = audit.OldValues.Count > 0 
                            ? audit.SerializeOldValues() 
                            : null,
                        NewValues = audit.NewValues.Count > 0 
                            ? audit.SerializeNewValues() 
                            : null
                    })
                    .ToList();

                // 5?? ADD AUDIT LOGS TO CONTEXT
                if (auditLogs.Any())
                {
                    var auditLogDbSet = _dbContext.Set<PatientRecordAuditLog>();
                    await auditLogDbSet.AddRangeAsync(auditLogs);
                    await _dbContext.SaveChangesAsync(); // Save audit logs
                }

                // 6?? COMMIT TRANSACTION (both business + audit are saved atomically)
                await transaction.CommitAsync();

                _logger.LogInformation(
                    "Successfully saved {RecordCount} business changes and {AuditCount} audit logs",
                    auditEntries.Count, auditLogs.Count);
            }
            catch (Exception ex)
            {
                // Rollback on any error
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to save changes with audit. Transaction rolled back.");
                throw;
            }
        }

        /// <summary>
        /// Map EntityState to audit action name
        /// </summary>
        /// <param name="entityState">State of the entity.</param>
        /// <returns></returns>
        private static string MapEntityStateToAction(string entityState)
        {
            return entityState switch
            {
                "Added" => "CREATE",
                "Modified" => "UPDATE",
                "Deleted" => "DELETE",
                _ => entityState
            };
        }
    }
}
