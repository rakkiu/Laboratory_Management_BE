using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;

namespace PatientService.Application.Models.Audit
{
    public class AuditEntry
    {
        public EntityEntry Entry { get; set; } = null!;
        public Guid EntityId { get; set; }
        public string EntityName { get; set; } = null!;
        public string Action { get; set; } = null!;
        public Dictionary<string, object?> OldValues { get; set; } = new();
        public Dictionary<string, object?> NewValues { get; set; } = new();
        public List<string> ChangedFields { get; set; } = new();

        public AuditEntry(EntityEntry entry, string action)
        {
            Entry = entry;
            Action = action;
            EntityName = entry.Entity.GetType().Name;
        }

        public string SerializeOldValues() => JsonSerializer.Serialize(OldValues);
        public string SerializeNewValues() => JsonSerializer.Serialize(NewValues);
        public string SerializeChangedFields() => JsonSerializer.Serialize(ChangedFields);
    }
}
