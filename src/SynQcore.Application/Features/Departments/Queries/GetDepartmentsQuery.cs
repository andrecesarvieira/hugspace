using MediatR;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Features.Departments.DTOs;

namespace SynQcore.Application.Features.Departments.Queries;

/// <summary>
/// Query para obter lista paginada de departamentos com filtros.
/// Suporta busca por nome, código e outras propriedades.
/// </summary>
/// <param name="Request">Parâmetros de busca e paginação para departamentos.</param>
public record GetDepartmentsQuery(GetDepartmentsRequest Request) : IRequest<PagedResult<DTOs.DepartmentDto>>;
