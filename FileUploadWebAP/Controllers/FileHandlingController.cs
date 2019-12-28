using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileUploadWebAP.Controllers
{
    public class FileHandlingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult FileUpload()
        { 
            return View();
        }

        [HttpPost]
        public IActionResult FileUpload(IFormFile file)
        {
            return Ok("upload success");
        }

        [HttpPost]
        public async Task<IActionResult> UploadOneFileAsync([FromForm]IFormFile file)
        {
            return Ok("upload success");
        }

        [HttpPost]
        public async Task<IActionResult> UploadFilesAsync(List<IFormFile> files)
        {
            return Ok("upload success");
        }
    }
}