/*
 * SynQcore - Corporate Social Network
 *
 * Collaboration Controller - Sistema de endorsements corporativos
 *
 * Author: André César Vieira <andrecesarvieira@hotmail.com>
 */

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Features.Collaboration.Commands;
using SynQcore.Application.Features.Collaboration.DTOs;
using SynQcore.Application.Features.Collaboration.Queries;

namespace SynQcore.Api.Controllers;

/// <summary>
/// Controller para gerenciamento de colaboração e endorsements corporativos
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CollaborationController : ControllerBase
{
    private readonly IMediator _mediator;

    public CollaborationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obter endorsements com filtros e paginação
    /// </summary>
    /// <param name="searchRequest">Filtros de busca</param>
    /// <returns>Lista paginada de endorsements</returns>
    /// <response code="200">Lista de endorsements retornada com sucesso</response>
    /// <response code="401">Usuário não autenticado</response>
    [HttpPost("search")]
    [ProducesResponseType(typeof(PagedResult<EndorsementDto>), 200)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<PagedResult<EndorsementDto>>> SearchEndorsements([FromBody] EndorsementSearchDto searchRequest)
    {
        var query = new GetEndorsementsQuery { SearchRequest = searchRequest };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Obter endorsement específico por ID
    /// </summary>
    /// <param name="id">ID do endorsement</param>
    /// <returns>Detalhes do endorsement</returns>
    /// <response code="200">Endorsement encontrado</response>
    /// <response code="404">Endorsement não encontrado</response>
    /// <response code="401">Usuário não autenticado</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EndorsementDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<EndorsementDto>> GetEndorsementById(Guid id)
    {
        var query = new GetEndorsementByIdQuery { Id = id };
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound($"Endorsement com ID {id} não encontrado");

        return Ok(result);
    }

    /// <summary>
    /// Obter endorsements de um post específico
    /// </summary>
    /// <param name="postId">ID do post</param>
    /// <param name="filterByType">Tipo de endorsement para filtrar (opcional)</param>
    /// <param name="includePrivate">Incluir endorsements privados</param>
    /// <returns>Lista de endorsements do post</returns>
    /// <response code="200">Lista de endorsements retornada</response>
    /// <response code="401">Usuário não autenticado</response>
    [HttpGet("post/{postId:guid}")]
    [ProducesResponseType(typeof(List<EndorsementDto>), 200)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<List<EndorsementDto>>> GetPostEndorsements(
        Guid postId,
        [FromQuery] string? filterByType = null,
        [FromQuery] bool includePrivate = false)
    {
        var query = new GetPostEndorsementsQuery
        {
            PostId = postId,
            IncludePrivate = includePrivate
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Obter estatísticas de endorsements de um conteúdo
    /// </summary>
    /// <param name="postId">ID do post (opcional)</param>
    /// <param name="commentId">ID do comentário (opcional)</param>
    /// <param name="includePrivate">Incluir endorsements privados</param>
    /// <returns>Estatísticas de endorsements</returns>
    /// <response code="200">Estatísticas retornadas com sucesso</response>
    /// <response code="400">Parâmetros inválidos</response>
    /// <response code="401">Usuário não autenticado</response>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(EndorsementStatsDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<EndorsementStatsDto>> GetEndorsementStats(
        [FromQuery] Guid? postId = null,
        [FromQuery] Guid? commentId = null,
        [FromQuery] bool includePrivate = false)
    {
        if (postId == null && commentId == null)
            return BadRequest("Deve informar pelo menos postId ou commentId");

        var query = new GetEndorsementStatsQuery
        {
            PostId = postId,
            CommentId = commentId,
            IncludePrivate = includePrivate
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Obter analytics de endorsements corporativos
    /// </summary>
    /// <param name="startDate">Data inicial do período</param>
    /// <param name="endDate">Data final do período</param>
    /// <param name="departmentId">ID do departamento para filtrar</param>
    /// <param name="includePrivate">Incluir endorsements privados</param>
    /// <returns>Analytics completo de endorsements</returns>
    /// <response code="200">Analytics retornado com sucesso</response>
    /// <response code="401">Usuário não autenticado</response>
    [HttpGet("analytics")]
    [ProducesResponseType(typeof(EndorsementAnalyticsDto), 200)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<EndorsementAnalyticsDto>> GetEndorsementAnalytics(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] Guid? departmentId = null,
        [FromQuery] bool includePrivate = false)
    {
        var query = new GetEndorsementAnalyticsQuery
        {
            StartDate = startDate,
            EndDate = endDate,
            DepartmentId = departmentId,
            IncludePrivate = includePrivate
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Criar um novo endorsement
    /// </summary>
    /// <param name="command">Dados do endorsement a ser criado</param>
    /// <returns>Endorsement criado</returns>
    /// <response code="201">Endorsement criado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="401">Usuário não autenticado</response>
    [HttpPost]
    [ProducesResponseType(typeof(EndorsementDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<EndorsementDto>> CreateEndorsement([FromBody] CreateEndorsementCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetEndorsementById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Atualizar um endorsement existente
    /// </summary>
    /// <param name="id">ID do endorsement</param>
    /// <param name="command">Dados atualizados do endorsement</param>
    /// <returns>Endorsement atualizado</returns>
    /// <response code="200">Endorsement atualizado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Endorsement não encontrado</response>
    /// <response code="401">Usuário não autenticado</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(EndorsementDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<EndorsementDto>> UpdateEndorsement(Guid id, [FromBody] UpdateEndorsementCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID na URL não corresponde ao ID no comando");

        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Excluir um endorsement
    /// </summary>
    /// <param name="id">ID do endorsement</param>
    /// <param name="command">Dados para exclusão</param>
    /// <returns>Confirmação de exclusão</returns>
    /// <response code="204">Endorsement excluído com sucesso</response>
    /// <response code="404">Endorsement não encontrado</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="403">Sem permissão para excluir</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult> DeleteEndorsement(Guid id, [FromBody] DeleteEndorsementCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID na URL não corresponde ao ID no comando");

        try
        {
            await _mediator.Send(command);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Toggle (criar/remover) endorsement em um conteúdo
    /// </summary>
    /// <param name="command">Dados do toggle</param>
    /// <returns>Endorsement resultante ou null se removido</returns>
    /// <response code="200">Toggle realizado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="401">Usuário não autenticado</response>
    [HttpPost("toggle")]
    [ProducesResponseType(typeof(EndorsementDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<EndorsementDto?>> ToggleEndorsement([FromBody] ToggleEndorsementCommand command)
    {
        if (command.PostId == null && command.CommentId == null)
            return BadRequest("Deve informar pelo menos postId ou commentId");

        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Verificar se usuário já endossou um conteúdo
    /// </summary>
    /// <param name="postId">ID do post (opcional)</param>
    /// <param name="commentId">ID do comentário (opcional)</param>
    /// <param name="userId">ID do usuário</param>
    /// <returns>Endorsement do usuário se existir</returns>
    /// <response code="200">Status retornado com sucesso</response>
    /// <response code="400">Parâmetros inválidos</response>
    /// <response code="401">Usuário não autenticado</response>
    [HttpGet("check")]
    [ProducesResponseType(typeof(EndorsementDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<EndorsementDto?>> CheckUserEndorsement(
        [FromQuery] Guid? postId = null,
        [FromQuery] Guid? commentId = null,
        [FromQuery] Guid? userId = null)
    {
        if (postId == null && commentId == null)
            return BadRequest("Deve informar pelo menos postId ou commentId");

        var query = new CheckUserEndorsementQuery
        {
            PostId = postId,
            CommentId = commentId,
            UserId = userId ?? Guid.Empty // Em implementação real, pegar do token JWT
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
