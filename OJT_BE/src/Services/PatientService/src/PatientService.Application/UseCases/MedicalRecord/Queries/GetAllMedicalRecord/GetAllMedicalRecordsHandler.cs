using PatientService.Application.Models.PatientDto;
using PatientService.Application.Models.PatientMedicalRecordDto;
using PatientService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Application.UseCases.MedicalRecord.Queries.GetAllMedicalRecord
{
    /// <summary>
    /// get all medical records handler
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;PatientService.Application.UseCases.PatientMedicalRecord.Queries.GetAllMedicalRecord.GetAllMedicalRecordsQuery, System.Collections.Generic.List&lt;PatientService.Application.Models.PatientMedicalRecordDto.MedicalRecordDto&gt;&gt;" />
    public class GetAllMedicalRecordsHandler : IRequestHandler<GetAllMedicalRecordsQuery, List<ListPatientDto?>>
    {
        /// <summary>
        /// The patient medical record repository
        /// </summary>
        private readonly IPatientRepository _patientRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllMedicalRecordsHandler"/> class.
        /// </summary>
        /// <param name="patientMedicalRecordRepository">The patient medical record repository.</param>
        public GetAllMedicalRecordsHandler(IPatientRepository patientRepository)
        {
           _patientRepository = patientRepository;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<List<ListPatientDto?>> Handle(GetAllMedicalRecordsQuery request, CancellationToken cancellationToken)
        {
            var records = (await _patientRepository.GetAllPatientsAsync())
            .Where(p => p.MedicalRecords != null && p.MedicalRecords.Any(mr => mr.IsDeleted == false))
            .ToList();


            foreach (var record in records)
            {
                if (record.MedicalRecords != null)
                {
                    record.MedicalRecords = record.MedicalRecords.Where(f => f.IsDeleted == false).ToList();
                }
            }

            List<ListPatientDto> result = records.Select(records => new ListPatientDto
            {
                PatientId = records.PatientId,
                FullName = records.FullName,
                DateOfBirth = records.DateOfBirth,
                LastTestDate = records.LastTestDate,
                PhoneNumber = records.PhoneNumber
            }).ToList();
            return result;
        }
    }
}
