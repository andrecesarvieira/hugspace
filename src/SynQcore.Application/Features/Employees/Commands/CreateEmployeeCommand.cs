using MediatR;
using SynQcore.Application.Features.Employees.DTOs;

namespace SynQcore.Application.Features.Employees.Commands;

/// <summary>
/// Command para criar novo funcionario na organização.
/// Processa dados de cadastro e integra com estrutura organizacional.
/// </summary>
/// <param name="Request">Dados do funcionario a ser criado.</param>
public record CreateEmployeeCommand(CreateEmployeeRequest Request) : IRequest<EmployeeDto>;
