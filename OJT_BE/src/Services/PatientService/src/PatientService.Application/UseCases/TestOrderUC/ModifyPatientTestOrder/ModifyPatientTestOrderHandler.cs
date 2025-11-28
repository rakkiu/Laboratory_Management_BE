using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using PatientService.Application.Interfaces;
using PatientService.Domain.Interfaces.TestOrderService;

namespace PatientService.Application.UseCases.TestOrderUC.Commands.ModifyPatientTestOrder
{
    /// <summary>
    /// Handler for modifying patient's test order
    /// </summary>
    public class ModifyPatientTestOrderHandler : IRequestHandler<ModifyPatientTestOrderCommand, bool>
    {
        private readonly ITestOrderRepository _testOrderRepository;
        private readonly IApplicationDbContext _dbContext;
        private readonly ILogger<ModifyPatientTestOrderHandler> _logger;

        private static readonly string[] AcceptableDobFormats = { "MM/dd/yyyy" };

        public ModifyPatientTestOrderHandler(
            ITestOrderRepository testOrderRepository,
            IApplicationDbContext dbContext,
            ILogger<ModifyPatientTestOrderHandler> logger)
        {
            _testOrderRepository = testOrderRepository;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<bool> Handle(ModifyPatientTestOrderCommand request, CancellationToken cancellationToken)
        {
            // 1. Get existing test order
            var testOrder = await _dbContext.Set<Domain.Entities.TestOrder.TestOrder>()
                .FindAsync(new object[] { request.TestOrderId }, cancellationToken);

            if (testOrder == null)
            {
                throw new KeyNotFoundException($"Test order with ID {request.TestOrderId} not found");
            }

            if (testOrder.IsDeleted)
            {
                throw new InvalidOperationException($"Cannot modify a deleted test order");
            }

            // 2. Parse date
            if (!DateTime.TryParseExact(
                request.DateOfBirth,
                AcceptableDobFormats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var dob))
            {
                throw new ArgumentException("Invalid date of birth. Accepted format: MM/dd/yyyy.");
            }

            // 3. ✅ Direct comparison without decryption
            bool hasChanges = false;

            if (!string.Equals(testOrder.PatientName, request.PatientName, StringComparison.Ordinal))
            {
                testOrder.PatientName = request.PatientName;
                hasChanges = true;
            }

            if (testOrder.DateOfBirth != dob.ToUniversalTime())
            {
                testOrder.DateOfBirth = dob.ToUniversalTime();
                hasChanges = true;
            }

            if (testOrder.Age != request.Age)
            {
                testOrder.Age = request.Age;
                hasChanges = true;
            }

            if (!string.Equals(testOrder.Gender, request.Gender, StringComparison.Ordinal))
            {
                testOrder.Gender = request.Gender;
                hasChanges = true;
            }

            if (!string.Equals(testOrder.Address, request.Address, StringComparison.Ordinal))
            {
                testOrder.Address = request.Address;
                hasChanges = true;
            }

            if (!string.Equals(testOrder.PhoneNumber, request.PhoneNumber, StringComparison.Ordinal))
            {
                testOrder.PhoneNumber = request.PhoneNumber;
                hasChanges = true;
            }

            // 4. ✅ If no changes, return early
            if (!hasChanges)
            {
                _logger.LogInformation("No changes detected for TestOrder {TestOrderId}, skipping update", request.TestOrderId);
                return true;
            }

            // 5. Update (only when there are changes)
            await _testOrderRepository.UpdateAsync(testOrder);

            return true;
        }
    }
}
