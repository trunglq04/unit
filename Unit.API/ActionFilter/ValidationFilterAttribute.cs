using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Unit.Entities.ErrorModel;

namespace Unit.API.ActionFilter
{
    public class ValidationFilterAttribute : IActionFilter
    {
        public ValidationFilterAttribute() { }
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var action = context.RouteData.Values["action"];
            var controller = context.RouteData.Values["controller"];

            var paramAction = context.ActionDescriptor.Parameters.SingleOrDefault(p => p.ParameterType.ToString().Contains("Dto"));
            if (paramAction is not null)
            {
                var param = context.ActionArguments
                .SingleOrDefault(x => x.Value.ToString().Contains("Dto"))
                .Value;
                if (param is null)
                {
                    context.Result = new BadRequestObjectResult(new ErrorDetails()
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Object is null"
                    });
                    return;
                }

                if (!context.ModelState.IsValid)
                {
                    var errors = context.ModelState.Where(ms => ms.Value.Errors.Count > 0).SelectMany(ms =>
                        ms.Value.Errors.Select(e => e.ErrorMessage)
                    ).ToList();

                    var errorMessage = string.Join(Environment.NewLine, errors);
                    context.Result = new UnprocessableEntityObjectResult(new ErrorDetails()
                    {
                        StatusCode = StatusCodes.Status415UnsupportedMediaType,
                        Message = errorMessage
                    });
                    return;
                }
            }

        }
    }
}
