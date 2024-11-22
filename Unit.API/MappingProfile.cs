using Amazon.CognitoIdentityProvider.Model;
using AutoMapper;
using System.Collections.Generic;
using Unit.Entities.Models;
using Unit.Shared.DataTransferObjects.Authentication;
using Unit.Shared.DataTransferObjects.Comment;
using Unit.Shared.DataTransferObjects.NestedReply;
using Unit.Shared.DataTransferObjects.Notification;
using Unit.Shared.DataTransferObjects.Post;
using Unit.Shared.DataTransferObjects.Reply;
using Unit.Shared.DataTransferObjects.User;

namespace Unit.API
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // user
            CreateMap<AuthenticationResultType, TokenDtoResponse>();
            CreateMap<User, UserDto>();
            CreateMap<FollowRequest, FollowRequestDto>();
            CreateMap<UserInfoDtoForUpdate, User>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // post
            CreateMap<Post, PostDto>();
            CreateMap<PostDtoForCreation, Post>();

            // comment
            CreateMap<Comment, CommentDto>();
            CreateMap<CommentDto, Comment>();
            CreateMap<CreateCommentDto, Comment>();
            CreateMap<UpdateCommentDto, Comment>();
            CreateMap<Comment, ResponseCommentDto>();
            CreateMap<Metadata, MetadataDto>();
            CreateMap<MetadataDto, Metadata>();
            CreateMap<Attachment, AttachmentDto>();
            CreateMap<AttachmentDto, Attachment>();
            CreateMap<Attachment, AttachmentDto>();

            // reply
            CreateMap<CreateNestedReplyDto, NestedReply>();
            CreateMap<NestedReplyDto, NestedReply>();

            CreateMap<Reply, ReplyDto>();
            CreateMap<Reply, ResponseReplyDto>();
            CreateMap<ReplyDto, Reply>();
            CreateMap<CreateReplyDto, Reply>();
            CreateMap<UpdateReplyDto, Reply>();

            //notification
            CreateMap<Notification, NotificationDto>();
            CreateMap<NotificationMetadata, NotificationMetadataDto>();
        }
    }
}
