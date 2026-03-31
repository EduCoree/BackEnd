using AutoMapper;
using EduCore.Domain.Entities.QuizModel;
using EduCore.Shared.Dtos.Quiz;
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
            CreateMap<Quiz, QuizDto>();
               
        }

    }
}
