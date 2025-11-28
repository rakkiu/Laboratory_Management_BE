using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Application.Interfaces
{
    public interface IIamUserClient
    {
        Task<string?> GetUserFullNameAsync(Guid id, CancellationToken ct = default);
    }
}
