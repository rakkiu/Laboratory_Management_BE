using IAMService.Application.UseCases.User.Commands.CreateUser;
using NUnit.Framework;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TestProject.CreateUserTest
{
    /// <summary>
    /// Unit tests for CreateUserValidator
    /// Tests FluentValidation rules
    /// </summary>
    [TestFixture]
    public class CreateUserValidatorTests
    {
        private CreateUserValidator _validator = null!;

        [SetUp]
        public void Setup()
        {
            _validator = new CreateUserValidator();
        }

        private static CreateUserCommand MakeValidCommand()
        {
            return new CreateUserCommand
            {
                RoleCode = "LECTURER",
                Email = "test@example.com",
                PhoneNumber = "0987654321",
                FullName = "Test User",
                IdentifyNumber = "ID123456",
                Gender = "Male",
                Age = 28,
                Address = "Test Address",
                DateOfBirth = "01/31/1995"
            };
        }

        #region Valid Cases

        [Test]
        public void Validate_WithValidCommand_ShouldPass()
        {
            // Arrange
            var command = MakeValidCommand();

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.True);
            Assert.That(result.Errors, Is.Empty);
        }

        [TestCase("Male")]
        [TestCase("Female")]
        [TestCase("M")]
        [TestCase("F")]
        [TestCase("male")]
        [TestCase("female")]
        [TestCase("m")]
        [TestCase("f")]
        public void Validate_WithValidGenderVariations_ShouldPass(string gender)
        {
            // Arrange
            var command = new CreateUserCommand
            {
                RoleCode = "LECTURER",
                Email = "test@example.com",
                PhoneNumber = "0987654321",
                FullName = "Test User",
                IdentifyNumber = "ID123456",
                Gender = gender,
                Age = 28,
                Address = "Test Address",
                DateOfBirth = "01/31/1995"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [TestCase("12345678")] // Minimum 8
        [TestCase("12345678901234567890")] // Maximum 20
        [TestCase("0123456789")]
        public void Validate_WithValidPhoneNumber_ShouldPass(string phoneNumber)
        {
            // Arrange
            var command = new CreateUserCommand
            {
                RoleCode = "LECTURER",
                Email = "test@example.com",
                PhoneNumber = phoneNumber,
                FullName = "Test User",
                IdentifyNumber = "ID123456",
                Gender = "Male",
                Age = 28,
                Address = "Test Address",
                DateOfBirth = "01/31/1995"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [TestCase("123456")] // Minimum 6
        [TestCase("12345678901234567890")] // Maximum 20
        [TestCase("ID123456789")]
        public void Validate_WithValidIdentifyNumber_ShouldPass(string identifyNumber)
        {
            // Arrange
            var command = new CreateUserCommand
            {
                RoleCode = "LECTURER",
                Email = "test@example.com",
                PhoneNumber = "0987654321",
                FullName = "Test User",
                IdentifyNumber = identifyNumber,
                Gender = "Male",
                Age = 28,
                Address = "Test Address",
                DateOfBirth = "01/31/1995"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [TestCase(1)]
        [TestCase(18)]
        [TestCase(65)]
        [TestCase(120)]
        public void Validate_WithValidAge_ShouldPass(int age)
        {
            // Arrange
            var command = new CreateUserCommand
            {
                RoleCode = "LECTURER",
                Email = "test@example.com",
                PhoneNumber = "0987654321",
                FullName = "Test User",
                IdentifyNumber = "ID123456",
                Gender = "Male",
                Age = age,
                Address = "Test Address",
                DateOfBirth = "01/31/1995"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        #endregion

        #region Email Validation

        [Test]
        public void Validate_WithEmptyEmail_ShouldFail()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                RoleCode = "LECTURER",
                Email = "",
                PhoneNumber = "0987654321",
                FullName = "Test User",
                IdentifyNumber = "ID123456",
                Gender = "Male",
                Age = 28,
                Address = "Test Address",
                DateOfBirth = "01/31/1995"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<FluentValidation.Results.ValidationFailure>(
                e => e.PropertyName == "Email"));
        }

        [TestCase("invalid-email")]
        [TestCase("@example.com")]
        [TestCase("test@")]
        [TestCase("test")]
        public void Validate_WithInvalidEmailFormat_ShouldFail(string invalidEmail)
        {
            // Arrange
            var command = new CreateUserCommand
            {
                RoleCode = "LECTURER",
                Email = invalidEmail,
                PhoneNumber = "0987654321",
                FullName = "Test User",
                IdentifyNumber = "ID123456",
                Gender = "Male",
                Age = 28,
                Address = "Test Address",
                DateOfBirth = "01/31/1995"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<FluentValidation.Results.ValidationFailure>(
                e => e.PropertyName == "Email"));
        }

        #endregion

        #region PhoneNumber Validation

        [Test]
        public void Validate_WithEmptyPhoneNumber_ShouldFail()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                RoleCode = "LECTURER",
                Email = "test@example.com",
                PhoneNumber = "",
                FullName = "Test User",
                IdentifyNumber = "ID123456",
                Gender = "Male",
                Age = 28,
                Address = "Test Address",
                DateOfBirth = "01/31/1995"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<FluentValidation.Results.ValidationFailure>(
                e => e.PropertyName == "PhoneNumber"));
        }

        [TestCase("123")] // Too short
        [TestCase("1234567")] // Just below minimum
        [TestCase("123456789012345678901")] // Too long (21 chars)
        public void Validate_WithInvalidPhoneNumberLength_ShouldFail(string phoneNumber)
        {
            // Arrange
            var command = new CreateUserCommand
            {
                RoleCode = "LECTURER",
                Email = "test@example.com",
                PhoneNumber = phoneNumber,
                FullName = "Test User",
                IdentifyNumber = "ID123456",
                Gender = "Male",
                Age = 28,
                Address = "Test Address",
                DateOfBirth = "01/31/1995"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<FluentValidation.Results.ValidationFailure>(
                e => e.PropertyName == "PhoneNumber"));
        }

        #endregion

        #region FullName Validation

        [Test]
        public void Validate_WithEmptyFullName_ShouldFail()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                RoleCode = "LECTURER",
                Email = "test@example.com",
                PhoneNumber = "0987654321",
                FullName = "",
                IdentifyNumber = "ID123456",
                Gender = "Male",
                Age = 28,
                Address = "Test Address",
                DateOfBirth = "01/31/1995"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<FluentValidation.Results.ValidationFailure>(
                e => e.PropertyName == "FullName"));
        }

        [Test]
        public void Validate_WithTooLongFullName_ShouldFail()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                RoleCode = "LECTURER",
                Email = "test@example.com",
                PhoneNumber = "0987654321",
                FullName = new string('A', 101),
                IdentifyNumber = "ID123456",
                Gender = "Male",
                Age = 28,
                Address = "Test Address",
                DateOfBirth = "01/31/1995"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<FluentValidation.Results.ValidationFailure>(
                e => e.PropertyName == "FullName"));
        }

        #endregion

        #region IdentifyNumber Validation

        [Test]
        public void Validate_WithEmptyIdentifyNumber_ShouldFail()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                RoleCode = "LECTURER",
                Email = "test@example.com",
                PhoneNumber = "0987654321",
                FullName = "Test User",
                IdentifyNumber = "",
                Gender = "Male",
                Age = 28,
                Address = "Test Address",
                DateOfBirth = "01/31/1995"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<FluentValidation.Results.ValidationFailure>(
                e => e.PropertyName == "IdentifyNumber"));
        }

        [TestCase("12345")] // Too short (5 chars)
        [TestCase("123456789012345678901")] // Too long (21 chars)
        public void Validate_WithInvalidIdentifyNumberLength_ShouldFail(string identifyNumber)
        {
            // Arrange
            var command = new CreateUserCommand
            {
                RoleCode = "LECTURER",
                Email = "test@example.com",
                PhoneNumber = "0987654321",
                FullName = "Test User",
                IdentifyNumber = identifyNumber,
                Gender = "Male",
                Age = 28,
                Address = "Test Address",
                DateOfBirth = "01/31/1995"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.False, "Validator should fail for invalid IdentifyNumber length.");

            var identifyNumberError = result.Errors.FirstOrDefault(e => e.PropertyName == "IdentifyNumber");

            Assert.That(identifyNumberError, Is.Not.Null, "IdentifyNumber should have a validation error.");
            Assert.That(identifyNumberError.ErrorMessage, Does.Contain("6"),
                "Error message should mention '6 to 20' characters requirement.");
        }


        [Test]
        public void Validate_WithEmptyGender_ShouldFail()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                RoleCode = "LECTURER",
                Email = "test@example.com",
                PhoneNumber = "0987654321",
                FullName = "Test User",
                IdentifyNumber = "ID123456",
                Gender = "",
                Age = 28,
                Address = "Test Address",
                DateOfBirth = "01/31/1995"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<FluentValidation.Results.ValidationFailure>(
                e => e.PropertyName == "Gender"));
        }

        [TestCase("Other")]
        [TestCase("X")]
        [TestCase("Unknown")]
        [TestCase("Man")]
        [TestCase("Woman")]
        public void Validate_WithInvalidGender_ShouldFail(string invalidGender)
        {
            // Arrange
            var command = new CreateUserCommand
            {
                RoleCode = "LECTURER",
                Email = "test@example.com",
                PhoneNumber = "0987654321",
                FullName = "Test User",
                IdentifyNumber = "ID123456",
                Gender = invalidGender,
                Age = 28,
                Address = "Test Address",
                DateOfBirth = "01/31/1995"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<FluentValidation.Results.ValidationFailure>(
                e => e.PropertyName == "Gender" && e.ErrorMessage.Contains("Male, Female, M, F")));
        }

        #endregion

        #region Age Validation

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-10)]
        public void Validate_WithZeroOrNegativeAge_ShouldFail(int invalidAge)
        {
            // Arrange
            var command = new CreateUserCommand
            {
                RoleCode = "LECTURER",
                Email = "test@example.com",
                PhoneNumber = "0987654321",
                FullName = "Test User",
                IdentifyNumber = "ID123456",
                Gender = "Male",
                Age = invalidAge,
                Address = "Test Address",
                DateOfBirth = "01/31/1995"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<FluentValidation.Results.ValidationFailure>(
                e => e.PropertyName == "Age"));
        }

        [TestCase(121)]
        [TestCase(150)]
        [TestCase(200)]
        public void Validate_WithAgeTooHigh_ShouldFail(int invalidAge)
        {
            // Arrange
            var command = new CreateUserCommand
            {
                RoleCode = "LECTURER",
                Email = "test@example.com",
                PhoneNumber = "0987654321",
                FullName = "Test User",
                IdentifyNumber = "ID123456",
                Gender = "Male",
                Age = invalidAge,
                Address = "Test Address",
                DateOfBirth = "01/31/1995"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<FluentValidation.Results.ValidationFailure>(
                e => e.PropertyName == "Age"));
        }

        #endregion

        #region Address Validation

        [Test]
        public void Validate_WithEmptyAddress_ShouldFail()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                RoleCode = "LECTURER",
                Email = "test@example.com",
                PhoneNumber = "0987654321",
                FullName = "Test User",
                IdentifyNumber = "ID123456",
                Gender = "Male",
                Age = 28,
                Address = "",
                DateOfBirth = "01/31/1995"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<FluentValidation.Results.ValidationFailure>(
                e => e.PropertyName == "Address"));
        }

        #endregion

        #region DateOfBirth Validation

        [Test]
        public void Validate_WithEmptyDateOfBirth_ShouldFail()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                RoleCode = "LECTURER",
                Email = "test@example.com",
                PhoneNumber = "0987654321",
                FullName = "Test User",
                IdentifyNumber = "ID123456",
                Gender = "Male",
                Age = 28,
                Address = "Test Address",
                DateOfBirth = ""
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<FluentValidation.Results.ValidationFailure>(
                e => e.PropertyName == "DateOfBirth"));
        }

        [TestCase("1995-01-31")] // YYYY-MM-DD
        [TestCase("31/01/1995")] // DD/MM/YYYY
        [TestCase("Jan 31, 1995")]
        [TestCase("invalid-date")]
        public void Validate_WithInvalidDateFormat_ShouldFail(string invalidDate)
        {
            // Arrange
            var command = new CreateUserCommand
            {
                RoleCode = "LECTURER",
                Email = "test@example.com",
                PhoneNumber = "0987654321",
                FullName = "Test User",
                IdentifyNumber = "ID123456",
                Gender = "Male",
                Age = 28,
                Address = "Test Address",
                DateOfBirth = invalidDate
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<FluentValidation.Results.ValidationFailure>(
                e => e.PropertyName == "DateOfBirth" && e.ErrorMessage.Contains("MM/DD/YYYY")));
        }

        [TestCase("01/31/1995")]
        [TestCase("12/25/2000")]
        [TestCase("06/15/1990")]
        public void Validate_WithValidDateFormat_ShouldPass(string validDate)
        {
            // Arrange
            var command = new CreateUserCommand
            {
                RoleCode = "LECTURER",
                Email = "test@example.com",
                PhoneNumber = "0987654321",
                FullName = "Test User",
                IdentifyNumber = "ID123456",
                Gender = "Male",
                Age = 28,
                Address = "Test Address",
                DateOfBirth = validDate
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        #endregion

        #region Multiple Errors

        [Test]
        public void Validate_WithMultipleInvalidFields_ShouldReturnMultipleErrors()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                RoleCode = "LECTURER",
                Email = "invalid-email",
                PhoneNumber = "123", // Too short
                FullName = "",
                IdentifyNumber = "12", // Too short
                Gender = "Other",
                Age = 0,
                Address = "",
                DateOfBirth = "invalid"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Count, Is.GreaterThanOrEqualTo(7));
            
            var propertyNames = result.Errors.Select(e => e.PropertyName).ToList();
            Assert.That(propertyNames, Does.Contain("Email"));
            Assert.That(propertyNames, Does.Contain("PhoneNumber"));
            Assert.That(propertyNames, Does.Contain("FullName"));
            Assert.That(propertyNames, Does.Contain("IdentifyNumber"));
            Assert.That(propertyNames, Does.Contain("Gender"));
            Assert.That(propertyNames, Does.Contain("Age"));
            Assert.That(propertyNames, Does.Contain("Address"));
            Assert.That(propertyNames, Does.Contain("DateOfBirth"));
        }

        #endregion
    }
}