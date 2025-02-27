﻿using System.Dynamic;
using Unit.Shared.DataTransferObjects.User;
using Unit.Shared.RequestFeatures;

namespace Unit.Service.Contracts
{
    public interface IUserService
    {
        Task<(IEnumerable<ExpandoObject> users, MetaData metaData)> GetUsersAsync(UserParameters parameters, string token);

        Task<ExpandoObject> GetUserByIdAsync(UserParameters parameters, string token, string? id = null);

        Task<(IEnumerable<ExpandoObject> users, MetaData metaData)> GetUsersByIdsAsync(UserParameters parameters, string token, List<string> ids);

        Task UpdateUser(UserInfoDtoForUpdate userDtoForUpdate, string id, string? imagePath = null);

        Task<string> UploadUserImageAsync(string userId, FileInfo imageFile);

    }
}