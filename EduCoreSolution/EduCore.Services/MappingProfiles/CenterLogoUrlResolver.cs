using AutoMapper;
using AutoMapper.Execution;
using EduCore.Domain.Entities.CenterModel;
using EduCore.Shared.DTOs.Centers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services.MappingProfiles
{
   public class CenterLogoUrlResolver : IValueResolver<Center, CenterDto, string>
    {
        private readonly IConfiguration _configuration;

        public CenterLogoUrlResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }



        public string Resolve(Center source, CenterDto destination, string destMember, ResolutionContext context)
        {
         if(string.IsNullOrEmpty(source.LogoUrl))
                return string.Empty;

            if (source.LogoUrl.StartsWith("http"))
                return source.LogoUrl;

            var BaseUrl = _configuration.GetSection("URLs")["BaseUrl"];
            if (string.IsNullOrEmpty(BaseUrl)) return string.Empty;


            var logUrl = $"{BaseUrl}{source.LogoUrl}";
            return logUrl;
        }
    }
}
