using AutoMapper;
using SynQcore.Application.DTOs.Communication;
using SynQcore.Application.Commands.Communication.DiscussionThreads;
using SynQcore.Application.Queries.Communication.DiscussionThreads;
using SynQcore.Domain.Entities.Communication;
using SynQcore.Domain.Entities.Organization;

namespace SynQcore.Application.Common.Mappings;

/// <summary>
/// Profile do AutoMapper para Discussion Threads e Analytics
/// </summary>
public class DiscussionThreadMappingProfile : Profile
{
    public DiscussionThreadMappingProfile()
    {
        // Mapeamentos para Comment -> DiscussionCommentDto
        CreateMap<Comment, DiscussionCommentDto>()
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.FullName))
            .ForMember(dest => dest.AuthorJobTitle, opt => opt.MapFrom(src => src.Author.JobTitle))
            .ForMember(dest => dest.AuthorProfilePhotoUrl, opt => opt.MapFrom(src => src.Author.ProfilePhotoUrl))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.ModerationStatus, opt => opt.MapFrom(src => src.ModerationStatus.ToString()))
            .ForMember(dest => dest.Visibility, opt => opt.MapFrom(src => src.Visibility.ToString()))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()))
            .ForMember(dest => dest.ResolvedByName, opt => opt.MapFrom(src => src.ResolvedBy != null ? src.ResolvedBy.FullName : null))
            .ForMember(dest => dest.Replies, opt => opt.Ignore()) // Será populado manualmente quando necessário
            .ForMember(dest => dest.IsLikedByCurrentUser, opt => opt.Ignore()); // Será calculado no handler

        // Mapeamentos para CommentMention -> CommentMentionDto
        CreateMap<CommentMention, CommentMentionDto>()
            .ForMember(dest => dest.MentionedEmployeeName, opt => opt.MapFrom(src => src.MentionedEmployee.FullName))
            .ForMember(dest => dest.Context, opt => opt.MapFrom(src => src.Context.ToString()))
            .ForMember(dest => dest.Urgency, opt => opt.MapFrom(src => src.Urgency.ToString()));

        // Mapeamentos para CommentMention -> MentionNotificationDto
        CreateMap<CommentMention, MentionNotificationDto>()
            .ForMember(dest => dest.CommentContent, opt => opt.MapFrom(src => src.Comment.Content))
            .ForMember(dest => dest.PostId, opt => opt.MapFrom(src => src.Comment.PostId))
            .ForMember(dest => dest.PostTitle, opt => opt.MapFrom(src => src.Comment.Post.Title))
            .ForMember(dest => dest.MentionedByName, opt => opt.MapFrom(src => src.MentionedBy.FullName))
            .ForMember(dest => dest.Context, opt => opt.MapFrom(src => src.Context.ToString()))
            .ForMember(dest => dest.Urgency, opt => opt.MapFrom(src => src.Urgency.ToString()));

        // Mapeamentos para Post -> TrendingDiscussionDto
        CreateMap<Post, TrendingDiscussionDto>()
            .ForMember(dest => dest.PostId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.FullName))
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Author.EmployeeDepartments.FirstOrDefault()!.Department.Name))
            .ForMember(dest => dest.CommentCount, opt => opt.MapFrom(src => src.Comments.Count))
            .ForMember(dest => dest.UniqueParticipants, opt => opt.MapFrom(src => src.Comments.Select(c => c.AuthorId).Distinct().Count()))
            .ForMember(dest => dest.LikesCount, opt => opt.MapFrom(src => src.Likes.Count))
            .ForMember(dest => dest.EndorsementCount, opt => opt.MapFrom(src => src.Endorsements.Count))
            .ForMember(dest => dest.UnresolvedQuestions, opt => opt.MapFrom(src => 
                src.Comments.Count(c => (c.Type == CommentType.Question || c.Type == CommentType.Concern) && !c.IsResolved)))
            .ForMember(dest => dest.HasHighPriorityItems, opt => opt.MapFrom(src => 
                src.Comments.Any(c => c.Priority == CommentPriority.High || c.Priority == CommentPriority.Urgent || c.Priority == CommentPriority.Critical)))
            .ForMember(dest => dest.CommentsByType, opt => opt.MapFrom(src => 
                src.Comments.GroupBy(c => c.Type.ToString()).ToDictionary(g => g.Key, g => g.Count())))
            .ForMember(dest => dest.HoursSinceLastActivity, opt => opt.MapFrom(src => 
                (int)(DateTime.UtcNow - src.LastActivityAt).TotalHours))
            .ForMember(dest => dest.TrendingScore, opt => opt.Ignore()) // Calculado no handler
            .ForMember(dest => dest.GrowthRate, opt => opt.Ignore()); // Calculado no handler

        // Mapeamentos para Employee -> TopContributor
        CreateMap<Employee, TopContributor>()
            .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.CommentCount, opt => opt.Ignore()) // Calculado no handler
            .ForMember(dest => dest.QuestionsAnswered, opt => opt.Ignore()) // Calculado no handler
            .ForMember(dest => dest.EndorsementsReceived, opt => opt.Ignore()); // Calculado no handler

        // Mapeamentos para CreateDiscussionCommentDto -> CreateDiscussionCommentCommand
        CreateMap<CreateDiscussionCommentDto, CreateDiscussionCommentCommand>()
            .ConstructUsing(src => new CreateDiscussionCommentCommand(
                src.PostId,
                src.Content,
                src.ParentCommentId,
                src.Type,
                src.Visibility,
                src.IsConfidential,
                src.Priority,
                src.Mentions));

        // Mapeamentos para UpdateDiscussionCommentDto -> UpdateDiscussionCommentCommand
        CreateMap<UpdateDiscussionCommentDto, UpdateDiscussionCommentCommand>();

        // Mapeamentos para ResolveCommentDto -> ResolveDiscussionCommentCommand
        CreateMap<ResolveCommentDto, ResolveDiscussionCommentCommand>();

        // Mapeamentos para ModerateCommentDto -> ModerateDiscussionCommentCommand
        CreateMap<ModerateCommentDto, ModerateDiscussionCommentCommand>();
    }
}