using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileUploadWebAP.libs.FilesSaver
{
    public enum FileUploaderType
    {
        LocalFile,
        AzureStorage,
        AWSStorage
    }

    public static class UploaderFactory
    {
        public static Type CreateFileUploader(FileUploaderType fileUploaderType, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            Type type = null;
            switch(fileUploaderType)
            {
                case FileUploaderType.LocalFile:
                    type = typeof(LocalFileUploader);
                    break;
                case FileUploaderType.AzureStorage:
                    type = typeof(AzureTableUploader);
                    AzureTableBlobHelper.StorageConnectionString = configuration.GetValue<string>("FileUploaderConfig:AzureConnectonString");
                    break;
                case FileUploaderType.AWSStorage:
                default:
                    throw new ArgumentException("Type is wrong");
            }

            return type;
        }
    }
}
