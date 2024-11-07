
namespace Unit.Shared.DataTransferObjects
{
    public interface IPasswordConfirmation
    {
        string Password { get; }
        string ConfirmPassword { get; }
    }
}
