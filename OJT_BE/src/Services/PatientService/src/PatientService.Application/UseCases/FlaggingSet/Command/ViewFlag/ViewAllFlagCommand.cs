using MediatR;
using PatientService.Application.Models.FlaggingSetDto;
using System.Collections.Generic;

namespace PatientService.Application.UseCases.FlaggingSet.Command.ViewFlag
{
    /// <summary>
    /// Command to retrieve all flagging set configurations.
    /// </summary>
    public class ViewAllFlagCommand : IRequest<IEnumerable<FlaggingSetConfigDto>>
    {
    }
}