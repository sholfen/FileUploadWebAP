## 檔案上傳套件 

花了一些時間，檔案上傳的工具算是初步完成了，雖然要這功能其實很容易實作，但我要的是可以隨時切換不同的檔案上傳目的地，目前已經有一個初版。

檔案 clone 下來後，首先打開 appsettings.json 檔案：

~~~json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "FileUploaderConfig": {
    "LocalFilePath": "Datas\\files\\",
    "AzureConnectonString": "<your key>"
  }
}
~~~

目前工具支援將檔案上傳到本機或是 Azure 上，appsettings.json 是用來設定這兩個上傳方式的設定，`FileUploaderConfig` 是本機的資料夾。而 `AzureConnectonString` 就是 Azure 上的連線字串。

接下來看 Startup.cs 檔案：

~~~csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllersWithViews();
    Helpers.Utilities.Configuration = Configuration;
    Type uploaderType = libs.FilesSaver.UploaderFactory.CreateFileUploader(libs.FilesSaver.FileUploaderType.AzureStorage, Configuration);
    services.AddTransient(typeof(libs.FilesSaver.IFileUploader), uploaderType);
}
~~~

`Type uploaderType = libs.FilesSaver.UploaderFactory.CreateFileUploader(libs.FilesSaver.FileUploaderType.AzureStorage, Configuration);` 是設定檔案上傳要使用本機或 Azure，最後一行就是 DI 的設定。

~~~csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{

    ......
    Helpers.Utilities.ServerPath = env.WebRootPath + "\\";
}
~~~

上面的設定是為了要讓上傳工具可以知道網站的真正根目錄位置，如果是用 Azure 的話就不用設定。接下來看 Web API 的程式碼：

~~~csharp
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
~~~

除了上傳檔案要設定 FileInfo 物件，下載檔案就比較單純，只要用 token 就可以直接取得了。

**參考資料**

- <a href="https://dotblogs.com.tw/help/2017/08/16/aspnetcore_api_upload_file" target="_blank">Asp.Net Core Web API上傳檔案</a>
- <a href="https://docs.microsoft.com/zh-tw/aspnet/core/mvc/models/file-uploads?view=aspnetcore-3.0" target="_blank">上傳 ASP.NET Core 中的檔案</a>
- <a href="https://docs.microsoft.com/en-us/azure/cosmos-db/tutorial-develop-table-dotnet" target="_blank">Get started with Azure Cosmos DB Table API and Azure Table storage using the .NET SDK</a>
- <a href="https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet" target="_blank">Quickstart: Azure Blob storage client library v12 for .NET</a>
- <a href="https://stackoverflow.com/questions/45727856/how-to-download-a-file-in-asp-net-core" target="_blank">How to download a file in ASP.NET Core</a>
- <a href="https://stackoverflow.com/questions/311165/how-do-you-convert-a-byte-array-to-a-hexadecimal-string-and-vice-versa" target="_blank">How do you convert a byte array to a hexadecimal string, and vice versa?
</a>