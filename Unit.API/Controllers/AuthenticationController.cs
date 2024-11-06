using Microsoft.AspNetCore.Mvc;
using Unit.Entities.ErrorModel;
using Unit.Service.Contracts;
using Unit.Shared.DataTransferObjects;

namespace Unit.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
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
        public async Task<IActionResult> SignUp([FromBody] SignUpDtoRequest request)
        {
            if (request.Password != request.Confirmpassword) return BadRequest(request);
            await _service.AuthenticationService.SignUp(request);
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
            await _service.AuthenticationService.IsEmailConfirmed(email);

            await _service.AuthenticationService.ResendConfirmationCode(email);
            return Ok();
        }

    }
}
