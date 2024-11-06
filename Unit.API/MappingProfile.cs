using Amazon.CognitoIdentityProvider.Model;
using AutoMapper;
using Unit.Shared.DataTransferObjects;

namespace Unit.API
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TokenDtoResponse, AuthenticationResultType>();
        }
    }
}
