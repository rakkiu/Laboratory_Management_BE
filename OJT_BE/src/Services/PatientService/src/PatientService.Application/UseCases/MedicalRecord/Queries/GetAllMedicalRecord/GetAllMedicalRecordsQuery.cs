using PatientService.Application.Models.PatientDto;
using PatientService.Application.Models.PatientMedicalRecordDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Application.UseCases.MedicalRecord.Queries.GetAllMedicalRecord
{
    /// <summary>
    /// get all medical records query
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;System.Collections.Generic.List&lt;PatientService.Application.Models.PatientMedicalRecordDto.MedicalRecordDto&gt;&gt;" />
    /// <seealso cref="MediatR.IBaseRequest" />
    /// <seealso cref="System.IEquatable&lt;PatientService.Application.UseCases.PatientMedicalRecord.Queries.GetAllMedicalRecord.GetAllMedicalRecordsQuery&gt;" />
    public record GetAllMedicalRecordsQuery : IRequest<List<ListPatientDto?>>;
}
