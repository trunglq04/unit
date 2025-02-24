namespace Unit.Shared.DataTransferObjects.Authentication
{
    public interface IPasswordConfirmation
    {
        string Password { get; }
        string ConfirmPassword { get; }
    }
}
