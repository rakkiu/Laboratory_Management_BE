using MediatR;
using PatientService.Application.Models.FlaggingSetDto;
using PatientService.Domain.Entities.TestOrder;
using PatientService.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PatientService.Application.UseCases.FlaggingSet.Command.CreateFlagging
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;PatientService.Application.UseCases.FlaggingSet.Command.CreateFlaggingSetCommand, PatientService.Application.Models.FlaggingSetDto.FlaggingSetConfigDto&gt;" />
    public class CreateFlaggingSetHandler : IRequestHandler<CreateFlaggingSetCommand, FlaggingSetConfigDto>
    {
        /// <summary>
        /// The flagging set repository
        /// </summary>
        private readonly IFlaggingSetRepository _flaggingSetRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateFlaggingSetHandler"/> class.
        /// </summary>
        /// <param name="flaggingSetRepository">The flagging set repository.</param>
        public CreateFlaggingSetHandler(IFlaggingSetRepository flaggingSetRepository)
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
        public async Task<FlaggingSetConfigDto> Handle(CreateFlaggingSetCommand request, CancellationToken cancellationToken)
        {
            var newConfig = new FlaggingSetConfig
            {
                TestName = request.TestName,
                LowThreshold = request.LowThreshold,
                HighThreshold = request.HighThreshold,
                CriticalThreshold = request.CriticalThreshold,
                Version = request.Version,
                UpdatedAt = DateTime.UtcNow
            };

            await _flaggingSetRepository.AddAsync(newConfig, cancellationToken);
            // AuditBehavior will save changes

            return new FlaggingSetConfigDto
            {
                ConfigId = newConfig.ConfigId,
                TestName = newConfig.TestName,
                LowThreshold = newConfig.LowThreshold,
                HighThreshold = newConfig.HighThreshold,
                CriticalThreshold = newConfig.CriticalThreshold,
                Version = newConfig.Version,
                UpdatedAt = newConfig.UpdatedAt
            };
        }
    }
}
