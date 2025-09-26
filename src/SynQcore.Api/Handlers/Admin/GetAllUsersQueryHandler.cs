using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SynQcore.Application.DTOs.Auth;
using SynQcore.Application.Queries.Admin;
using SynQcore.Infrastructure.Identity;

namespace SynQcore.Api.Handlers.Admin;

// Handler responsável por processar consultas de listagem de usuários com paginação e filtros
public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, UsersListResponse>
{
    private readonly UserManager<ApplicationUserEntity> _userManager;

    public GetAllUsersQueryHandler(UserManager<ApplicationUserEntity> userManager)
    {
        _userManager = userManager;
    }

    // Processar consulta de usuários aplicando filtros, paginação e incluindo roles
    public async Task<UsersListResponse> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _userManager.Users.AsQueryable();

        // Aplicar filtro de busca se fornecido
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLowerInvariant();
            query = query.Where(u => 
                u.Email!.ToLowerInvariant().Contains(searchTerm) ||
                u.UserName!.ToLowerInvariant().Contains(searchTerm));
        }

        // Contar total de registros
        var totalCount = await query.CountAsync(cancellationToken);

        // Aplicar paginação
        var users = await query
            .OrderBy(u => u.Email)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        // Converter para DTOs e buscar roles
        var userDtos = new List<UserInfoDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userDtos.Add(new UserInfoDto
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEnd = user.LockoutEnd,
                AccessFailedCount = user.AccessFailedCount,
                Roles = roles.ToList()
            });
        }

        // Calcular informações de paginação
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);
        var hasPrevious = request.Page > 1;
        var hasNext = request.Page < totalPages;

        return new UsersListResponse
        {
            Users = userDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = totalPages,
            HasPrevious = hasPrevious,
            HasNext = hasNext
        };
    }
}