using AutoMapper;
using SynQcore.Domain.Entities.Communication;
using SynQcore.Application.Features.KnowledgeManagement.DTOs;

namespace SynQcore.Application.Common.Mappings;

public class KnowledgeManagementProfile : Profile
{
    public KnowledgeManagementProfile()
    {
        // KnowledgeCategory mappings
        CreateMap<KnowledgeCategory, KnowledgeCategoryDto>()
            .ForMember(dest => dest.ParentCategoryName, opt => opt.MapFrom(src => src.ParentCategory != null ? src.ParentCategory.Name : null))
            .ForMember(dest => dest.SubCategories, opt => opt.MapFrom(src => src.SubCategories))
            .ForMember(dest => dest.PostsCount, opt => opt.Ignore()); // Será preenchido na query

        CreateMap<CreateKnowledgeCategoryDto, KnowledgeCategory>();
        CreateMap<UpdateKnowledgeCategoryDto, KnowledgeCategory>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Tag mappings
        CreateMap<Tag, TagDto>();
        CreateMap<CreateTagDto, Tag>();
        CreateMap<UpdateTagDto, Tag>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Post mappings (enhanced for knowledge management)
        CreateMap<Post, KnowledgePostDto>()
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author != null ? $"{src.Author.FirstName} {src.Author.LastName}" : ""))
            .ForMember(dest => dest.AuthorEmail, opt => opt.MapFrom(src => src.Author != null ? src.Author.Email : ""))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : null))
            .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Team != null ? src.Team.Name : null))
            .ForMember(dest => dest.ParentPostTitle, opt => opt.MapFrom(src => src.ParentPost != null ? src.ParentPost.Title : null))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.PostTags.Select(pt => pt.Tag)))
            .ForMember(dest => dest.Versions, opt => opt.MapFrom(src => src.Versions));

        CreateMap<CreateKnowledgePostDto, Post>();
        CreateMap<UpdateKnowledgePostDto, Post>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // PostTag mappings
        CreateMap<PostTag, TagDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Tag.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Tag.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Tag.Description))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Tag.Type))
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Tag.Color))
            .ForMember(dest => dest.UsageCount, opt => opt.MapFrom(src => src.Tag.UsageCount))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Tag.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.Tag.UpdatedAt));
    }
}