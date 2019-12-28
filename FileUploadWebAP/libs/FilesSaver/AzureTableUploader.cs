using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileUploadWebAP.libs.FilesSaver
{
    #region Azure Table
    public static class AzureTableBlobHelper
    {
        private static CloudStorageAccount CreateStorageAccountFromConnectionString()
        {
            CloudStorageAccount storageAccount;
            storageAccount = CloudStorageAccount.Parse(StorageConnectionString);
            return storageAccount;
        }

        public static string StorageConnectionString;

        private static readonly string ContainerName = "FileResources";

        public static async Task<CloudTable> CreateTableAsync()
        {
            string tableName = "UserFiles";

            // Retrieve storage account information from connection string.
            CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString();

            // Create a table client for interacting with the table service
            Microsoft.Azure.Cosmos.Table.CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            // Create a table client for interacting with the table service 
            CloudTable table = tableClient.GetTableReference(tableName);
            _ = await table.CreateIfNotExistsAsync().ConfigureAwait(true);

            return table;
        }

        public static async Task WriteBlob(FileInfo fileInfo)
        {
            CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString();
            //CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            BlobServiceClient blobServiceClient = new BlobServiceClient(StorageConnectionString);
            BlobContainerClient container = await blobServiceClient.CreateBlobContainerAsync(ContainerName).ConfigureAwait(true);
            //CloudBlockBlob blob = container.GetBlockBlobReference("DSC03409.jpg");
            BlobClient blobClient = container.GetBlobClient($"{fileInfo.FileName}.{fileInfo.Token}");
            await blobClient.UploadAsync(fileInfo.FileStream);
        }

        public static async Task<Stream> GetBlob(FileInfo fileInfo)
        {
            CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString();
            BlobServiceClient blobServiceClient = new BlobServiceClient(StorageConnectionString);
            BlobContainerClient container = await blobServiceClient.CreateBlobContainerAsync(ContainerName);
            BlobClient blobClient = container.GetBlobClient($"{fileInfo.FileName}.{fileInfo.Token}");
            BlobDownloadInfo download = await blobClient.DownloadAsync();

            return download.Content;
        }
    }
    #endregion

    public class AzureTableUploader : IFileUploader
    {
        public void WriteFile(FileInfo fileInfo)
        {
            if(fileInfo==null)
            {
                throw new ArgumentNullException("fileInfo is null");
            }
            AzureTableBlobHelper.WriteBlob(fileInfo).Wait();
        }

        public FileInfo GetFile(string token)
        {
            TokenInfo tokenInfo = FileInfo.DecodeToken(token);
            FileInfo fileInfo = new FileInfo
            {
                FileName = tokenInfo.FileName,
                Token = token
            };

            fileInfo.FileStream = AzureTableBlobHelper.GetBlob(fileInfo).Result;

            return fileInfo;
        }
    }
}
