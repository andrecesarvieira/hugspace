using AutoFixture;
using FluentAssertions;
using SynQcore.Domain.Entities.Organization;
using Xunit;

namespace SynQcore.UnitTests.Domain.Entities;

/// <summary>
/// Testes unitários para a entidade Employee
/// Valida regras de negócio, validações e comportamentos da entidade
/// </summary>
public class EmployeeTests
{
    private readonly Fixture _fixture;

    public EmployeeTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void EmployeeWhenCreatedShouldHaveValidDefaultValues()
    {
        // Arrange & Act
        var employee = new Employee();

        // Assert
        employee.Id.Should().NotBeEmpty();
        employee.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        employee.IsActive.Should().BeTrue();
        employee.IsDeleted.Should().BeFalse();
        employee.EmployeeId.Should().BeEmpty();
        employee.Email.Should().BeEmpty();
        employee.FirstName.Should().BeEmpty();
        employee.LastName.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void EmployeeWhenCreatedWithInvalidFirstNameShouldStoreValue(string invalidFirstName)
    {
        // Arrange & Act
        var employee = new Employee { FirstName = invalidFirstName };

        // Assert
        employee.FirstName.Should().Be(invalidFirstName);
    }

    [Fact]
    public void EmployeeWhenCreatedWithValidDataShouldHaveCorrectProperties()
    {
        // Arrange
        var firstName = "João";
        var lastName = "Silva";
        var email = "joao.silva@synqcore.com";
        var position = "Desenvolvedor";
        var jobTitle = "Desenvolvedor Senior";

        // Act
        var employee = new Employee
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Position = position,
            JobTitle = jobTitle
        };

        // Assert
        employee.FirstName.Should().Be(firstName);
        employee.LastName.Should().Be(lastName);
        employee.Email.Should().Be(email);
        employee.Position.Should().Be(position);
        employee.JobTitle.Should().Be(jobTitle);
        employee.FullName.Should().Be($"{firstName}{lastName}");
        employee.DisplayName.Should().Be($"{firstName}{lastName}");
    }

    [Fact]
    public void EmployeeFullNameShouldConcatenateFirstAndLastName()
    {
        // Arrange
        var employee = new Employee
        {
            FirstName = "Ana",
            LastName = "Costa"
        };

        // Act & Assert
        employee.FullName.Should().Be("AnaCosta");
    }

    [Fact]
    public void EmployeeDisplayNameShouldReturnEmailWhenNamesAreEmpty()
    {
        // Arrange
        var email = "test@synqcore.com";
        var employee = new Employee
        {
            Email = email,
            FirstName = "",
            LastName = ""
        };

        // Act & Assert
        employee.DisplayName.Should().Be(email);
    }

    [Fact]
    public void EmployeeYearsOfServiceShouldCalculateCorrectly()
    {
        // Arrange
        var hireDate = DateTime.UtcNow.AddYears(-3);
        var employee = new Employee { HireDate = hireDate };

        // Act & Assert
        employee.YearsOfService.Should().Be(3);
    }

    [Fact]
    public void EmployeeWhenSoftDeletedShouldMarkAsDeleted()
    {
        // Arrange
        var employee = new Employee();
        employee.IsDeleted.Should().BeFalse();

        // Act
        employee.MarkAsDeleted();

        // Assert
        employee.IsDeleted.Should().BeTrue();
        employee.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void EmployeeWhenRestoredShouldClearDeletionFlags()
    {
        // Arrange
        var employee = new Employee();
        employee.MarkAsDeleted();

        // Act
        employee.RestoreFromDeletion();

        // Assert
        employee.IsDeleted.Should().BeFalse();
        employee.DeletedAt.Should().BeNull();
    }

    [Fact]
    public void EmployeeWhenTimestampUpdatedShouldSetUpdatedAt()
    {
        // Arrange
        var employee = new Employee();
        var originalUpdatedAt = employee.UpdatedAt;

        // Wait a bit to ensure different timestamp
        Thread.Sleep(10);

        // Act
        employee.UpdateTimestamp();

        // Assert
        employee.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void EmployeeWhenActiveShouldDefaultToTrue()
    {
        // Arrange & Act
        var employee = new Employee();

        // Assert
        employee.IsActive.Should().BeTrue();
    }

    [Fact]
    public void EmployeeCanBeSetToInactive()
    {
        // Arrange
        var employee = new Employee();

        // Act
        employee.IsActive = false;

        // Assert
        employee.IsActive.Should().BeFalse();
    }

    [Fact]
    public void EmployeeCanHaveManager()
    {
        // Arrange
        var managerId = Guid.NewGuid();
        var employee = new Employee();

        // Act
        employee.ManagerId = managerId;

        // Assert
        employee.ManagerId.Should().Be(managerId);
    }

    [Fact]
    public void EmployeeShouldInitializeCollections()
    {
        // Arrange & Act
        var employee = new Employee();

        // Assert
        employee.Subordinates.Should().NotBeNull().And.BeEmpty();
        employee.EmployeeDepartments.Should().NotBeNull().And.BeEmpty();
        employee.TeamMemberships.Should().NotBeNull().And.BeEmpty();
        employee.Posts.Should().NotBeNull().And.BeEmpty();
        employee.Comments.Should().NotBeNull().And.BeEmpty();
    }
}
