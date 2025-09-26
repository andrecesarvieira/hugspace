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
        
        // Mapeamentos para Post (Knowledge Articles)
        CreateMap<Post, KnowledgePostDto>()
            .ForMember(dest => dest.AuthorName, 
                       opt => opt.MapFrom(src => $"{src.Author.FirstName} {src.Author.LastName}"))
            .ForMember(dest => dest.AuthorEmail, 
                       opt => opt.MapFrom(src => src.Author.Email))
            .ForMember(dest => dest.CategoryName, 
                       opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
            .ForMember(dest => dest.DepartmentName, 
                       opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : null))
            .ForMember(dest => dest.TeamName, 
                       opt => opt.MapFrom(src => src.Team != null ? src.Team.Name : null))
            .ForMember(dest => dest.ParentPostTitle, 
                       opt => opt.MapFrom(src => src.ParentPost != null ? src.ParentPost.Title : null))
            .ForMember(dest => dest.Tags, 
                       opt => opt.MapFrom(src => src.PostTags.Select(pt => pt.Tag)))
            .ForMember(dest => dest.Versions, 
                       opt => opt.MapFrom(src => src.Versions.Where(v => !v.IsDeleted)));

        CreateMap<CreateKnowledgePostDto, Post>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());
    }
}