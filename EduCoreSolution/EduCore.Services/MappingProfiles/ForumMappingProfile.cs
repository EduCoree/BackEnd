using AutoMapper;
using EduCore.Domain.Entities.ForumModel;
using EduCore.Shared.DTOs.Forum;

namespace EduCore.Services.MappingProfiles
{
    public class ForumMappingProfile : Profile
    {
        public ForumMappingProfile()
        {
            CreateMap<ForumPost, ForumPostDto>()
                .ForMember(d => d.StudentName, o => o.MapFrom(s => s.Student.Name))
                .ForMember(d => d.ReplyCount, o => o.MapFrom(s => s.Replies.Count(r => !r.IsRemoved)));

            CreateMap<ForumPost, ForumPostDetailDto>()
                .ForMember(d => d.StudentName, o => o.MapFrom(s => s.Student.Name))
                .ForMember(d => d.Replies, o => o.MapFrom(s => s.Replies.Where(r => !r.IsRemoved)));

            CreateMap<ForumReply, ForumReplyDto>()
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.User.Name));

            CreateMap<PostReport, PostReportDto>()
                .ForMember(d => d.PostTitle, o => o.MapFrom(s => s.Post.Title))
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.User.Name));
        }
    }
}
