using MediatR;
using PatientService.Application.Models.FlaggingSetDto;
using PatientService.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PatientService.Application.UseCases.FlaggingSet.Command.UpdateFlagging
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;PatientService.Application.UseCases.FlaggingSet.Command.UpdateFlagging.UpdateFlaggingSetCommand, PatientService.Application.Models.FlaggingSetDto.FlaggingSetConfigDto&gt;" />
    public class UpdateFlaggingSetHandler : IRequestHandler<UpdateFlaggingSetCommand, FlaggingSetConfigDto>
    {
        /// <summary>
        /// The flagging set repository
        /// </summary>
        private readonly IFlaggingSetRepository _flaggingSetRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateFlaggingSetHandler"/> class.
        /// </summary>
        /// <param name="flaggingSetRepository">The flagging set repository.</param>
        public UpdateFlaggingSetHandler(IFlaggingSetRepository flaggingSetRepository)
        {
            _flaggingSetRepository = flaggingSetRepository;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<FlaggingSetConfigDto> Handle(UpdateFlaggingSetCommand request, CancellationToken cancellationToken)
        {
            var existingConfig = await _flaggingSetRepository.GetByIdAsync(request.ConfigId, cancellationToken);

            if (existingConfig == null)
            {
                return null; // Return null to indicate not found
            }

            // Update properties
            existingConfig.TestName = request.TestName;
            existingConfig.LowThreshold = request.LowThreshold;
            existingConfig.HighThreshold = request.HighThreshold;
            existingConfig.CriticalThreshold = request.CriticalThreshold;
            existingConfig.Version = request.Version;
            existingConfig.UpdatedAt = DateTime.UtcNow;

            _flaggingSetRepository.Update(existingConfig);
            // AuditBehavior will save changes

            return new FlaggingSetConfigDto
            {
                ConfigId = existingConfig.ConfigId,
                TestName = existingConfig.TestName,
                LowThreshold = existingConfig.LowThreshold,
                HighThreshold = existingConfig.HighThreshold,
                CriticalThreshold = existingConfig.CriticalThreshold,
                Version = existingConfig.Version,
                UpdatedAt = existingConfig.UpdatedAt
            };
        }
    }
}
