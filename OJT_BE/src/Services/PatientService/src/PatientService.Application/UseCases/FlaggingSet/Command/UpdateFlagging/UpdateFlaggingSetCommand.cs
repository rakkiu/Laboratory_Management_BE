using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MediatR;
using PatientService.Application.Interfaces;
using PatientService.Application.Models.FlaggingSetDto;

namespace PatientService.Application.UseCases.FlaggingSet.Command.UpdateFlagging
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;PatientService.Application.Models.FlaggingSetDto.FlaggingSetConfigDto&gt;" />
    /// <seealso cref="PatientService.Application.Interfaces.IAuditableCommand" />
    public class UpdateFlaggingSetCommand : IRequest<FlaggingSetConfigDto>, IAuditableCommand
    {
        /// <summary>
        /// Gets or sets the configuration identifier.
        /// </summary>
        /// <value>
        /// The configuration identifier.
        /// </value>
        [JsonIgnore] // Bỏ qua thuộc tính này khi đọc JSON từ body
        public int ConfigId { get; set; }
        /// <summary>
        /// Gets or sets the name of the test.
        /// </summary>
        /// <value>
        /// The name of the test.
        /// </value>
        public string TestName { get; set; }
        /// <summary>
        /// Gets or sets the low threshold.
        /// </summary>
        /// <value>
        /// The low threshold.
        /// </value>
        public float? LowThreshold { get; set; }
        /// <summary>
        /// Gets or sets the high threshold.
        /// </summary>
        /// <value>
        /// The high threshold.
        /// </value>
        public float? HighThreshold { get; set; }
        /// <summary>
        /// Gets or sets the critical threshold.
        /// </summary>
        /// <value>
        /// The critical threshold.
        /// </value>
        public float? CriticalThreshold { get; set; }
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public string Version { get; set; } = "1.0";

        /// <summary>
        /// User who performed the action
        /// </summary>
        [JsonIgnore]
        public Guid PerformedBy { get; set; }

        /// <summary>
        /// Action type for audit log (CREATE, UPDATE, DELETE)
        /// </summary>
        /// <returns></returns>
        public string GetAuditAction() => "UPDATE";

        /// <summary>
        /// Get the entity ID that is being audited
        /// </summary>
        /// <returns></returns>
        public Guid? GetEntityId() => null; // Since ConfigId is int, we might need to adjust IAuditableCommand or how it's used. For now, returning null as a placeholder if the audit system relies on Guid. If your entity ID is not a Guid, you might need a more generic approach.
    }
}
