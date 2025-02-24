using Amazon.CognitoIdentityProvider.Model;
using Unit.Shared.DataTransferObjects.Authentication;

namespace Unit.Service.Contracts
{
    public interface IAuthenticationService
    {
        Task<TokenDtoResponse> SignIn(SignInDtoRequest request);

        Task SignUp(SignUpDtoRequest request);

        Task SignOut(string AccessToken);

        Task ConfirmSignUp(ConfirmSignUpDtoRequest request);

        Task IsEmailConfirmed(string email, bool isConfirmed);

        Task ResendConfirmationCode(string email);

        Task<UserType?> GetUserByEmail(string email);

        Task SendCodeResetPassword(string email);

        Task ResetPassword(ResetPasswordDtoRequest request);

        Task<TokenDtoResponse> RefreshAccessToken(string refreshToken);
    }
}
