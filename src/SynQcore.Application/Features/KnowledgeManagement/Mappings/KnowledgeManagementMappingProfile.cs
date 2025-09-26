using AutoMapper;
using SynQcore.Application.Features.KnowledgeManagement.DTOs;
using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Application.Features.KnowledgeManagement.Mappings;

// Perfil de mapeamento AutoMapper para entidades de Knowledge Management
public class KnowledgeManagementMappingProfile : Profile
{
    public KnowledgeManagementMappingProfile()
    {
        // Mapeamentos para KnowledgeCategory
        CreateMap<KnowledgeCategory, KnowledgeCategoryDto>()
            .ForMember(dest => dest.ParentCategoryName, 
                       opt => opt.MapFrom(src => src.ParentCategory != null ? src.ParentCategory.Name : null))
            .ForMember(dest => dest.PostsCount, opt => opt.Ignore()); // Será preenchido manualmente

        CreateMap<CreateKnowledgeCategoryDto, KnowledgeCategory>();
        
        // Mapeamentos para Tag
        CreateMap<Tag, TagDto>();
        CreateMap<CreateTagDto, Tag>();
    }
}