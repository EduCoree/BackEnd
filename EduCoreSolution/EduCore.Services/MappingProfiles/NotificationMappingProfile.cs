using AutoMapper;
using EduCore.Domain.Entities.NotificationsModel;
using EduCore.Shared.DTOs.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services.MappingProfiles
{
    public class NotificationMappingProfile:Profile
    {
        public NotificationMappingProfile()
        {
            CreateMap<Notification, NotificationDto>();
        }

    }
}
