using AutoMapper;
using EduCore.Domain.Entities.QuizModel;
using EduCore.Shared.DTOs.Quiz.Student;
using EduCore.Shared.DTOs.Quiz.Teacher;
using EduCore.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services.MappingProfiles
{
    public class QuizMappingProfile:Profile
    {
         public QuizMappingProfile()
         {
            CreateMap<CreateQuizDto, Quiz>();
            CreateMap<UpdateQuizDto, Quiz>()
                .ForAllMembers(o => o.Condition((src, dest, member) => member != null)); ;
            CreateMap<Quiz, QuizDto>()
                  .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course.Title));
            CreateMap<Quiz, QuizDetailsDto>()
                 .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course.Title));

            CreateMap<CreateQuestionDto, Question>()
                .ForMember(dest => dest.AnswerOptions, opt => opt.MapFrom(src => src.AnswerOptions));
            CreateMap<UpdateQuestionDto, Question>()
              .ForMember(dest => dest.AnswerOptions, opt => opt.Ignore())
              .ForAllMembers(o => o.Condition((src, dest, member) => member != null));
            CreateMap<Question, QuestionDto>();

            CreateMap<CreateAnswerOptionDto, AnswerOption>();
            CreateMap<UpdateAnswerOptionDto, AnswerOption>()
      .ForMember(dest => dest.Id, opt => opt.Ignore())
      .ForMember(dest => dest.QuestionId, opt => opt.Ignore())
      .ForAllMembers(o => o.Condition((src, dest, member) => member != null));

            CreateMap<AnswerOption, AnswerOptionDto>();

            CreateMap<Quiz, StudentQuizDto>();
            CreateMap<Question, StudentQuestionDto>();
            CreateMap<AnswerOption, StudentAnswerOptionDto>();
            CreateMap<QuizAttempt, AttemptDto>();
            CreateMap<QuizAttempt, AttemptHistoryDto>()
            .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Quiz.Course.Title))
            .ForMember(dest => dest.TotalPoints, opt => opt.MapFrom(src =>
            src.Quiz.Questions != null ? src.Quiz.Questions.Sum(q => q.Points) : 0))
            .ForMember(dest => dest.EarnedPoints, opt => opt.MapFrom(src =>
            (src.Score.HasValue && src.Quiz.Questions != null)
              ? (int)Math.Round((src.Score.Value / 100) * src.Quiz.Questions.Sum(q => q.Points))
              : 0
            ));


            //quiz ai
            CreateMap<Quiz, AvailableQuizzesDto>()
    .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course.Title));

            CreateMap<AiGeneratedQuestionDto, Question>()
                .ForMember(dest => dest.QuizId, opt => opt.Ignore())
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => Enum.Parse<QuestionType>(src.Type, true)));
            CreateMap<AiGeneratedOptionDto, AnswerOption>()
    .ForMember(dest => dest.QuestionId, opt => opt.Ignore());

        }



    }
   
}
