//using PatientService.Domain.Entities.TestOrder;
//using PatientService.Domain.Interfaces.TestOrderService;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace PatientService.Application.UseCases.TestOrderUC.TRSyncUp.Command.CreateTRSyncUp
//{
//    /// <summary>
//    /// Handler for creating a new test result
//    /// </summary>
//    public class CreateTRSyncUptHandler : IRequestHandler<CreateTRSyncUpCommand, Guid>
//    {
//        private readonly ITestResultRepository _testResultRepository;

//        /// <summary>
//        /// Initializes a new instance of the <see cref="CreateTestResultHandler"/> class.
//        /// </summary>
//        /// <param name="testResultRepository">The test result repository</param>
//        public CreateTRSyncUptHandler(ITestResultRepository testResultRepository)
//        {
//            _testResultRepository = testResultRepository;
//        }

//        /// <summary>
//        /// Handles the create test result command
//        /// </summary>
//        /// <param name="request">The command request</param>
//        /// <param name="cancellationToken">Cancellation token</param>
//        /// <returns>The created test result ID</returns>
//        /// <exception cref="KeyNotFoundException">Thrown when test order is not found</exception>
//        public async Task<Guid> Handle(CreateTRSyncUpCommand request, CancellationToken cancellationToken)
//        {
//            // ✅ Validate TestOrder exists
//            var testOrderExists = await _testResultRepository.TestOrderExistsAsync(request.TestOrderId);
//            if (!testOrderExists)
//            {
//                throw new KeyNotFoundException($"Test order with ID '{request.TestOrderId}' not found or has been deleted.");
//            }

//            // ✅ Create new TestResult entity
//            var testResult = new TestResult
//            {
//                ResultId = Guid.NewGuid(),
//                TestOrderId = request.TestOrderId,
//                TestName = request.TestName,
//                Value = request.Value,
//                ReferenceRange = request.ReferenceRange,
//                Interpretation = request.Interpretation,
//                InstrumentUsed = request.InstrumentUsed,
//                Flag = request.Flag,
//                CreatedAt = DateTime.UtcNow
//            };

//            // ✅ Add to repository
//            await _testResultRepository.AddTestResultAsync(testResult);
//            await _testResultRepository.SaveChangesAsync();

//            return testResult.ResultId;
//        }
//    }

//}
