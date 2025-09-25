using AutoMapper;
using SynQcore.Application.Features.Departments.DTOs;
using SynQcore.Domain.Entities.Organization;
using SynQcore.Domain.Entities.Relationships;

namespace SynQcore.Application.Features.Departments.Mappings;

public class DepartmentProfile : Profile
{
    public DepartmentProfile()
    {
        // Department -> DepartmentDto
        CreateMap<Department, DepartmentDto>()
            .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.ParentDepartmentId))
            .ForMember(dest => dest.ParentName, opt => opt.MapFrom(src => src.ParentDepartment != null ? src.ParentDepartment.Name : null))
            .ForMember(dest => dest.ChildrenCount, opt => opt.MapFrom(src => src.SubDepartments != null ? src.SubDepartments.Count(c => !c.IsDeleted) : 0))
            .ForMember(dest => dest.EmployeesCount, opt => opt.MapFrom(src => src.Employees != null ? src.Employees.Count(ed => !ed.IsDeleted) : 0))
            .ForMember(dest => dest.Employees, opt => opt.MapFrom(src => src.Employees != null ? src.Employees.Where(ed => !ed.IsDeleted).Select(ed => ed.Employee) : new List<Employee>()));

        // Department -> DepartmentHierarchyDto
        CreateMap<Department, DepartmentHierarchyDto>()
            .ForMember(dest => dest.DirectEmployeesCount, opt => opt.MapFrom(src => src.Employees != null ? src.Employees.Count(ed => !ed.IsDeleted) : 0))
            .ForMember(dest => dest.DirectEmployees, opt => opt.MapFrom(src => src.Employees != null ? src.Employees.Where(ed => !ed.IsDeleted).Select(ed => ed.Employee) : new List<Employee>()))
            .ForMember(dest => dest.Children, opt => opt.Ignore()) // Será preenchido manualmente nos handlers
            .ForMember(dest => dest.Parent, opt => opt.Ignore()) // Será preenchido manualmente nos handlers
            .ForMember(dest => dest.Level, opt => opt.Ignore()) // Será calculado manualmente
            .ForMember(dest => dest.HierarchyPath, opt => opt.Ignore()) // Será calculado manualmente
            .ForMember(dest => dest.TotalEmployeesInHierarchy, opt => opt.Ignore()); // Será calculado manualmente

        // Employee -> EmployeeSummaryDto
        CreateMap<Employee, EmployeeSummaryDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.ProfilePhotoUrl))
            .ForMember(dest => dest.HireDate, opt => opt.MapFrom(src => src.HireDate))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));
    }
}