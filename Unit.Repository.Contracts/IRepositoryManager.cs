namespace Unit.Repository.Contracts
{
    public interface IRepositoryManager
    {
        IUserRepository User { get; }
        IPostRepository Post { get; }
    }
}
