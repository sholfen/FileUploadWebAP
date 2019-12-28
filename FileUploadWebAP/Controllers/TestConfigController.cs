using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FileUploadWebAP.Controllers
{
    public class TestClass
    {
        public string Field1 { get; set; }
        public string Field2 { get; set; }
    }

    public class TestConfigController : Controller
    {
        public IActionResult Index()
        {
            string aaa3 = Helpers.Utilities.GetValueByKey("Aaa3");
            return Content(aaa3);
        }

        public IActionResult Index2()
        {
            string val = Helpers.Utilities.GetValueByKey("Obj:Field1");
            return Content(val);
        }

        public IActionResult Index3()
        {
            TestClass inst = Helpers.Utilities.GetValueFromAppSettingsAndByKey<TestClass>("Obj");
            return new JsonResult(inst);
        }
    }
}