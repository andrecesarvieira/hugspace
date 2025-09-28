using MediatR;
using SynQcore.Application.Features.Employees.DTOs;

namespace SynQcore.Application.Features.Employees.Queries;

/// <summary>
/// Query para buscar funcionarios por termo de pesquisa.
/// Busca em nome, email e outras informações relevantes.
/// </summary>
/// <param name="SearchTerm">Termo de busca para filtrar funcionarios.</param>
public record SearchEmployeesQuery(string SearchTerm) : IRequest<List<EmployeeDto>>;
