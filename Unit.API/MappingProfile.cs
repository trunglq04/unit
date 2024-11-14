using Amazon.CognitoIdentityProvider.Model;
using AutoMapper;
using System.Collections.Generic;
using Unit.Entities.Models;
using Unit.Shared.DataTransferObjects;
using Unit.Shared.DataTransferObjects.Comment;
using Unit.Shared.RequestFeatures;

namespace Unit.API
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //user
            CreateMap<AuthenticationResultType, TokenDtoResponse>();
            CreateMap<User, UserDto>();
            CreateMap<UserInfoDtoForUpdate, User>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            //post
            CreateMap<Post, PostDto>();
            CreateMap<PostDtoForCreation, Post>();

            CreateMap<Comment, CommentDto>();
            CreateMap<CommentDto, Comment>();

            CreateMap<MetadataDto, Metadata>();
            CreateMap<AttachmentDto, Attachment>();

        }
    }
}
