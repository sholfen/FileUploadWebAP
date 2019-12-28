using FileUploadWebAP.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FileUploadWebAP.libs.FilesSaver
{
    public class TokenInfo
    {
        public string Id { get; set; }
        public string Guid { get; set; }
        public string FileName { get; set; }
    }

    public class FileInfo
    {
        public string FileName { get; set; }
        public Stream FileStream { get; set; }
        public string Token { get; set; }
        public string Version { get; set; }

        private readonly static AesCryptoServiceProvider AESProvider = new AesCryptoServiceProvider();

        public static string CreateToken(string userId, string fileName)
        {
            byte[] key = Encoding.ASCII.GetBytes("1234567812345678");
            byte[] iv = Encoding.ASCII.GetBytes("8765432187654321");
            AESProvider.IV = iv;
            AESProvider.Key = key;

            string guid = Guid.NewGuid().ToString();
            TokenInfo tokenInfo = new TokenInfo
            {
                Id = userId,
                FileName = fileName,
                Guid = guid
            };
            string jsonStr = System.Text.Json.JsonSerializer.Serialize(tokenInfo);
            byte[] source = Encoding.UTF8.GetBytes(jsonStr);
            ICryptoTransform cryptoTransform = AESProvider.CreateEncryptor();
            byte[] finalBytes = cryptoTransform.TransformFinalBlock(source, 0, source.Length);
            //string encodeStr = Convert.ToBase64String(finalBytes);
            string encodeStr = Utilities.ByteArrayToString(finalBytes);

            return encodeStr;
        }
        public static TokenInfo DecodeToken(string encodeStr)
        {
            TokenInfo tokenInfo = null;

            //decrypt
            byte[] key = Encoding.ASCII.GetBytes("1234567812345678");
            byte[] iv = Encoding.ASCII.GetBytes("8765432187654321");
            AESProvider.IV = iv;
            AESProvider.Key = key;
            ICryptoTransform decryptoTransform = AESProvider.CreateDecryptor();

            //byte[] dataBytes = Convert.FromBase64String(encodeStr);
            byte[] dataBytes = Utilities.StringToByteArray(encodeStr);
            string jsonStr = string.Empty;
            byte[] finalBytes = decryptoTransform.TransformFinalBlock(dataBytes, 0, dataBytes.Length);
            string decodeStr = Encoding.UTF8.GetString(finalBytes);
            tokenInfo = System.Text.Json.JsonSerializer.Deserialize<TokenInfo>(decodeStr);

            return tokenInfo;
        }
    }

    public interface IFileUploader
    {
        void WriteFile(FileInfo fileInfo);
        FileInfo GetFile(string token);
    }
}
