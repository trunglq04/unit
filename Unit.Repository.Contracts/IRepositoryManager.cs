namespace Unit.Repository.Contracts
{
    public interface IRepositoryManager
    {
        IUserRepository User { get; }
        ICommentRepository Comment { get; }
        IPostRepository Post { get; }
    }
}
