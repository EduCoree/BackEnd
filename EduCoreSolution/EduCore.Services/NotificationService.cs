using AutoMapper;
using EduCore.Domain.Contracts;
using EduCore.Domain.Entities.AuthModel;
using EduCore.Domain.Entities.NotificationsModel;
using EduCore.Services_Abstraction;
using EduCore.Shared.Common;
using EduCore.Shared.DTOs.Notifications;
using EduCore.Shared.Enums;
using EduCore.Shared.Exceptions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EduCore.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationSender _sender;
        private readonly UserManager<User> _userManager;

        public NotificationService(IUnitOfWork unitOfWork,IMapper mapper,INotificationSender sender, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
             _sender = sender;
           _userManager = userManager;
        }

        public async Task DeleteAsync(int notificationId)
        {
            var Notification = await _unitOfWork.NotificationRepository.GetByIdAsync(notificationId);
            if (Notification == null) throw new NotFoundException($"The Notification With id {notificationId} Not Found");
             _unitOfWork.NotificationRepository.Remove(Notification);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> GetUnreadCountAsync(string UserId)
        {
            return await _unitOfWork.NotificationRepository.GetUnreadCountAsync(UserId);
            
        }

        public async Task<NotificationListDto> GetUserNotificationsAync(string userId, PaginationParams pagination)
        {
            var notifications= await _unitOfWork.NotificationRepository.GetUserNotificationsAsync(userId, pagination);
            var unreadCount = await _unitOfWork.NotificationRepository.GetUnreadCountAsync(userId);
            var totalCount = await _unitOfWork.NotificationRepository.GetTotalCountAsync(userId);
            return new NotificationListDto
            {
                Notifications = _mapper.Map<IEnumerable<NotificationDto>>(notifications),
                UnreadCount = unreadCount,
                TotalCount = totalCount
            };

        }

        public async Task MarkAllAsRead(string UserId)
        {
            await _unitOfWork.NotificationRepository.MarkAllAsReadAsync(UserId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task MarkAsRead(int NotificationId)
        {
           var Notification = await _unitOfWork.NotificationRepository.GetByIdAsync(NotificationId);
            if (Notification == null) throw new NotFoundException($"The Notification With id {NotificationId} Not Found");
            await _unitOfWork.NotificationRepository.MarkAsReadAsync(NotificationId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task SendNotificationAsync(string userId, string title, string message, NotificationType notificationType,int entityId, object? extraData = null)
        {
            var Notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = notificationType.ToString(),
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                EntityId = entityId,
                Metadata = extraData != null
            ? JsonSerializer.Serialize(extraData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
            : null
            };
            await _unitOfWork.NotificationRepository.AddAsync(Notification);
            await _unitOfWork.SaveChangesAsync();
            var dto = _mapper.Map<NotificationDto>(Notification);
            await _sender.SendAsync(userId, dto);


        }
        public async Task SendNotificationToAdminsAsync(string title, string message, NotificationType notificationType, int entityId)
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");

            foreach (var admin in admins)
            {
                await SendNotificationAsync(
                    userId: admin.Id,
                    title: title,
                    message: message,
                    notificationType: notificationType,
                    entityId: entityId
                );
            }
        }
    }
}
