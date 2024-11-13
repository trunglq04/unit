using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Unit.Entities.ErrorModel;
using Unit.Entities.Exceptions.Messages;

namespace Unit.API.ActionFilter
{
    public class ImageFileValidationFilter : IActionFilter
    {
        private readonly long _maxFileSize;
        private readonly string[] _permittedImageTypes;

        public ImageFileValidationFilter(long maxFileSize = 10 * 1024 * 1024, string[]? permittedImageTypes = null)
        {
            _maxFileSize = maxFileSize;
            _permittedImageTypes = permittedImageTypes ?? new[] { "image/jpeg", "image/png", "image/gif" };
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionArguments.TryGetValue("imageFile", out var imageFileObj) && imageFileObj is IFormFile imageFile)
            {

                if (!_permittedImageTypes.Contains(imageFile.ContentType))
                {
                    context.Result = new BadRequestObjectResult(new ErrorDetails()
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = UserExMsg.WrongFormatImage
                    });
                    return;
                }


                if (imageFile.Length > _maxFileSize)
                {
                    context.Result = new BadRequestObjectResult(new ErrorDetails()
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = UserExMsg.ProfileImageOverSize
                    });
                    return;
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
