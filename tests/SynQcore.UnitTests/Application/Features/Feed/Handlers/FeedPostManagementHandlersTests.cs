using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Features.Feed.Commands;
using SynQcore.Application.Features.Feed.DTOs;
using SynQcore.Application.Features.Feed.Validators;
using MediatR;

namespace SynQcore.UnitTests.Application.Features.Feed.Handlers;

/// <summary>
/// Testes unitários para handlers de gerenciamento de posts do feed
/// </summary>
public class FeedPostManagementHandlersTests
{
    /// <summary>
    /// Teste básico de comando de atualização
    /// </summary>
    [Fact]
    public void UpdateFeedPostCommandDeveTemPropriedadesCorretas()
    {
        // Arrange & Act
        var command = new UpdateFeedPostCommand
        {
            PostId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Content = "Conteúdo de teste",
            Tags = new[] { "tag1", "tag2" },
            ImageUrl = "https://example.com/image.jpg",
            IsPublic = true
        };

        // Assert
        Assert.NotEqual(Guid.Empty, command.PostId);
        Assert.NotEqual(Guid.Empty, command.UserId);
        Assert.Equal("Conteúdo de teste", command.Content);
        Assert.Equal(2, command.Tags?.Length);
        Assert.Equal("https://example.com/image.jpg", command.ImageUrl);
        Assert.True(command.IsPublic);
    }

    /// <summary>
    /// Teste básico de comando de exclusão
    /// </summary>
    [Fact]
    public void DeleteFeedPostCommandDeveTemPropriedadesCorretas()
    {
        // Arrange & Act
        var command = new DeleteFeedPostCommand
        {
            PostId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        // Assert
        Assert.NotEqual(Guid.Empty, command.PostId);
        Assert.NotEqual(Guid.Empty, command.UserId);
    }

    /// <summary>
    /// Teste básico de comando de obtenção
    /// </summary>
    [Fact]
    public void GetFeedPostCommandDeveTemPropriedadesCorretas()
    {
        // Arrange & Act
        var command = new GetFeedPostCommand
        {
            PostId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        // Assert
        Assert.NotEqual(Guid.Empty, command.PostId);
        Assert.NotEqual(Guid.Empty, command.UserId);
    }

    /// <summary>
    /// Teste de DTO de resposta
    /// </summary>
    [Fact]
    public void FeedPostDtoDeveTemPropriedadesCorretas()
    {
        // Arrange & Act
        var dto = new FeedPostDto
        {
            Id = Guid.NewGuid(),
            Content = "Conteúdo de teste",
            ImageUrl = "https://example.com/image.jpg",
            Tags = new List<string> { "tag1", "tag2" },
            IsPublic = true,
            CreatedAt = DateTime.UtcNow,
            AuthorId = Guid.NewGuid(),
            AuthorName = "João Silva",
            AuthorAvatarUrl = "https://example.com/avatar.jpg",
            AuthorDepartment = "TI",
            LikeCount = 5,
            CommentCount = 3,
            ViewCount = 10,
            Status = "Published",
            Type = "FeedPost",
            Visibility = "Company"
        };

        // Assert
        Assert.NotEqual(Guid.Empty, dto.Id);
        Assert.Equal("Conteúdo de teste", dto.Content);
        Assert.Equal("https://example.com/image.jpg", dto.ImageUrl);
        Assert.Equal(2, dto.Tags.Count);
        Assert.True(dto.IsPublic);
        Assert.Equal("João Silva", dto.AuthorName);
        Assert.Equal(5, dto.LikeCount);
        Assert.Equal(3, dto.CommentCount);
        Assert.Equal(10, dto.ViewCount);
    }
}

/// <summary>
/// Testes unitários para o validador de atualização de posts
/// </summary>
public class UpdateFeedPostRequestValidatorTests
{
    private readonly UpdateFeedPostRequestValidator _validator;

    public UpdateFeedPostRequestValidatorTests()
    {
        _validator = new UpdateFeedPostRequestValidator();
    }

    [Fact]
    public void DeveValidarRequestVazioComSucesso()
    {
        // Arrange
        var request = new UpdateFeedPostRequest();

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("Conteúdo válido")]
    [InlineData("")]
    [InlineData(null)]
    public void DeveValidarConteudoValido(string? content)
    {
        // Arrange
        var request = new UpdateFeedPostRequest { Content = content };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void DeveRejeitarConteudoMuitoLongo()
    {
        // Arrange
        var longContent = new string('a', 5001);
        var request = new UpdateFeedPostRequest { Content = longContent };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("máximo 5000 caracteres", result.Errors.First().ErrorMessage);
    }

    [Theory]
    [InlineData("https://example.com/imagem.jpg")]
    [InlineData("http://test.com/foto.png")]
    [InlineData("")]
    [InlineData(null)]
    public void DeveValidarUrlImagemValida(string? imageUrl)
    {
        // Arrange
        var request = new UpdateFeedPostRequest { ImageUrl = imageUrl };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("url-invalida")]
    [InlineData("ftp://example.com/arquivo.jpg")]
    [InlineData("javascript:alert('xss')")]
    public void DeveRejeitarUrlImagemInvalida(string imageUrl)
    {
        // Arrange
        var request = new UpdateFeedPostRequest { ImageUrl = imageUrl };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("URL da imagem deve ser válida", result.Errors.First().ErrorMessage);
    }

    [Fact]
    public void DeveValidarTagsValidas()
    {
        // Arrange
        var request = new UpdateFeedPostRequest 
        { 
            Tags = new[] { "tecnologia", "desenvolvimento" } 
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void DeveRejeitarTagsInvalidas()
    {
        // Arrange
        var request = new UpdateFeedPostRequest 
        { 
            Tags = new[] { "tag com espaço", "a", "" } 
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public void DeveRejeitarMuitasTags()
    {
        // Arrange
        var manyTags = Enumerable.Range(1, 11).Select(i => $"tag{i}").ToArray();
        var request = new UpdateFeedPostRequest { Tags = manyTags };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Máximo de 10 tags", result.Errors.First().ErrorMessage);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void DeveValidarVisibilidadeValida(bool isPublic)
    {
        // Arrange
        var request = new UpdateFeedPostRequest { IsPublic = isPublic };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void DeveValidarPerformanceDoValidador()
    {
        // Arrange
        var request = new UpdateFeedPostRequest
        {
            Content = "Conteúdo de teste para performance",
            Tags = new[] { "performance", "teste", "validacao" },
            ImageUrl = "https://example.com/test.jpg",
            IsPublic = true
        };

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = _validator.Validate(request);
        stopwatch.Stop();

        // Assert
        Assert.True(result.IsValid);
        Assert.True(stopwatch.ElapsedMilliseconds < 100, 
            $"Validação levou {stopwatch.ElapsedMilliseconds}ms, esperado < 100ms");
    }
}