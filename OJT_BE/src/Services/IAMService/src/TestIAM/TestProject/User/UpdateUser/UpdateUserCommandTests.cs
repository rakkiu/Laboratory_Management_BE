using System;
using IAMService.Application.UseCases.Users.Commands;
using NUnit.Framework;

namespace IAMService.Tests.Application.Users
{
    [TestFixture]
    public class UpdateUserCommandTests
    {
        [Test]
        public void Should_CreateCommand_With_CorrectValues()
        {
            var userId = Guid.NewGuid();
            var command = new UpdateUserCommand(
                userId,
                "John Doe",
                new DateTime(2000, 1, 1),
                25,
                "male",
                "Hanoi",
                "john.doe@example.com",
                "0912345678"
            );

            Assert.That(command.UserId, Is.EqualTo(userId));
            Assert.That(command.FullName, Is.EqualTo("John Doe"));
            Assert.That(command.Email, Is.EqualTo("john.doe@example.com"));
        }
    }
}
