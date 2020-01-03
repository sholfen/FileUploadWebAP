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

    public class FileInfoEntity : TableEntity
    {

        public FileInfoEntity(FileInfo fileInfo)
        {
            PartitionKey = "FileInfo";
            RowKey = fileInfo.Token;
            FileName = fileInfo.FileName;
            Token = fileInfo.Token;
            Version = fileInfo.Version;
        }

        public string FileName { get; set; }
        public string Token { get; set; }
        public string Version { get; set; }
    }

    public static class AzureTableBlobHelper
    {
        //static AzureTableBlobHelper()
        //{
        //    CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString();
        //    BlobServiceClient blobServiceClient = new BlobServiceClient(StorageConnectionString);
        //    bool isExist = false;
        //    foreach (var item in blobServiceClient.GetBlobContainers())
        //    {
               
        //        if (ContainerName == item.Name)
        //        {
        //            isExist = true;
        //            break;
        //        }
        //    }
        //    if(!isExist)
        //    {
        //        _ = blobServiceClient.CreateBlobContainer(ContainerName);
        //    }
        //}

        private static CloudStorageAccount CreateStorageAccountFromConnectionString()
        {
            CloudStorageAccount storageAccount;
            storageAccount = CloudStorageAccount.Parse(StorageConnectionString);
            return storageAccount;
        }

        public static string StorageConnectionString;

        private static readonly string ContainerName = "fileresources";

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

        public static async Task WriteTable(FileInfo fileInfo)
        {
            CloudTable table = await CreateTableAsync();
            FileInfoEntity entity = new FileInfoEntity(fileInfo);
            // Create the InsertOrReplace table operation
            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);
            // Execute the operation.
            TableResult result = await table.ExecuteAsync(insertOrMergeOperation).ConfigureAwait(true);
        }

        public static async Task WriteBlob(FileInfo fileInfo)
        {
            CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString();
            //CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            BlobServiceClient blobServiceClient = new BlobServiceClient(StorageConnectionString);

            bool isExist = false;
            foreach (var item in blobServiceClient.GetBlobContainers())
            {

                if (ContainerName == item.Name)
                {
                    isExist = true;
                    break;
                }
            }
            if (!isExist)
            {
                _ = blobServiceClient.CreateBlobContainer(ContainerName);
            }

            BlobContainerClient container = blobServiceClient.GetBlobContainerClient(ContainerName);
            //CloudBlockBlob blob = container.GetBlockBlobReference("DSC03409.jpg");
            BlobClient blobClient = container.GetBlobClient($"{fileInfo.FileName}.{fileInfo.Token}");
            await blobClient.UploadAsync(fileInfo.FileStream).ConfigureAwait(true);
        }

        public static async Task<Stream> GetBlob(FileInfo fileInfo)
        {
            CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString();
            BlobServiceClient blobServiceClient = new BlobServiceClient(StorageConnectionString);
            BlobContainerClient container = blobServiceClient.GetBlobContainerClient(ContainerName);
            BlobClient blobClient = container.GetBlobClient($"{fileInfo.FileName}.{fileInfo.Token}");
            BlobDownloadInfo download = await blobClient.DownloadAsync().ConfigureAwait(true);

            return download.Content;
        }
    }
    #endregion

    public class AzureTableUploader : IFileUploader
    {
        public AzureTableUploader()
        {

        }

        public void WriteFile(FileInfo fileInfo)
        {
            if (fileInfo == null)
            {
                throw new ArgumentNullException("fileInfo is null");
            }
            //write table
            AzureTableBlobHelper.WriteTable(fileInfo).Wait();

            //write file
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
