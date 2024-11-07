﻿
using Amazon.CognitoIdentityProvider.Model;
using Unit.Shared.DataTransferObjects;

namespace Unit.Service.Contracts
{
    public interface IAuthenticationService
    {
        Task<TokenDtoResponse> SignIn(SignInDtoRequest request);

        Task SignUp(SignUpDtoRequest request);

        Task ConfirmSignUp(ConfirmSignUpDtoRequest request);

        Task IsEmailConfirmed(string email);

        Task ResendConfirmationCode(string email);

        Task<UserType?> GetUserByEmail(string email);

        Task SendCodeResetPassword(string email);

        Task ResetPassword(ResetPasswordDtoRequest request);
    }
}
