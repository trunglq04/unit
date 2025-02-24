using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unit.API.ActionFilter;
using Unit.Service.Contracts;
using Unit.Shared.DataTransferObjects.Authentication;

namespace Unit.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public class AuthenticationController : ControllerBase
    {
        private readonly IServiceManager _service;

        public AuthenticationController(IServiceManager service)
        {
            _service = service;
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn([FromBody] SignInDtoRequest request)
        {
            var tokens = await _service.AuthenticationService.SignIn(request);
            return Ok(tokens);
        }

        [HttpPost("SignUp")]
        [ServiceFilter(typeof(ValidationFilterPasswordConfirmation))]
        public async Task<IActionResult> SignUp([FromBody] SignUpDtoRequest request)
        {
            await _service.AuthenticationService.SignUp(request);
            return Ok();
        }

        [HttpPost("SignOut")]
        [Authorize]
        public async Task<IActionResult> SignOut([FromHeader(Name = "Authorization")] string AccessToken)
        {
            var accessToken = AccessToken.Split(" ").Last()!;
            await _service.AuthenticationService.SignOut(accessToken);
            return Ok();
        }

        [HttpPost("Confirm-Signup")]
        public async Task<IActionResult> ConfirmSignUp([FromBody] ConfirmSignUpDtoRequest request)
        {
            await _service.AuthenticationService.ConfirmSignUp(request);
            return Ok();
        }

        [HttpGet("Resend-Confirmation-Code")]
        public async Task<IActionResult> ResendConfirmationCode([FromQuery] string email)
        {
            await _service.AuthenticationService.IsEmailConfirmed(email, isConfirmed: false);

            await _service.AuthenticationService.ResendConfirmationCode(email);
            return Ok();
        }

        [HttpGet("send-reset-password-Code")]
        public async Task<IActionResult> SendResetPasswordCode([FromQuery] string email)
        {
            await _service.AuthenticationService.IsEmailConfirmed(email, isConfirmed: true);

            await _service.AuthenticationService.SendCodeResetPassword(email);
            return Ok();
        }

        [HttpPost("reset-password")]
        [ServiceFilter(typeof(ValidationFilterPasswordConfirmation))]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDtoRequest request)
        {
            await _service.AuthenticationService.IsEmailConfirmed(request.Email, isConfirmed: true);

            await _service.AuthenticationService.ResetPassword(request);

            return Ok();
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshAccessToken(RefreshTokenDtoRequest request)
        {
            var response = await _service.AuthenticationService.RefreshAccessToken(request.RefreshToken);
            return Ok(response);
        }

    }
}
