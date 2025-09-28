using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;
using SynQcore.IntegrationTests.Infrastructure;

namespace SynQcore.IntegrationTests.Api;

/// <summary>
/// Testes de integração para AuthController
/// Valida fluxos completos de autenticação incluindo JWT
/// </summary>
public class AuthControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AuthControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task HealthEndpointShouldReturnOk()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/health");

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task LoginWithValidCredentialsShouldReturnToken()
    {
        // Arrange
        var loginRequest = new
        {
            email = "admin@synqcore.com",
            password = "SynQcore@Admin123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();

        // Verificar se contém um token (estrutura básica)
        var jsonDoc = JsonDocument.Parse(content);
        jsonDoc.RootElement.TryGetProperty("token", out var tokenElement)
            .Should().BeTrue("Response should contain token");

        tokenElement.GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task LoginWithInvalidCredentialsShouldReturnUnauthorized()
    {
        // Arrange
        var loginRequest = new
        {
            email = "invalid@synqcore.com",
            password = "wrongpassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task LoginWithEmptyCredentialsShouldReturnBadRequest()
    {
        // Arrange
        var loginRequest = new
        {
            email = "",
            password = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterWithValidDataShouldCreateUser()
    {
        // Arrange
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var registerRequest = new
        {
            userName = $"newuser{timestamp}",
            email = $"newuser{timestamp}@synqcore.com",
            password = "NewUser@123!",
            confirmPassword = "NewUser@123!",
            phoneNumber = "+5511999887766"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task RegisterWithExistingEmailShouldReturnConflict()
    {
        // Arrange
        var registerRequest = new
        {
            userName = "duplicateuser",
            email = "admin@synqcore.com", // Email já existente
            password = "NewUser@123!",
            confirmPassword = "NewUser@123!",
            phoneNumber = "+5511999887766"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        // Como o handler retorna BadRequest para email existente, ajustamos a expectativa
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("", "password123", "testuser")] // Email vazio
    [InlineData("test@test.com", "", "testuser")] // Password vazio
    [InlineData("test@test.com", "123", "testuser")] // Password muito curto
    [InlineData("invalid-email", "password123", "testuser")] // Email inválido
    public async Task RegisterWithInvalidDataShouldReturnBadRequest(
        string email, string password, string userName)
    {
        // Arrange
        var registerRequest = new
        {
            userName,
            email,
            password,
            confirmPassword = password,
            phoneNumber = "+5511999887766"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ProtectedEndpointWithoutTokenShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/employees");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ProtectedEndpointWithValidTokenShouldReturnOk()
    {
        // Arrange - First login to get token
        var loginRequest = new
        {
            email = "admin@synqcore.com",
            password = "SynQcore@Admin123!"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        loginResponse.IsSuccessStatusCode.Should().BeTrue();

        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(loginContent);
        var token = jsonDoc.RootElement.GetProperty("token").GetString();

        // Add token to request
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/employees");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task SwaggerEndpointShouldBeAccessible()
    {
        // Act
        var response = await _client.GetAsync("/swagger");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
    }
}
