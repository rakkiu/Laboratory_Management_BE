using MediatR;
using PatientService.Application.Models.FlaggingSetDto;
using PatientService.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PatientService.Application.UseCases.FlaggingSet.Command.ViewFlag
{
    /// <summary>
    /// Handles the retrieval of all flagging set configurations.
    /// </summary>
    public class ViewAllFlagHandler : IRequestHandler<ViewAllFlagCommand, IEnumerable<FlaggingSetConfigDto>>
    {
        private readonly IFlaggingSetRepository _flaggingSetRepository;

        public ViewAllFlagHandler(IFlaggingSetRepository flaggingSetRepository)
        {
            _flaggingSetRepository = flaggingSetRepository;
        }

        public async Task<IEnumerable<FlaggingSetConfigDto>> Handle(ViewAllFlagCommand request, CancellationToken cancellationToken)
        {
            var configs = await _flaggingSetRepository.GetAllAsync(cancellationToken);

            // Map entities to DTOs
            return configs.Select(c => new FlaggingSetConfigDto
            {
                ConfigId = c.ConfigId,
                TestName = c.TestName,
                LowThreshold = c.LowThreshold,
                HighThreshold = c.HighThreshold,
                CriticalThreshold = c.CriticalThreshold,
                UpdatedAt = c.UpdatedAt,
                Version = c.Version
            }).ToList();
        }
    }
}