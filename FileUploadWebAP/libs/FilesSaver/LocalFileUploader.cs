using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploadWebAP.libs.FilesSaver
{
    public class LocalFileUploader : IFileUploader
    {
        public FileInfo GetFile(string token)
        {
            FileInfo fileInfo = null;
            TokenInfo tokenInfo = FileInfo.DecodeToken(token);
            string serverPath = Helpers.Utilities.ServerPath;
            string localFilePath = Helpers.Utilities.GetValueByKey("FilePath");
            string fileFullPath = $"{serverPath}{localFilePath}{tokenInfo.FileName}.{tokenInfo.Id}-{tokenInfo.Guid}";
            string tokenFullPath = $"{serverPath}{localFilePath}{tokenInfo.Id}-{tokenInfo.Guid}.token";
            using FileStream streamReader = new FileStream(tokenFullPath, FileMode.Open);
            byte[] dataBytes = new byte[streamReader.Length];
            streamReader.Read(dataBytes, 0, (int)streamReader.Length);
            string decodeStr = Encoding.UTF8.GetString(dataBytes);
            fileInfo = System.Text.Json.JsonSerializer.Deserialize<FileInfo>(decodeStr);
            fileInfo.FileStream = new FileStream(fileFullPath, FileMode.Open);

            return fileInfo;
        }

        public void WriteFile(FileInfo fileInfo)
        {
            if (fileInfo == null)
            {
                throw new ArgumentNullException(nameof(fileInfo));
            }

            TokenInfo tokenInfo = FileInfo.DecodeToken(fileInfo.Token);

            //write file
            string serverPath = Helpers.Utilities.ServerPath;
            string localFilePath = Helpers.Utilities.GetValueByKey("FileUploaderConfig:LocalFilePath");
            string fullPath = $"{serverPath}{localFilePath}{fileInfo.FileName}.{tokenInfo.Id}-{tokenInfo.Guid}";
            //string fullPath = $"{serverPath}{localFilePath}{fileInfo.FileName}";
            using FileStream fileStream = new FileStream(fullPath, FileMode.Create);
            fileInfo.FileStream.CopyTo(fileStream);

            //write token file
            string tokenFileName = $"{tokenInfo.Id}-{tokenInfo.Guid}.token";
            string fullTokenFilePath = $"{serverPath}{localFilePath}{tokenFileName}";
            fileInfo.FileStream = null;
            string fileInfoJSONStr = System.Text.Json.JsonSerializer.Serialize(fileInfo);
            using FileStream tokenFileStream = new FileStream(fullTokenFilePath, FileMode.Create);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(fileInfoJSONStr);
            tokenFileStream.Write(jsonBytes, 0, jsonBytes.Length);
        }
    }
}
