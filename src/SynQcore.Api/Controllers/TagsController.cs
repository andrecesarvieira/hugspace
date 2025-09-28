using Microsoft.AspNetCore.Mvc;
using MediatR;
using SynQcore.Application.Features.KnowledgeManagement.Commands;
using SynQcore.Application.Features.KnowledgeManagement.Queries;
using SynQcore.Application.Features.KnowledgeManagement.DTOs;
using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Api.Controllers;

/// <summary>
/// Controller para gerenciamento de tags
/// Fornece endpoints para busca, criação e manutenção de tags corporativas
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
    /// <summary>
    /// Classe para operações do sistema
    /// </summary>
public class TagsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Construtor da classe
    /// </summary>
    public TagsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Buscar tags com filtros avançados e ordenação personalizada
    /// </summary>
    /// <param name="type">Tipo de tag a filtrar</param>
    /// <param name="searchTerm">Termo para busca textual</param>
    /// <param name="minUsageCount">Número mínimo de usos da tag</param>
    /// <param name="sortBy">Campo para ordenação (padrão: Name)</param>
    /// <param name="sortDescending">Ordenação decrescente</param>
    /// <returns>Lista de tags encontradas</returns>
    [HttpGet]
    public async Task<ActionResult<List<TagDto>>> GetTags(
        [FromQuery] TagType? type = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int? minUsageCount = null,
        [FromQuery] string sortBy = "Name",
        [FromQuery] bool sortDescending = false)
    {
        var query = new GetTagsQuery
        {
            Type = type,
            SearchTerm = searchTerm,
            MinUsageCount = minUsageCount,
            SortBy = sortBy,
            SortDescending = sortDescending
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Obter tags mais populares por número de usos
    /// </summary>
    /// <param name="count">Número de tags a retornar (padrão: 20)</param>
    /// <param name="type">Tipo de tag a filtrar</param>
    /// <returns>Lista das tags mais populares</returns>
    [HttpGet("popular")]
    public async Task<ActionResult<List<TagDto>>> GetPopularTags(
        [FromQuery] int count = 20,
        [FromQuery] TagType? type = null)
    {
        var query = new GetPopularTagsQuery
        {
            Count = count,
            Type = type
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Obter tag específica por ID com informações de uso
    /// </summary>
    /// <param name="id">ID da tag</param>
    /// <returns>Dados completos da tag</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TagDto>> GetTag(Guid id)
    {
        var query = new GetTagByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Criar nova tag (skill, knowledge, etc.)
    /// </summary>
    /// <param name="createDto">Dados para criação da tag</param>
    /// <returns>Tag criada com ID gerado</returns>
    [HttpPost]
    public async Task<ActionResult<TagDto>> CreateTag(CreateTagDto createDto)
    {
        var command = new CreateTagCommand { Data = createDto };
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetTag), new { id = result.Id }, result);
    }

    /// <summary>
    /// Atualizar informações da tag existente
    /// </summary>
    /// <param name="id">ID da tag a atualizar</param>
    /// <param name="updateDto">Novos dados da tag</param>
    /// <returns>Tag atualizada</returns>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TagDto>> UpdateTag(Guid id, UpdateTagDto updateDto)
    {
        var command = new UpdateTagCommand { Id = id, Data = updateDto };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Excluir tag (soft delete) com validação de dependências
    /// </summary>
    /// <param name="id">ID da tag a excluir</param>
    /// <returns>Confirmação da exclusão</returns>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteTag(Guid id)
    {
        var command = new DeleteTagCommand { Id = id };
        await _mediator.Send(command);
        return NoContent();
    }
}