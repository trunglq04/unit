using Microsoft.AspNetCore.Mvc;
using Unit.Service;

namespace Unit.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly S3Service _s3Service;

        public FileUploadController(S3Service s3Service)
        {
            _s3Service = s3Service;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is not selected.");
            }

            using var fileStream = file.OpenReadStream();
            var result = await _s3Service.UploadFileAsync("your-bucket-name", file.FileName, fileStream);

            return Ok(result);
        }
    }
}
