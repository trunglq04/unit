namespace Unit.Service.Contracts
{
    public interface IServiceManager
    {
        IUserService UserService { get; }

        IPostService PostService { get; }

        IAuthenticationService AuthenticationService { get; }

        ICommentService CommentService { get; }

        INotificationService NotificationService { get; }
    }
}
