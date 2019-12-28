using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileUploadWebAP.libs.FilesSaver;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FileUploadWebAP.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private IFileUploader fileUploader;

        public ValuesController(IFileUploader fileUploader)
        {
            this.fileUploader = fileUploader;
        }

        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            FileInfo fileInfo = fileUploader.GetFile(id);
            MediaTypeHeaderValue mediaTypeHeaderValue = new MediaTypeHeaderValue("application/octet-stream");
            return File(fileInfo.FileStream, "application/octet-stream", fileInfo.FileName);
            //return new FileStreamResult(fileInfo.FileStream, mediaTypeHeaderValue);
        }

        // POST api/<controller>
        [HttpPost]
        public string Post([FromForm]IFormFile file)
        {
            FileInfo fileInfo = new FileInfo
            {
                FileName = file.FileName,
                FileStream = file.OpenReadStream(),
                Token = FileInfo.CreateToken("peter", file.FileName),
                Version = "0.1"
            };
            fileUploader.WriteFile(fileInfo);
            return "OK";
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
