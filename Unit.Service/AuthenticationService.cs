using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Extensions.CognitoAuthentication;
using AutoMapper;
using Microsoft.Extensions.Options;
using Unit.Entities.ConfigurationModels;
using Unit.Entities.Exceptions;
using Unit.Entities.Exceptions.Messages;
using Unit.Entities.Models;
using Unit.Repository.Contracts;
using Unit.Service.Contracts;
using Unit.Shared.DataTransferObjects.Authentication;

namespace Unit.Service
{
    public sealed class AuthenticationService : IAuthenticationService
    {
        private readonly ILoggerManager _logger;

        private readonly IRepositoryManager _repository;

        private readonly IAmazonCognitoIdentityProvider _provider;

        private readonly CognitoUserPool _userPool;

        private readonly string _clientId;

        private readonly IMapper _mapper;

        public AuthenticationService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper,
            IAmazonCognitoIdentityProvider cognitoProvider, IOptions<AWSConfiguration> configuration)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;

            _provider = cognitoProvider;
            var userPoolId = configuration.Value.Cognito!.UserPoolId;
            _clientId = configuration.Value.Cognito!.ClientId!;
            _userPool = new CognitoUserPool(userPoolId, _clientId, _provider);
        }

        public async Task<TokenDtoResponse> SignIn(SignInDtoRequest request)
        {
            var user = new CognitoUser(request.Email, _clientId, _userPool, _provider);

            var authRequest = new InitiateSrpAuthRequest()
            {
                Password = request.Password
            };

            var authResponse = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);
            var token = _mapper.Map<TokenDtoResponse>(authResponse.AuthenticationResult);

            return token;
        }

        public async Task SignUp(SignUpDtoRequest request)
        {
            var signUpRequest = new SignUpRequest
            {
                ClientId = _clientId,
                Username = request.Email,
                Password = request.Password,
                UserAttributes = new List<AttributeType>
                {
                    new AttributeType
                    {
                        Name = "email",
                        Value = request.Email
                    }
                }
            };
            await _provider.SignUpAsync(signUpRequest);
        }

        public async Task SignOut(string AccessToken)
        {
            var signOutRequest = new GlobalSignOutRequest()
            {
                AccessToken = AccessToken
            };

            await _provider.GlobalSignOutAsync(signOutRequest);
        }

        public async Task<UserType?> GetUserByEmail(string email)
        {
            var listUsersRequest = new ListUsersRequest
            {
                UserPoolId = _userPool.PoolID,
                Filter = $"email = \"{email}\""
            };

            var response = await _provider.ListUsersAsync(listUsersRequest);
            var user = response.Users.FirstOrDefault();
            return user;
        }

        public async Task ConfirmSignUp(ConfirmSignUpDtoRequest request)
        {
            var confirmRequest = new ConfirmSignUpRequest
            {
                ClientId = _clientId,
                Username = request.Email,
                ConfirmationCode = request.ConfirmCode
            };

            await _provider.ConfirmSignUpAsync(confirmRequest);

            var user = await GetUserByEmail(request.Email);
            var userName = request.Email.Split('@')[0];
            var newUser = new User()
            {
                UserName = userName,
                UserId = user.Username,
                CreatedAt = user.UserCreateDate,
                LastModified = user.UserLastModifiedDate,
                Private = false,
                Active = true
            };
            await _repository.User.CreateUserAsync(newUser);
        }

        public async Task IsEmailConfirmed(string email, bool isConfirmed)
        {
            var request = new AdminGetUserRequest
            {
                Username = email,
                UserPoolId = _userPool.PoolID

            };

            var response = await _provider.AdminGetUserAsync(request);

            var emailVerifiedAttribute = response.UserAttributes
                .FirstOrDefault(attr => attr.Name == "email_verified");
            if (emailVerifiedAttribute != null && emailVerifiedAttribute.Value == "false" && isConfirmed)
                throw new BadRequestException(AuthExMsg.EmailIsNotConfirmed);
            if (emailVerifiedAttribute != null && emailVerifiedAttribute.Value == "true" && !isConfirmed)
                throw new BadRequestException(AuthExMsg.EmailAlreadyConfirmed);
        }

        public async Task ResendConfirmationCode(string email)
        {
            var resendRequest = new ResendConfirmationCodeRequest
            {
                ClientId = _clientId,
                Username = email
            };

            await _provider.ResendConfirmationCodeAsync(resendRequest);
        }

        public async Task SendCodeResetPassword(string email)
        {
            var request = new ForgotPasswordRequest()
            {
                Username = email,
                ClientId = _clientId
            };

            var response = await _provider.ForgotPasswordAsync(request);
        }

        public async Task ResetPassword(ResetPasswordDtoRequest requestDto)
        {
            var request = new ConfirmForgotPasswordRequest()
            {
                ConfirmationCode = requestDto.Code,
                Password = requestDto.Password,
                Username = requestDto.Email,
                ClientId = _clientId
            };

            await _provider.ConfirmForgotPasswordAsync(request);
        }

        public async Task<TokenDtoResponse> RefreshAccessToken(string refreshToken)
        {
            var authRequest = new InitiateAuthRequest()
            {
                AuthFlow = AuthFlowType.REFRESH_TOKEN,
                ClientId = _clientId,
                AuthParameters = new Dictionary<string, string>
                {
                    { "REFRESH_TOKEN", refreshToken }
                }
            };

            var authResponse = await _provider.InitiateAuthAsync(authRequest);
            var tokens = _mapper.Map<TokenDtoResponse>(authResponse.AuthenticationResult);

            return tokens;
        }
    }
}
