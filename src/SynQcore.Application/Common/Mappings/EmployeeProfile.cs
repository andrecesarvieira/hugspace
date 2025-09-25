using AutoMapper;
using SynQcore.Application.Features.Employees.DTOs;
using SynQcore.Domain.Entities.Organization;

namespace SynQcore.Application.Common.Mappings;

public class EmployeeProfile : Profile
{
    public EmployeeProfile()
    {
        CreateMap<Employee, EmployeeDto>()
            .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => 
                src.Manager != null ? $"{src.Manager.FirstName} {src.Manager.LastName}" : null))
            .ForMember(dest => dest.Departments, opt => opt.MapFrom(src => 
                src.EmployeeDepartments.Where(ed => !ed.IsDeleted).Select(ed => ed.Department)))
            .ForMember(dest => dest.Teams, opt => opt.MapFrom(src => 
                src.TeamMemberships.Where(tm => !tm.IsDeleted).Select(tm => new TeamDto
                {
                    Id = tm.Team.Id,
                    Name = tm.Team.Name,
                    Description = tm.Team.Description,
                    Role = tm.Role.ToString()
                })));

        CreateMap<Department, EmployeeDepartmentDto>();
    }
}