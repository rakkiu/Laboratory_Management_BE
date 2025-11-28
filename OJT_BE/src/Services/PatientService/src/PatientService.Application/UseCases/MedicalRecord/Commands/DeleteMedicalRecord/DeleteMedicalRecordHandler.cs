using PatientService.Domain.Interfaces;

namespace PatientService.Application.UseCases.MedicalRecord.Commands.DeleteMedicalRecord
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;PatientService.Application.UseCases.MedicalRecord.Commands.DeleteMedicalRecord.DeleteMedicalRecordCommand, System.Boolean&gt;" />
    public class DeleteMedicalRecordHandler : IRequestHandler<DeleteMedicalRecordCommand, bool>
    {
        /// <summary>
        /// The medical record repository
        /// </summary>
        private readonly IPatientMedicalRecordRepository _medicalRecordRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteMedicalRecordHandler"/> class.
        /// </summary>
        /// <param name="medicalRecordRepository">The medical record repository.</param>
        public DeleteMedicalRecordHandler(IPatientMedicalRecordRepository medicalRecordRepository)
        {
            _medicalRecordRepository = medicalRecordRepository;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<bool> Handle(DeleteMedicalRecordCommand request, CancellationToken cancellationToken)
        {
            var existingRecords = await _medicalRecordRepository.GetMedicalRecordsByPatientIdAsync(request.PatientId);

            if (existingRecords == null || !existingRecords.Any())
            {
                return false;
            }

            foreach (var record in existingRecords)
            {
                // Soft delete
                record.IsDeleted = true;
                record.UpdatedBy = request.DeletedBy;
                record.UpdatedAt = DateTime.UtcNow;

                _medicalRecordRepository.UpdateMedicalRecord(record);
            }
            
            // ? DO NOT call SaveChangesAsync() - AuditBehavior will handle it

            return true;
        }
    }
}