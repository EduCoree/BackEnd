using AutoMapper;
using EduCore.Domain.Entities.QuizModel;
using EduCore.Shared.DTOs.Quiz.Student;
using EduCore.Shared.DTOs.Quiz.Teacher;
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
            CreateMap<Quiz, QuizDto>();
            CreateMap<Quiz, QuizDetailsDto>();

            CreateMap<CreateQuestionDto, Question>();
            CreateMap<UpdateQuestionDto, Question>()
                .ForAllMembers(o => o.Condition((src, dest, member) => member != null)); ;
            CreateMap<Question, QuestionDto>();

            CreateMap<CreateAnswerOptionDto, AnswerOption>();
            CreateMap<UpdateAnswerOptionDto, AnswerOption>()
                .ForAllMembers(o => o.Condition((src, dest, member) => member != null)); ;
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

            CreateMap<Quiz, AvailableQuizzesDto>()
    .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course.Title));

        }



    }
}
