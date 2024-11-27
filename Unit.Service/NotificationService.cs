using AutoMapper;
using Microsoft.Extensions.Options;
using Unit.Entities.ConfigurationModels;
using Unit.Entities.Exceptions;
using Unit.Repository.Contracts;
using Unit.Service.Contracts;
using Unit.Service.Helper;
using Unit.Shared.DataTransferObjects.Notification;
using Unit.Shared.RequestFeatures;

namespace Unit.Service
{
    public class NotificationService : INotificationService
    {
        private readonly ILoggerManager _logger;

        private readonly IRepositoryManager _repository;

        private readonly IMapper _mapper;


        public NotificationService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task DeleteNotificationById(string token, string createdAt)
        {
            var userId = JwtHelper.GetPayloadData(token, "username");

            await _repository.Notification.DeleteNotification(userId!, createdAt);
        }

        public async Task<(List<NotificationDto> notificationDtos, MetaData metaData)> GetAllNotificationOfUser(NotificationParameters parameters, string token)
        {
            var userId = JwtHelper.GetPayloadData(token, "username");
            var listNotificationEntity = await _repository.Notification.GetAllNotificationsOfUser(parameters, userId!);

            var listNotificationDto = _mapper.Map<List<NotificationDto>>(listNotificationEntity);

            foreach (var notificationDto in listNotificationDto)
            {
                if (notificationDto.Metadata!.LastestActionUserId == null) continue;

                var LastedActionUser = await _repository.User.GetUserAsync(notificationDto.Metadata!.LastestActionUserId);

                notificationDto.Metadata.ProfilePicture = LastedActionUser.ProfilePicture;
                notificationDto.Metadata.UserName = LastedActionUser.UserName;
            }

            return (notificationDtos: listNotificationDto, listNotificationEntity.MetaData);
        }

        public async Task UpdateNotificationById(string token, string createdAt)
        {
            var userId = JwtHelper.GetPayloadData(token, "username");
            var notificationOfUser = await _repository.Notification.GetNotificationById(userId, createdAt);
            if (notificationOfUser is null) throw new BadRequestException("Notification is not exist!!");
            notificationOfUser.IsSeen = true;

            await _repository.Notification.UpdateNotification(notificationOfUser);
        }

    }
}
