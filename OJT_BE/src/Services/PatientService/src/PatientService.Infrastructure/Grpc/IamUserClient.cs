using Grpc.Core;
using PatientService.Application.Interfaces;
using Shared.GrpcContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Infrastructure.Grpc
{
    public class IamUserClient : IIamUserClient
    {
        private readonly UserService.UserServiceClient _client;

        public IamUserClient(UserService.UserServiceClient client)
        {
            _client = client;
        }

        public async Task<string?> GetUserFullNameAsync(Guid id, CancellationToken ct = default)
        {
            var reply = await _client.GetUserByIdAsync(
                new GetUserByIdRequest { UserId = id.ToString() },
                cancellationToken: ct);

            if (!reply.Found) return null;

            return reply.FullName;
        }
    }
}
