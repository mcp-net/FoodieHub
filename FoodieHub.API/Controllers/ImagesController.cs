using FoodieHub.API.Authentication;
using FoodieHub.API.Models.Domain;
using FoodieHub.API.Models.DTO;
using FoodieHub.API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.PortableExecutable;

namespace FoodieHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository _imageRepository;

        public ImagesController(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        // POST: /api/Images/Upload
        [Authorize(AuthenticationSchemes = JweAuthenticationDefaults.AuthenticationScheme, Roles = "RestaurantOwner")]
        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> Upload([FromForm] ImageUploadRequestDto request)
        {
            ValidateFileUpload(request);

            if (ModelState.IsValid)
            {
                // Convert DTO to Domain model
                var imageDomainModel = new Image
                {
                    File = request.File,
                    FileExtension = Path.GetExtension(request.File.FileName),
                    FileSizeInBytes = request.File.Length,
                    FileName = request.FileName,
                    FileDescription = request.FileDescription,
                };

                // User repository to upload image
                await _imageRepository.UploadAsync(imageDomainModel);

                return Ok(imageDomainModel);
            }

            return BadRequest(ModelState);
        }



        [HttpGet("{fileName}")]
        [Authorize(Roles = "Reviewer,RestaurantOwner")]
        public IActionResult Get(string fileName)
        {
            var path = _imageRepository.GetPhysicalPath(fileName);
            if (!System.IO.File.Exists(path)) return NotFound();

            var mime = Path.GetExtension(path).ToLower() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };

            var stream = System.IO.File.OpenRead(path);
            return File(stream, mime);
        }



        [HttpDelete("{fileName}")]
        [Authorize(Roles = "RestaurantOwner")]
        public async Task<IActionResult> Delete(string fileName)
        {
            var success = await _imageRepository.DeleteAsync(fileName);
            if (!success) return NotFound();
            return NoContent();
        }





        private void ValidateFileUpload(ImageUploadRequestDto request)
        {
            var allowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };

            if (!allowedExtensions.Contains(Path.GetExtension(request.File.FileName)))
            {
                ModelState.AddModelError("file", "Unsupported file extension");
            }

            if (request.File.Length > 10485760)
            {
                ModelState.AddModelError("file", "File size more than 10MB, please upload a smaller size file.");
            }
        }


        [HttpGet]
        [Authorize(Roles = "Reviewer")]
        public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var (items, total) = await _imageRepository.GetPagedAsync(page, pageSize);

            //Response.Headers.Add("X-Total-Count", total.ToString());

            // New:
            //Response.Headers.Append("X-Total-Count", total.ToString());

            //Add will throw if the header already exists; Append will concatenate.
            //Alternatively, to overwrite:

            Response.Headers["X-Total-Count"] = total.ToString();


            //var dtos = items.Select(img => new ImageResponseDto { … });
            // Replace the ellipsis with real assignments:
            var dtos = items.Select(img => new ImageResponseDto
            {
                Id = img.Id,
                FileName = img.FileName,
                FileExtension = img.FileExtension,
                FileSizeInBytes = img.FileSizeInBytes,
                FilePath = img.FilePath,
                FileDescription = img.FileDescription
            }).ToList();


            return Ok(dtos);
        }



        private void ValidateFileUpload2(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowed = new[] { ".jpg", ".jpeg", ".png" };
            if (!allowed.Contains(ext))
                ModelState.AddModelError("File", "Only JPG/PNG allowed");

            if (file.Length > 10 * 1024 * 1024)
                ModelState.AddModelError("File", "Max size is 10MB");
        }

    }
}