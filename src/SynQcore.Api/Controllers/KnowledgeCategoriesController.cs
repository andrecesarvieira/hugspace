using MediatR;
using Microsoft.AspNetCore.Mvc;
using SynQcore.Application.Features.KnowledgeManagement.Commands;
using SynQcore.Application.Features.KnowledgeManagement.DTOs;
using SynQcore.Application.Features.KnowledgeManagement.Queries;

namespace SynQcore.Api.Controllers;

/// <summary>
/// Controller para gerenciamento de categorias de conhecimento
/// Fornece endpoints para organização hierárquica de conteúdo corporativo
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
/// <summary>
/// Classe para operações do sistema
/// </summary>
public class KnowledgeCategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Construtor da classe
    /// </summary>
    public KnowledgeCategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Buscar todas as categorias de conhecimento com opções de filtro
    /// </summary>
    /// <param name="includeInactive">Incluir categorias inativas</param>
    /// <param name="includeHierarchy">Incluir estrutura hierárquica</param>
    /// <returns>Lista de categorias com estrutura opcional de hierarquia</returns>
    [HttpGet]
    public async Task<ActionResult<List<KnowledgeCategoryDto>>> GetCategories(
        [FromQuery] bool includeInactive = false,
        [FromQuery] bool includeHierarchy = true)
    {
        var query = new GetKnowledgeCategoriesQuery
        {
            IncludeInactive = includeInactive,
            IncludeHierarchy = includeHierarchy
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Obter categoria de conhecimento específica por ID
    /// </summary>
    /// <param name="id">ID da categoria</param>
    /// <returns>Dados completos da categoria incluindo hierarquia</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<KnowledgeCategoryDto>> GetCategory(Guid id)
    {
        var query = new GetKnowledgeCategoryByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Criar nova categoria de conhecimento
    /// </summary>
    /// <param name="createDto">Dados da nova categoria incluindo hierarquia</param>
    /// <returns>Categoria criada com ID gerado</returns>
    [HttpPost]
    public async Task<ActionResult<KnowledgeCategoryDto>> CreateCategory(CreateKnowledgeCategoryDto createDto)
    {
        var command = new CreateKnowledgeCategoryCommand { Data = createDto };
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetCategory), new { id = result.Id }, result);
    }

    /// <summary>
    /// Atualizar categoria de conhecimento existente
    /// </summary>
    /// <param name="id">ID da categoria a atualizar</param>
    /// <param name="updateDto">Novos dados da categoria</param>
    /// <returns>Categoria atualizada</returns>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<KnowledgeCategoryDto>> UpdateCategory(Guid id, UpdateKnowledgeCategoryDto updateDto)
    {
        var command = new UpdateKnowledgeCategoryCommand { Id = id, Data = updateDto };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Excluir categoria de conhecimento (soft delete)
    /// </summary>
    /// <param name="id">ID da categoria a excluir</param>
    /// <returns>Confirmação da exclusão</returns>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteCategory(Guid id)
    {
        var command = new DeleteKnowledgeCategoryCommand { Id = id };
        await _mediator.Send(command);
        return NoContent();
    }
}
