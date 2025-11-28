using MediatR;
using PatientService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Application.UseCases.MedicalRecord.Commands.DeletePatient
{
    public class DeletePatientCommand : IRequest<bool>, IAuditableCommand
    {
        public Guid PatientId { get; set; }
        public Guid DeletedBy { get; set; }

        // Implementation for IAuditableCommand
        public Guid PerformedBy => DeletedBy;

        public string GetAuditAction() => "DELETE";

        public Guid? GetEntityId() => PatientId;
    }
}
