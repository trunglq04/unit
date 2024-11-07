using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Unit.Entities.ErrorModel;
using Unit.Entities.Exceptions;
using Unit.Shared.DataTransferObjects;

namespace Unit.API.ActionFilter
{
    public class ValidationFilterPasswordConfirmation : IActionFilter
    {
        public ValidationFilterPasswordConfirmation() { }
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var passwordDto = context.ActionArguments
                .Values
                .OfType<IPasswordConfirmation>()
                .FirstOrDefault()!;

            if (passwordDto.Password != passwordDto.ConfirmPassword)
            {
                context.Result = new BadRequestObjectResult(new ErrorDetails()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Password and confirmpassword is not match"
                });
                return;
            }
        }
    }
}