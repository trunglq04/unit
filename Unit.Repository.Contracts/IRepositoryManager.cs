﻿namespace Unit.Repository.Contracts
{
    public interface IRepositoryManager
    {
        IUserRepository User { get; }

        ICommentRepository Comment { get; }

        IPostRepository Post { get; }

        INestedReplyRepository NestedReply { get; }

        IPostLikeListsRepository PostLikeLists { get; }

        INotificationRepository Notification { get; }
    }
}
