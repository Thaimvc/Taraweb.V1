using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Taraweb.Data;
using Taraweb.Models.TarawebM1;

namespace Taraweb.Controllers
{
    public partial class UploadController : Controller
    {
        private readonly TarawebM1Context db;
        private readonly IWebHostEnvironment environment;

        List<Gallery> Galleries = new List<Gallery>();

        public UploadController(IWebHostEnvironment environment, TarawebM1Context context)
        {
            this.environment = environment;
            this.db = context;
        }

        // Single file upload
        [HttpPost("upload/single")]
        public IActionResult Single(IFormFile file)
        {
            try
            {
                Console.WriteLine(file.FileName);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

                using (var stream = new FileStream(Path.Combine(environment.WebRootPath + "/uploads/", fileName), FileMode.Create))
                {
                    // Save the file
                    file.CopyTo(stream);

                    // Return the URL of the file
                    var url = Url.Content($"~/uploads/{fileName}");
                    var g = new Gallery();

                    g.Url = url;
                    g.FileName = file.FileName;
                    g.FileType = Path.GetExtension(file.FileName);
                    g.DateCreate = DateTime.Now;
                    g.DateUpdate = DateTime.Now;
                    g.IsActive = true;
                    g.FileExtension = Path.GetExtension(file.FileName);
                    Galleries.Add(g);
                    AddGallery();
                    return Ok(new { Url = url, id = g.Id });
                }
                // Put your code here

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Multiple files upload
        [HttpPost("upload/multiple")]
        public IActionResult Multiple(IFormFile[] files)
        {
            try
            {
                foreach (var file in files)
                {
                    Console.WriteLine(file.FileName);
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

                    using (var stream = new FileStream(Path.Combine(environment.WebRootPath + "/uploads/", fileName), FileMode.Create))
                    {
                        // Save the file
                        file.CopyTo(stream);

                        // Return the URL of the file
                        var url = Url.Content($"~/uploads/{fileName}");

                        return Ok(new { Url = url });
                    }
                }
                // Put your code here
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Multiple files upload with parameter
        [HttpPost("upload/{id}")]
        public IActionResult Post(IFormFile[] files, int id)
        {
            try
            {
                // Put your code here
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Image file upload (used by HtmlEditor components)
        [HttpPost("upload/image")]
        public IActionResult Image(IFormFile file)
        {
            try
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

                using (var stream = new FileStream(Path.Combine(environment.WebRootPath + "/uploads/", fileName), FileMode.Create))
                {
                    // Save the file
                    file.CopyTo(stream);

                    // Return the URL of the file
                    var url = Url.Content($"~/uploads/{fileName}");

                    return Ok(new { Url = url });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }


        }

        void AddGallery()
        {

            db.Galleries.AddRange(Galleries);
            db.SaveChanges();
        }
    }
}
