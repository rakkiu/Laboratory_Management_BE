using FluentAssertions;
using NUnit.Framework;
using PatientService.Application.UseCases.TestOrderUC.TestOrders.Commands.CreateTestOrder;
using CreatePatientDto = PatientService.Application.Models.PatientDto.CreatePatient;

namespace TestProject.CreateTestOrder;

[TestFixture]
public class CreateTestOrderValidatorTests
{
    private CreateTestOrderValidator _validator = null!;

    [SetUp]
    public void Setup()
    {
        _validator = new CreateTestOrderValidator();
    }

    [Test]
    public void Validate_ShouldPass_WhenAllRequiredFieldsAreValid()
    {
        // Arrange
        var command = new CreateTestOrderCommand
        {
            CreatedBy = "user123",
            Patient = new CreatePatientDto
            {
                FullName = "John Doe",
                DateOfBirth = "01/15/1990",
                Gender = "male",
                PhoneNumber = "0123456789",
                Email = "john@example.com",
                Address = "123 Main St",
                IdentifyNumber = "ID123456",
                UserId = Guid.NewGuid()
            }
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_ShouldFail_WhenPatientIsNull()
    {
        // Arrange
        var command = new CreateTestOrderCommand
        {
            CreatedBy = "user123",
            Patient = null!
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Patient information is required.");
    }

    [Test]
    public void Validate_ShouldFail_WhenFullNameIsEmpty()
    {
        // Arrange
        var command = new CreateTestOrderCommand
        {
            CreatedBy = "user123",
            Patient = new CreatePatientDto
            {
                FullName = "",
                DateOfBirth = "01/15/1990",
                Gender = "male",
                PhoneNumber = "0123456789"
            }
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Full name is required.");
    }

    [Test]
    public void Validate_ShouldFail_WhenFullNameExceedsMaxLength()
    {
        // Arrange
        var command = new CreateTestOrderCommand
        {
            CreatedBy = "user123",
            Patient = new CreatePatientDto
            {
                FullName = new string('A', 201), // 201 characters
                DateOfBirth = "01/15/1990",
                Gender = "male",
                PhoneNumber = "0123456789"
            }
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Patient.FullName");
    }

    [Test]
    public void Validate_ShouldFail_WhenDateOfBirthIsEmpty()
    {
        // Arrange
        var command = new CreateTestOrderCommand
        {
            CreatedBy = "user123",
            Patient = new CreatePatientDto
            {
                FullName = "John Doe",
                DateOfBirth = "",
                Gender = "male",
                PhoneNumber = "0123456789"
            }
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Patient.DateOfBirth");
    }

    [Test]
    [TestCase("1990-01-15")] // Wrong format (ISO)
    [TestCase("15/01/1990")] // Wrong format (DD/MM/YYYY)
    [TestCase("2024-12-31")] // Wrong format
    [TestCase("invalid")] // Invalid string
    [TestCase("13/32/2024")] // Invalid date
    public void Validate_ShouldFail_WhenDateOfBirthFormatIsInvalid(string invalidDate)
    {
        // Arrange
        var command = new CreateTestOrderCommand
        {
            CreatedBy = "user123",
            Patient = new CreatePatientDto
            {
                FullName = "John Doe",
                DateOfBirth = invalidDate,
                Gender = "male",
                PhoneNumber = "0123456789"
            }
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "Patient.DateOfBirth" &&
            e.ErrorMessage.Contains("Invalid date of birth"));
    }

    [Test]
    [TestCase("01/15/1990")]
    [TestCase("12/31/2024")]
    [TestCase("06/01/1985")]
    public void Validate_ShouldPass_WhenDateOfBirthFormatIsValid(string validDate)
    {
        // Arrange
        var command = new CreateTestOrderCommand
        {
            CreatedBy = "user123",
            Patient = new CreatePatientDto
            {
                FullName = "John Doe",
                DateOfBirth = validDate,
                Gender = "male",
                PhoneNumber = "0123456789"
            }
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void Validate_ShouldFail_WhenGenderIsEmpty()
    {
        // Arrange
        var command = new CreateTestOrderCommand
        {
            CreatedBy = "user123",
            Patient = new CreatePatientDto
            {
                FullName = "John Doe",
                DateOfBirth = "01/15/1990",
                Gender = "",
                PhoneNumber = "0123456789"
            }
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Gender is required.");
    }

    [Test]
    [TestCase("male")]
    [TestCase("female")]
    public void Validate_ShouldPass_WhenGenderIsValid(string gender)
    {
        // Arrange
        var command = new CreateTestOrderCommand
        {
            CreatedBy = "user123",
            Patient = new CreatePatientDto
            {
                FullName = "John Doe",
                DateOfBirth = "01/15/1990",
                Gender = gender,
                PhoneNumber = "0123456789"
            }
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Test]
    [TestCase("other")]
    [TestCase("Male")] // Case sensitive
    [TestCase("FEMALE")]
    [TestCase("unknown")]
    public void Validate_ShouldFail_WhenGenderIsInvalid(string invalidGender)
    {
        // Arrange
        var command = new CreateTestOrderCommand
        {
            CreatedBy = "user123",
            Patient = new CreatePatientDto
            {
                FullName = "John Doe",
                DateOfBirth = "01/15/1990",
                Gender = invalidGender,
                PhoneNumber = "0123456789"
            }
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "Patient.Gender" &&
            e.ErrorMessage.Contains("must be 'male', 'female'"));
    }

    [Test]
    public void Validate_ShouldFail_WhenPhoneNumberIsEmpty()
    {
        // Arrange
        var command = new CreateTestOrderCommand
        {
            CreatedBy = "user123",
            Patient = new CreatePatientDto
            {
                FullName = "John Doe",
                DateOfBirth = "01/15/1990",
                Gender = "male",
                PhoneNumber = ""
            }
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Phone number is required.");
    }

    [Test]
    [TestCase("0123456789")] // 10 digits
    [TestCase("+84123456789")] // With country code
    [TestCase("987654321")] // 9 digits
    [TestCase("123456789012345")] // 15 digits
    public void Validate_ShouldPass_WhenPhoneNumberIsValid(string phoneNumber)
    {
        // Arrange
        var command = new CreateTestOrderCommand
        {
            CreatedBy = "user123",
            Patient = new CreatePatientDto
            {
                FullName = "John Doe",
                DateOfBirth = "01/15/1990",
                Gender = "male",
                PhoneNumber = phoneNumber
            }
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Test]
    [TestCase("12345")] // Too short
    [TestCase("1234567890123456")] // Too long
    [TestCase("abcdefghij")] // Letters
    [TestCase("123-456-7890")] // Invalid format
    public void Validate_ShouldFail_WhenPhoneNumberIsInvalid(string invalidPhone)
    {
        // Arrange
        var command = new CreateTestOrderCommand
        {
            CreatedBy = "user123",
            Patient = new CreatePatientDto
            {
                FullName = "John Doe",
                DateOfBirth = "01/15/1990",
                Gender = "male",
                PhoneNumber = invalidPhone
            }
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "Patient.PhoneNumber" &&
            e.ErrorMessage.Contains("Invalid phone number"));
    }

    [Test]
    [TestCase("john@example.com")]
    [TestCase("test.user@domain.co.uk")]
    [TestCase("user+tag@example.org")]
    public void Validate_ShouldPass_WhenEmailIsValid(string email)
    {
        // Arrange
        var command = new CreateTestOrderCommand
        {
            CreatedBy = "user123",
            Patient = new CreatePatientDto
            {
                FullName = "John Doe",
                DateOfBirth = "01/15/1990",
                Gender = "male",
                PhoneNumber = "0123456789",
                Email = email
            }
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Test]
    [TestCase("invalid-email")]
    [TestCase("@example.com")]
    [TestCase("user@")]
    public void Validate_ShouldFail_WhenEmailIsInvalid(string invalidEmail)
    {
        // Arrange
        var command = new CreateTestOrderCommand
        {
            CreatedBy = "user123",
            Patient = new CreatePatientDto
            {
                FullName = "John Doe",
                DateOfBirth = "01/15/1990",
                Gender = "male",
                PhoneNumber = "0123456789",
                Email = invalidEmail
            }
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "Patient.Email" &&
            e.ErrorMessage.Contains("Invalid email"));
    }

    [Test]
    public void Validate_ShouldPass_WhenEmailIsEmpty()
    {
        // Arrange - Email is optional
        var command = new CreateTestOrderCommand
        {
            CreatedBy = "user123",
            Patient = new CreatePatientDto
            {
                FullName = "John Doe",
                DateOfBirth = "01/15/1990",
                Gender = "male",
                PhoneNumber = "0123456789",
                Email = ""
            }
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void Validate_ShouldFail_WhenEmailExceedsMaxLength()
    {
        // Arrange
        var command = new CreateTestOrderCommand
        {
            CreatedBy = "user123",
            Patient = new CreatePatientDto
            {
                FullName = "John Doe",
                DateOfBirth = "01/15/1990",
                Gender = "male",
                PhoneNumber = "0123456789",
                Email = new string('a', 190) + "@example.com" // > 200 chars
            }
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Patient.Email");
    }

    [Test]
    public void Validate_ShouldPass_WhenAddressIsValid()
    {
        // Arrange
        var command = new CreateTestOrderCommand
        {
            CreatedBy = "user123",
            Patient = new CreatePatientDto
            {
                FullName = "John Doe",
                DateOfBirth = "01/15/1990",
                Gender = "male",
                PhoneNumber = "0123456789",
                Address = "123 Main Street, City, Country"
            }
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void Validate_ShouldFail_WhenAddressExceedsMaxLength()
    {
        // Arrange
        var command = new CreateTestOrderCommand
        {
            CreatedBy = "user123",
            Patient = new CreatePatientDto
            {
                FullName = "John Doe",
                DateOfBirth = "01/15/1990",
                Gender = "male",
                PhoneNumber = "0123456789",
                Address = new string('A', 301) // > 300 chars
            }
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Patient.Address");
    }

    [Test]
    public void Validate_ShouldPass_WhenOptionalFieldsAreNull()
    {
        // Arrange
        var command = new CreateTestOrderCommand
        {
            CreatedBy = "user123",
            Patient = new CreatePatientDto
            {
                FullName = "John Doe",
                DateOfBirth = "01/15/1990",
                Gender = "male",
                PhoneNumber = "0123456789",
                Email = null,
                Address = null,
                IdentifyNumber = null,
                LastTestDate = null
            }
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void Validate_ShouldFail_WhenIdentifyNumberExceedsMaxLength()
    {
        // Arrange
        var command = new CreateTestOrderCommand
        {
            CreatedBy = "user123",
            Patient = new CreatePatientDto
            {
                FullName = "John Doe",
                DateOfBirth = "01/15/1990",
                Gender = "male",
                PhoneNumber = "0123456789",
                IdentifyNumber = new string('1', 51) // > 50 chars
            }
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Patient.IdentifyNumber");
    }

    [Test]
    public void Validate_ShouldReturnMultipleErrors_WhenMultipleFieldsAreInvalid()
    {
        // Arrange
        var command = new CreateTestOrderCommand
        {
            CreatedBy = "user123",
            Patient = new CreatePatientDto
            {
                FullName = "", // Invalid
                DateOfBirth = "invalid-date", // Invalid
                Gender = "unknown", // Invalid
                PhoneNumber = "123", // Invalid
                Email = "not-an-email" // Invalid
            }
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Count.Should().BeGreaterThan(4);
        result.Errors.Should().Contain(e => e.PropertyName == "Patient.FullName");
        result.Errors.Should().Contain(e => e.PropertyName == "Patient.DateOfBirth");
        result.Errors.Should().Contain(e => e.PropertyName == "Patient.Gender");
        result.Errors.Should().Contain(e => e.PropertyName == "Patient.PhoneNumber");
        result.Errors.Should().Contain(e => e.PropertyName == "Patient.Email");
    }
}
