using MediatR;
using Microsoft.AspNetCore.Http;

namespace SynQcore.Application.Features.Employees.Commands;

public record UploadEmployeeAvatarCommand(Guid EmployeeId, IFormFile Avatar) : IRequest<string>;