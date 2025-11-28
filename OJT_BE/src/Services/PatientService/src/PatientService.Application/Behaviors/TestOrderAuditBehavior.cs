using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PatientService.Application.Interfaces;
using PatientService.Application.Models.Audit;
using PatientService.Domain.Entities.TestOrder;
using System.Text.Json;

namespace PatientService.Application.Behaviors
{
    public class TestOrderAuditBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ILogger<TestOrderAuditBehavior<TRequest, TResponse>> _logger;

        public TestOrderAuditBehavior(
            IApplicationDbContext dbContext,
            ILogger<TestOrderAuditBehavior<TRequest, TResponse>> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (request is not IAuditableCommand auditableCommand)
            {
                return await next();
            }

            _logger.LogInformation("Starting audit behavior for {CommandType}", typeof(TRequest).Name);

            var auditEntries = await BeforeHandlerExecution();

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

            await AfterHandlerExecution(auditEntries, auditableCommand);

            _logger.LogInformation("Completed audit behavior for {CommandType}", typeof(TRequest).Name);

            return response;
        }

        private async Task<List<AuditEntry>> BeforeHandlerExecution()
        {
            var auditEntries = new List<AuditEntry>();

            _dbContext.ChangeTracker.DetectChanges();

            foreach (var entry in _dbContext.ChangeTracker.Entries<TestOrder>())
            {
                if (entry.State == EntityState.Unchanged || entry.State == EntityState.Detached)
                    continue;

                var auditEntry = new AuditEntry(entry, entry.State.ToString())
                {
                    EntityId = entry.Entity.TestOrderId
                };

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

        private async Task AfterHandlerExecution(
            List<AuditEntry> auditEntries,
            IAuditableCommand command)
        {
            var dbContext = _dbContext as DbContext;
            if (dbContext == null)
            {
                throw new InvalidOperationException("IApplicationDbContext must be implemented by a DbContext");
            }

            await using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                _dbContext.ChangeTracker.DetectChanges();

                foreach (var entry in _dbContext.ChangeTracker.Entries<TestOrder>())
                {
                    if (entry.State == EntityState.Unchanged || entry.State == EntityState.Detached)
                        continue;

                    var existingAudit = auditEntries.FirstOrDefault(a => a.Entry == entry);

                    if (existingAudit != null)
                    {
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
                        var auditEntry = new AuditEntry(entry, entry.State.ToString())
                        {
                            EntityId = entry.Entity.TestOrderId
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

                await _dbContext.SaveChangesAsync();

                var auditLogs = auditEntries
                    .Where(a => a.EntityName == nameof(TestOrder))
                    .Select(audit => new TestOrderAuditLog
                    {
                        TestOrderId = audit.EntityId,
                        UserId = command.PerformedBy.ToString(),
                        ActionType = MapEntityStateToAction(audit.Action),
                        ChangedFields = audit.ChangedFields.Count > 0
                            ? SerializeChangedFieldsWithValues(audit)
                            : null,
                        Timestamp = DateTime.UtcNow
                    })
                    .ToList();

                if (auditLogs.Any())
                {
                    var auditLogDbSet = _dbContext.Set<TestOrderAuditLog>();
                    await auditLogDbSet.AddRangeAsync(auditLogs);
                    await _dbContext.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                _logger.LogInformation(
                    "Successfully saved {RecordCount} business changes and {AuditCount} audit logs",
                    auditEntries.Count, auditLogs.Count);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to save changes with audit. Transaction rolled back.");
                throw;
            }
        }

        private static string SerializeChangedFieldsWithValues(AuditEntry audit)
        {
            var changes = new Dictionary<string, object>();

            foreach (var field in audit.ChangedFields)
            {
                changes[field] = new
                {
                    OldValue = audit.OldValues.ContainsKey(field) ? audit.OldValues[field] : null,
                    NewValue = audit.NewValues.ContainsKey(field) ? audit.NewValues[field] : null
                };
            }

            return JsonSerializer.Serialize(changes);
        }

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