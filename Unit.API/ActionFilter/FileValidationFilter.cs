using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using MediaToolkit.Model;
using MediaToolkit;
using Unit.Entities.ErrorModel;
using Unit.Entities.Exceptions.Messages;

namespace Unit.API.ActionFilter
{
    public class FileValidationFilter : IAsyncActionFilter
    {
        private const long MaxImageSize = 30 * 1024 * 1024; // 30MB
        private const int MaxFileCount = 20; // Giới hạn số lượng file tải lên
        private readonly string[] permittedImageTypes ={"image/jpeg",
                                                        "image/png",
                                                        "image/gif",
                                                        "image/bmp",
                                                        "image/webp",
                                                        "image/tiff",
                                                        "image/svg+xml"
                                                    };
        private readonly string[] permittedVideoTypes = { "video/mp4", "video/quicktime", "video/x-msvideo", "video/x-ms-wmv", "video/x-matroska" };
        private const int MaxVideoDurationInSeconds = 5 * 60; // 5 phút

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionArguments.TryGetValue("media", out var mediaObj) && mediaObj is List<IFormFile> mediaFiles)
            {
                if (mediaFiles.Count > MaxFileCount)
                {
                    context.Result = new BadRequestObjectResult(new ErrorDetails()
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = UserExMsg.MaximumFileUpload + $" {MaxFileCount}"
                    });
                    return;
                }

                foreach (var file in mediaFiles)
                {
                    if (permittedImageTypes.Contains(file.ContentType))
                    {
                        if (file.Length > MaxImageSize)
                        {
                            context.Result = new BadRequestObjectResult(new ErrorDetails()
                            {
                                StatusCode = StatusCodes.Status400BadRequest,
                                Message = UserExMsg.ImageOverSize30Mb
                            });
                            return;
                        }
                    }
                    else if (permittedVideoTypes.Contains(file.ContentType))
                    {
                        if (!await IsValidVideoDuration(file, MaxVideoDurationInSeconds))
                        {
                            context.Result = new BadRequestObjectResult(new ErrorDetails()
                            {
                                StatusCode = StatusCodes.Status400BadRequest,
                                Message = UserExMsg.MaximumVideoDuration
                            });
                            return;
                        }
                    }
                    else
                    {
                        context.Result = new BadRequestObjectResult(new ErrorDetails()
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            Message = UserExMsg.WrongPostFormatImage
                        });
                        return;
                    }
                }
            }

            await next();
        }

        private async Task<bool> IsValidVideoDuration(IFormFile videoFile, int maxDurationInSeconds)
        {

            var tempFilePath = Path.GetTempFileName();
            try
            {
                using (var fileStream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await videoFile.CopyToAsync(fileStream);
                }


                var inputFile = new MediaFile { Filename = tempFilePath };
                using (var engine = new Engine())
                {
                    engine.GetMetadata(inputFile);
                    var duration = inputFile.Metadata.Duration;
                    return duration <= TimeSpan.FromSeconds(maxDurationInSeconds);
                }
            }
            finally
            {
                File.Delete(tempFilePath);
            }
        }
    }
}
