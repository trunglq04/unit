using Amazon.CognitoIdentityProvider.Model;
using AutoMapper;
using System.Collections.Generic;
using Unit.Entities.Models;
using Unit.Shared.DataTransferObjects;
using Unit.Shared.RequestFeatures;

namespace Unit.API
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AuthenticationResultType, TokenDtoResponse>();
            CreateMap<User, UserDto>();
            CreateMap<UserInfoDtoForUpdate, User>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
