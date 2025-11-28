using System;
using FluentValidation.TestHelper;
using IAMService.Application.Features.Users.Commands;
using IAMService.Application.UseCases.Users.Commands;
using NUnit.Framework;

namespace IAMService.Tests.Application.Users
{
    [TestFixture]
    public class UpdateUserCommandValidatorTests
    {
        private UpdateUserCommandValidator _validator = null!;

        [SetUp]
        public void Setup()
        {
            _validator = new UpdateUserCommandValidator();
        }

        [Test]
        public void Should_HaveError_When_FullName_IsEmpty()
        {
            var command = new UpdateUserCommand(Guid.NewGuid(), "", DateTime.UtcNow.AddYears(-20), 20, "male", "Hanoi", "test@example.com", "0912345678");
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.FullName);
        }

        [Test]
        public void Should_Pass_When_AllFields_Valid()
        {
            var command = new UpdateUserCommand(Guid.NewGuid(), "John Doe", DateTime.UtcNow.AddYears(-25), 25, "male", "Hanoi", "john.doe@example.com", "0912345678");
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
