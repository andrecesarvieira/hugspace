using AutoMapper;
using SynQcore.Application.Features.Collaboration.DTOs;
using SynQcore.Domain.Entities.Communication;
using SynQcore.Domain.Entities.Organization;

namespace SynQcore.Application.Features.Collaboration.Mappings;

/// <summary>
/// Profile do AutoMapper para mapeamento de entidades de endorsement corporativo
/// </summary>
public class EndorsementMappingProfile : Profile
{
    public EndorsementMappingProfile()
    {
        // Mapping principal Endorsement -> EndorsementDto
        CreateMap<Endorsement, EndorsementDto>()
            .ForMember(dest => dest.EndorserName, opt => opt.MapFrom(src => src.Endorser.FullName))
            .ForMember(dest => dest.EndorserEmail, opt => opt.MapFrom(src => src.Endorser.Email))
            .ForMember(dest => dest.EndorserDepartment, opt => opt.MapFrom(src => 
                src.Endorser.EmployeeDepartments.FirstOrDefault() != null ? 
                src.Endorser.EmployeeDepartments.First().Department.Name : null))
            .ForMember(dest => dest.EndorserPosition, opt => opt.MapFrom(src => 
                src.Endorser.Position))
            .ForMember(dest => dest.PostTitle, opt => opt.MapFrom(src => 
                src.Post != null ? src.Post.Title : null))
            .ForMember(dest => dest.CommentContent, opt => opt.MapFrom(src => 
                src.Comment != null ? 
                    (src.Comment.Content.Length > 100 ? 
                     src.Comment.Content.Substring(0, 100) + "..." : 
                     src.Comment.Content) : null))
            .ForMember(dest => dest.TypeDisplayName, opt => opt.Ignore()) // Será preenchido pelo Helper
            .ForMember(dest => dest.TypeIcon, opt => opt.Ignore()); // Será preenchido pelo Helper

        // Mapping CreateEndorsementDto -> Endorsement (não usado diretamente, mas útil para testes)
        CreateMap<CreateEndorsementDto, Endorsement>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.EndorserId, opt => opt.Ignore())
            .ForMember(dest => dest.Endorser, opt => opt.Ignore())
            .ForMember(dest => dest.Post, opt => opt.Ignore())
            .ForMember(dest => dest.Comment, opt => opt.Ignore())
            .ForMember(dest => dest.EndorsedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        // Mapping Employee -> EmployeeEndorsementRankingDto (para rankings)
        CreateMap<Employee, EmployeeEndorsementRankingDto>()
            .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Department, opt => opt.MapFrom(src => 
                src.EmployeeDepartments.FirstOrDefault() != null ? 
                src.EmployeeDepartments.First().Department.Name : null))
            .ForMember(dest => dest.Position, opt => opt.MapFrom(src => 
                src.Position))
            .ForMember(dest => dest.TotalEndorsementsReceived, opt => opt.Ignore()) // Calculado no handler
            .ForMember(dest => dest.TotalEndorsementsGiven, opt => opt.Ignore()) // Calculado no handler
            .ForMember(dest => dest.HelpfulReceived, opt => opt.Ignore()) // Calculado no handler
            .ForMember(dest => dest.InsightfulReceived, opt => opt.Ignore()) // Calculado no handler
            .ForMember(dest => dest.AccurateReceived, opt => opt.Ignore()) // Calculado no handler
            .ForMember(dest => dest.InnovativeReceived, opt => opt.Ignore()) // Calculado no handler
            .ForMember(dest => dest.EngagementScore, opt => opt.Ignore()) // Calculado no handler
            .ForMember(dest => dest.Ranking, opt => opt.Ignore()); // Calculado no handler
    }
}