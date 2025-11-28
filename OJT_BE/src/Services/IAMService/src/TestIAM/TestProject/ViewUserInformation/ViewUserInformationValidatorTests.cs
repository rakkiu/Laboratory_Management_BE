using IAMService.Application.UseCases.User.Queries.ViewUserInformation;

namespace TestProject.ViewUserInformationTest
{
    /// <summary>
    /// Tests validator for <see cref="ViewUserInformationCommand"/>
    /// </summary>
    [TestFixture]
    public class ViewUserInformationValidatorTests
    {
        /// <summary>
        /// The validator
        /// </summary>
        private ViewUserInformationValidator _validator;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _validator = new ViewUserInformationValidator();
        }

        /// <summary>
        /// Validates the with valid admin role should pass.
        /// </summary>
        [Test]
        public void Validate_WithValidAdminRole_ShouldPass()
        {
            // Arrange
            var command = new ViewUserInformationCommand(Guid.NewGuid(), "ADMIN");

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.True);
            Assert.That(result.Errors, Is.Empty);
        }

        /// <summary>
        /// Validates the with valid lab manager role should pass.
        /// </summary>
        [Test]
        public void Validate_WithValidLabManagerRole_ShouldPass()
        {
            // Arrange
            var command = new ViewUserInformationCommand(Guid.NewGuid(), "LAB_MANAGER");

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.True);
            Assert.That(result.Errors, Is.Empty);
        }

        /// <summary>
        /// Validates the with empty user identifier should fail.
        /// </summary>
        [Test]
        public void Validate_WithEmptyUserId_ShouldFail()
        {
            // Arrange
            var command = new ViewUserInformationCommand(Guid.Empty, "ADMIN");

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Any(e => e.PropertyName == "UserId"), Is.True);
            Assert.That(result.Errors.Any(e => e.ErrorMessage.Contains("valid")), Is.True);
        }

        /// <summary>
        /// Validates the with empty role should fail.
        /// </summary>
        [Test]
        public void Validate_WithEmptyRole_ShouldFail()
        {
            // Arrange
            var command = new ViewUserInformationCommand(Guid.NewGuid(), "");

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Any(e => e.PropertyName == "CurrentUserRole"), Is.True);
        }

        /// <summary>
        /// Validates the with invalid role should fail.
        /// </summary>
        /// <param name="invalidRole">The invalid role.</param>
        [TestCase("LAB_USER")]
        [TestCase("GUEST")]
        [TestCase("USER")]
        [TestCase("RANDOM_ROLE")]
        public void Validate_WithInvalidRole_ShouldFail(string invalidRole)
        {
            // Arrange
            var command = new ViewUserInformationCommand(Guid.NewGuid(), invalidRole);

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Any(e => e.PropertyName == "CurrentUserRole"), Is.True);
            Assert.That(result.Errors.Any(e => e.ErrorMessage.Contains("Admin") || e.ErrorMessage.Contains("Lab Manager")), Is.True);
        }

        /// <summary>
        /// Validates the with both empty user identifier and role should fail with multiple errors.
        /// </summary>
        [Test]
        public void Validate_WithBothEmptyUserIdAndRole_ShouldFailWithMultipleErrors()
        {
            // Arrange
            var command = new ViewUserInformationCommand(Guid.Empty, "");

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Count, Is.GreaterThanOrEqualTo(2));
            Assert.That(result.Errors.Any(e => e.PropertyName == "UserId"), Is.True);
            Assert.That(result.Errors.Any(e => e.PropertyName == "CurrentUserRole"), Is.True);
        }

        /// <summary>
        /// Validates the with null role should fail.
        /// </summary>
        [Test]
        public void Validate_WithNullRole_ShouldFail()
        {
            // Arrange
            var command = new ViewUserInformationCommand(Guid.NewGuid(), null!);

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Any(e => e.PropertyName == "CurrentUserRole"), Is.True);
        }

        /// <summary>
        /// Validates the with valid roles multiple times should always pass.
        /// </summary>
        [TestCase("ADMIN")]
        [TestCase("LAB_MANAGER")]
        public void Validate_WithValidRoles_ShouldAlwaysPass(string validRole)
        {
            // Arrange
            var command = new ViewUserInformationCommand(Guid.NewGuid(), validRole);

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        /// <summary>
        /// Validates the with whitespace role should fail.
        /// </summary>
        [Test]
        public void Validate_WithWhitespaceRole_ShouldFail()
        {
            // Arrange
            var command = new ViewUserInformationCommand(Guid.NewGuid(), "   ");

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Any(e => e.PropertyName == "CurrentUserRole"), Is.True);
        }

        /// <summary>
        /// Validates the with case sensitive role should fail.
        /// </summary>
        /// <param name="caseSensitiveRole">The case sensitive role.</param>
        [TestCase("admin")]
        [TestCase("Admin")]
        [TestCase("lab_manager")]
        [TestCase("Lab_Manager")]
        public void Validate_WithCaseSensitiveRole_ShouldFail(string caseSensitiveRole)
        {
            // Arrange
            var command = new ViewUserInformationCommand(Guid.NewGuid(), caseSensitiveRole);

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Any(e => e.PropertyName == "CurrentUserRole"), Is.True);
        }

        /// <summary>
        /// Validates the error messages should be clear and descriptive.
        /// </summary>
        [Test]
        public void Validate_ErrorMessages_ShouldBeClearAndDescriptive()
        {
            // Arrange
            var command = new ViewUserInformationCommand(Guid.Empty, "LAB_USER");

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.False);
            
            var userIdError = result.Errors.FirstOrDefault(e => e.PropertyName == "UserId");
            Assert.That(userIdError, Is.Not.Null);
            Assert.That(userIdError!.ErrorMessage, Is.Not.Empty);
            
            var roleError = result.Errors.FirstOrDefault(e => e.PropertyName == "CurrentUserRole");
            Assert.That(roleError, Is.Not.Null);
            Assert.That(roleError!.ErrorMessage, Does.Contain("Admin").Or.Contains("Manager"));
        }
    }
}

