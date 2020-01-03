using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FileUploadWebAP.libs.FilesSaver;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileUploadWebAP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadingController : ControllerBase
    {
        private IFileUploader fileUploader;

        public FileUploadingController(IFileUploader fileUploader)
        {
            this.fileUploader = fileUploader;
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            FileInfo fileInfo = fileUploader.GetFile(id);
            MediaTypeHeaderValue mediaTypeHeaderValue = new MediaTypeHeaderValue("application/octet-stream");
            return File(fileInfo.FileStream, "application/octet-stream", fileInfo.FileName);
        }

        // POST api/<controller>
        [HttpPost]
        public IActionResult Post([FromForm]IFormFile file)
        {
            FileInfo fileInfo = new FileInfo
            {
                FileName = file.FileName,
                FileStream = file.OpenReadStream(),
                Token = FileInfo.CreateToken("peter", file.FileName),
                Version = "0.1"
            };
            fileUploader.WriteFile(fileInfo);
            return new JsonResult(new { Token = fileInfo.Token });
        }
    }
}