using MediatR;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Features.Employees.DTOs;

namespace SynQcore.Application.Features.Employees.Queries;

/// <summary>
/// Query para obter lista paginada de funcionários com filtros.
/// Processa parâmetros de busca e retorna resultados paginados.
/// </summary>
/// <param name="Request">Parâmetros de busca e filtros para funcionarios.</param>
public record GetEmployeesQuery(EmployeeSearchRequest Request) : IRequest<PagedResult<EmployeeDto>>;
