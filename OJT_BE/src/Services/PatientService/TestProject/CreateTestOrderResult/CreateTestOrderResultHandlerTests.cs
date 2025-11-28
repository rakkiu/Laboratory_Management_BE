using NUnit.Framework;
using Moq;
using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

// Alias tránh trùng namespace TestOrder
using DomainTestOrder = PatientService.Domain.Entities.TestOrder.TestOrder;
using DomainTestResult = PatientService.Domain.Entities.TestOrder.TestResult;
using DomainTestResultDetail = PatientService.Domain.Entities.TestOrder.TestResultDetail;
using DomainFlagSet = PatientService.Domain.Entities.TestOrder.FlaggingSetConfig;
using DomainPatient = PatientService.Domain.Entities.Patient.Patient;

using PatientService.Application.UseCases.TestOrderUC.TestOrderResult.Command.CreateTestOrderResult;
using PatientService.Domain.Interfaces;
using PatientService.Domain.Interfaces.TestOrderService;

namespace TestProject.CreateTestOrderResult
{
    [TestFixture]
    public class CreateTestOrderResultHandlerTests
    {
        private Mock<ITestOrderRepository> _orderRepoMock;
        private Mock<ITestResultRepository> _resultRepoMock;
        private Mock<ITestResultDetailRepository> _detailRepoMock;
        private Mock<IFlaggingSetRepository> _flagRepoMock;
        private Mock<IPatientRepository> _patientRepoMock;
        private CreateTestOrderResultHandler _handler;

        [SetUp]
        public void Setup()
        {
            _orderRepoMock = new Mock<ITestOrderRepository>();
            _resultRepoMock = new Mock<ITestResultRepository>();
            _detailRepoMock = new Mock<ITestResultDetailRepository>();
            _flagRepoMock = new Mock<IFlaggingSetRepository>();
            _patientRepoMock = new Mock<IPatientRepository>();

            _handler = new CreateTestOrderResultHandler(
                _resultRepoMock.Object,
                _flagRepoMock.Object,
                _orderRepoMock.Object,
                _detailRepoMock.Object,
                _patientRepoMock.Object
            );
        }

        private DomainTestOrder FakeOrder(Guid id, Guid patientId)
        {
            return new DomainTestOrder
            {
                TestOrderId = id,
                PatientId = patientId,
                Gender = "male",
                Status = "Pending"
            };
        }

        private DomainPatient FakePatient(Guid id)
        {
            return new DomainPatient
            {
                PatientId = id,
                FullName = "Test Patient"
            };
        }

        // =====================================================================
        // 1) HAPPY PATH
        // =====================================================================
        [Test]
        public async Task Handle_Should_Create_8_Details_And_Complete_Order()
        {
            var orderId = Guid.NewGuid();
            var patientId = Guid.NewGuid();

            var order = FakeOrder(orderId, patientId);
            var patient = FakePatient(patientId);

            _orderRepoMock
                .Setup(r => r.GetByIdWithResultsAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            _patientRepoMock
                .Setup(r => r.GetPatientByIdAsync(patientId))
                .ReturnsAsync(patient);

            _flagRepoMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DomainFlagSet>());

            DomainTestResult? capturedResult = null;

            _resultRepoMock
                .Setup(r => r.AddTestResultAsync(It.IsAny<DomainTestResult>()))
                .Callback<DomainTestResult>(tr => capturedResult = tr)
                .Returns(Task.CompletedTask);

            var details = new List<DomainTestResultDetail>();

            _detailRepoMock
                .Setup(r => r.AddTestResultDetailAsync(It.IsAny<DomainTestResultDetail>()))
                .Callback<DomainTestResultDetail>(d => details.Add(d))
                .Returns(Task.CompletedTask);

            var cmd = new CreateTestOrderResultCommand
            {
                TestOrderId = orderId,
                PatientId = patientId,
                EnteredBy = "Tester"
            };

            await _handler.Handle(cmd, CancellationToken.None);

            capturedResult.Should().NotBeNull();
            details.Should().HaveCount(8);

            order.Status.Should().Be("Complete");
            order.RunBy.Should().Be("Tester");
        }

        // =====================================================================
        // 2) ORDER NOT FOUND
        // =====================================================================
        [Test]
        public async Task Handle_Should_Throw_When_Order_Not_Found()
        {
            _orderRepoMock
                .Setup(r => r.GetByIdWithResultsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((DomainTestOrder?)null);

            var cmd = new CreateTestOrderResultCommand
            {
                TestOrderId = Guid.NewGuid(),
                PatientId = Guid.NewGuid()
            };

            Func<Task> act = async () => await _handler.Handle(cmd, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Test order not found");
        }

        // =====================================================================
        // 3) PATIENT NOT FOUND
        // =====================================================================
        [Test]
        public async Task Handle_Should_Throw_When_Patient_Not_Found()
        {
            var orderId = Guid.NewGuid();
            var order = FakeOrder(orderId, Guid.NewGuid());

            _orderRepoMock
                .Setup(r => r.GetByIdWithResultsAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            _patientRepoMock
                .Setup(r => r.GetPatientByIdAsync(order.PatientId))
                .ReturnsAsync((DomainPatient?)null);

            var cmd = new CreateTestOrderResultCommand
            {
                TestOrderId = orderId,
                PatientId = order.PatientId
            };

            Func<Task> act = async () => await _handler.Handle(cmd, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Patient not found");
        }

        // =====================================================================
        // 4) ALWAYS CREATE 8 DETAILS
        // =====================================================================
        [Test]
        public async Task Handle_Should_Always_Create_8_Details()
        {
            var orderId = Guid.NewGuid();
            var patientId = Guid.NewGuid();

            var order = FakeOrder(orderId, patientId);
            var patient = FakePatient(patientId);

            _orderRepoMock
                .Setup(r => r.GetByIdWithResultsAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            _patientRepoMock
                .Setup(r => r.GetPatientByIdAsync(patientId))
                .ReturnsAsync(patient);

            _flagRepoMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DomainFlagSet>());

            var details = new List<DomainTestResultDetail>();

            _detailRepoMock
                .Setup(r => r.AddTestResultDetailAsync(It.IsAny<DomainTestResultDetail>()))
                .Callback<DomainTestResultDetail>(d => details.Add(d))
                .Returns(Task.CompletedTask);

            var cmd = new CreateTestOrderResultCommand
            {
                TestOrderId = orderId,
                PatientId = patientId
            };

            await _handler.Handle(cmd, CancellationToken.None);

            details.Should().HaveCount(8);
        }
    }
}
