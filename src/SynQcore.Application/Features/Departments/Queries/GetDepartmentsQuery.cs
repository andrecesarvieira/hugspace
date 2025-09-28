using MediatR;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Features.Departments.DTOs;

namespace SynQcore.Application.Features.Departments.Queries;

public record GetDepartmentsQuery(GetDepartmentsRequest Request) : IRequest<PagedResult<DTOs.DepartmentDto>>;
