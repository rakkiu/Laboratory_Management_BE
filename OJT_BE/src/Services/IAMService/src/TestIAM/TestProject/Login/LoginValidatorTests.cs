using IAMService.Application.UseCases.Auth.Login;

namespace TestProject.LoginTest
{
    /// <summary>
    /// tests validator for <see cref="LoginCommand"/>
    /// </summary>
    [TestFixture]
    public class LoginValidatorTests
    {
        /// <summary>
        /// The validator
        /// </summary>
        private LoginValidator _validator;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _validator = new LoginValidator();
        }

        /// <summary>
        /// Validates the with valid credentials should pass.
        /// </summary>
        [Test]
        public void Validate_WithValidCredentials_ShouldPass()
        {
            // Arrange
            var command = new LoginCommand("test@example.com", "Password123");

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        /// <summary>
        /// Validates the with empty email should fail.
        /// </summary>
        [Test]
        public void Validate_WithEmptyEmail_ShouldFail()
        {
            // Arrange
            var command = new LoginCommand("", "Password123");

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Any(e => e.PropertyName == "Email"), Is.True);
        }

        /// <summary>
        /// Validates the with invalid email format should fail.
        /// </summary>
        [Test]
        public void Validate_WithInvalidEmailFormat_ShouldFail()
        {
            // Arrange
            var command = new LoginCommand("invalid-email", "Password123");

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Any(e => e.PropertyName == "Email"), Is.True);
        }

        /// <summary>
        /// Validates the with empty password should fail.
        /// </summary>
        [Test]
        public void Validate_WithEmptyPassword_ShouldFail()
        {
            // Arrange
            var command = new LoginCommand("test@example.com", "");

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Any(e => e.PropertyName == "Password"), Is.True);
        }

        /// <summary>
        /// Validates the with short password should fail.
        /// </summary>
        [Test]
        public void Validate_WithShortPassword_ShouldFail()
        {
            // Arrange
            var command = new LoginCommand("test@example.com", "12345");

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Any(e => e.PropertyName == "Password"), Is.True);
        }

        /// <summary>
        /// Validates the with invalid password should fail.
        /// </summary>
        /// <param name="password">The password.</param>
        [TestCase("")]
        [TestCase("abc")]
        [TestCase("12345")]
        public void Validate_WithInvalidPassword_ShouldFail(string password)
        {
            // Arrange
            var command = new LoginCommand("test@example.com", password);

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        /// <summary>
        /// Validates the with valid inputs should pass.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="password">The password.</param>
        [TestCase("test@example.com", "Password123")]
        [TestCase("user@domain.co.uk", "ValidPass123")]
        [TestCase("admin@system.com", "Admin@123")]
        public void Validate_WithValidInputs_ShouldPass(string email, string password)
        {
            // Arrange
            var command = new LoginCommand(email, password);

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }
    }
}
