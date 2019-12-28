using Microsoft.Extensions.Configuration;
using System;

namespace FileUploadWebAP.Helpers
{
    public static class Utilities
    {
        public static IConfiguration Configuration = null;

        public static string ServerPath { get; set; }

        public static T GetValueFromAppSettingsAndByKey<T>(string key)
        {
            return Configuration.GetSection(key).Get<T>();
        }

        public static string GetValueByKey(string key)
        {
            return Configuration.GetValue<string>(key);
        }

        public static string ByteArrayToString(byte[] byteArray)
        {
            return BitConverter.ToString(byteArray).Replace("-", "");
        }

        public static byte[] StringToByteArray(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
    }
}
