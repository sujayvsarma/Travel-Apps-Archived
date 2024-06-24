using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.File;

using SujaySarma.Sdk.AspNetCore.Mvc;
using SujaySarma.Sdk.DataSources.AzureTables;

using System.IO;

namespace TravelIdeasPortalWeb.ServiceClients
{
    /// <summary>
    /// Client that abstracts and provides services for File management and IO
    /// </summary>
    public static class FileServicesClient
    {

        /// <summary>
        /// Read a previously stored session file
        /// </summary>
        /// <param name="sessionId">Session ID</param>
        /// <param name="fileName">Name of file</param>
        /// <returns>Data from the file as string - if file was not found, returns NULL</returns>
        public static string ReadFile(string sessionId, string fileName)
        {
            if (enableAzureFileServices)
            {
                CloudFile file = EnsureAzureSessionFolder(sessionId)
                    .GetFileReference($"{fileName}.json");

                if (file.Exists())
                {
                    using StreamReader reader = new StreamReader(file.OpenRead());
                    return reader.ReadToEnd();
                }
            }
            else
            {
                DirectoryInfo directory = EnsureLocalSessionFolder(sessionId);
                FileInfo file = new FileInfo(Path.Combine(directory.FullName, $"{fileName}.json"));
                if (file.Exists)
                {
                    using StreamReader reader = new StreamReader(file.OpenRead());
                    return reader.ReadToEnd();
                }
            }

            return null;
        }

        /// <summary>
        /// Store a session file
        /// </summary>
        /// <param name="sessionId">Session ID</param>
        /// <param name="fileName">Name of file</param>
        /// <param name="data">Data to store</param>
        public static void StoreFile(string sessionId, string fileName, string data)
        {
            if (enableAzureFileServices)
            {
                CloudFile file = EnsureAzureSessionFolder(sessionId)
                    .GetFileReference($"{fileName}.json");

                using CloudFileStream stream = file.OpenWrite(data.Length);
                stream.Write(System.Text.Encoding.UTF8.GetBytes(data), 0, data.Length);
                stream.Flush();

                file = null;
            }
            else
            {
                DirectoryInfo directory = EnsureLocalSessionFolder(sessionId);
                FileInfo file = new FileInfo(Path.Combine(directory.FullName, $"{fileName}.json"));

                using StreamWriter stream = new StreamWriter(file.FullName, false, System.Text.Encoding.UTF8);
                stream.Write(data);
                stream.Flush();

                file = null;
                directory = null;
            }
        }

        /// <summary>
        /// Ensure that the session folder exists on local disk
        /// </summary>
        /// <param name="sessionId">Session Id</param>
        /// <returns>Reference to the session directory</returns>
        private static DirectoryInfo EnsureLocalSessionFolder(string sessionId)
        {
            string path = Path.Combine(AppSettingsJson.Environment.ContentRootPath, "storage", "sessionLogs", sessionId);
            DirectoryInfo directory = new DirectoryInfo(path);
            if (!directory.Exists)
            {
                directory = sessionLogsDirectoryLocal.CreateSubdirectory(sessionId);
            }

            return directory;
        }

        /// <summary>
        /// Ensure the session folder exists on Azure FS
        /// </summary>
        /// <param name="sessionId">Session Id</param>
        /// <returns>Reference to the session directory</returns>
        private static CloudFileDirectory EnsureAzureSessionFolder(string sessionId)
        {
            CloudFileDirectory directory = sessionLogsDirectoryAzure.GetDirectoryReference(sessionId);
            directory.CreateIfNotExists();

            return directory;
        }


        static FileServicesClient()
        {
            storageConnectionString = AppSettingsJson.Configuration.GetSection("ConnectionStrings")["fileStorage"];

            AzureStorageAccount storageAccount = new AzureStorageAccount(storageConnectionString);
            enableAzureFileServices = (!storageAccount.IsDevelopmentStorageAccount);
            if (enableAzureFileServices)
            {
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(storageConnectionString);
                CloudFileClient fileClient = cloudStorageAccount.CreateCloudFileClient();

                CloudFileShare drive = fileClient.GetShareReference("travelideasportalweb");
                drive.CreateIfNotExists();

                CloudFileDirectory root = drive.GetRootDirectoryReference();
                root.CreateIfNotExists();

                sessionLogsDirectoryAzure = root.GetDirectoryReference("sessionLogs");
                sessionLogsDirectoryAzure.CreateIfNotExists();
            }
            else
            {
                string path = Path.Combine(AppSettingsJson.Environment.ContentRootPath, "storage", "sessionLogs");
                sessionLogsDirectoryLocal = new DirectoryInfo(path);
                if (!sessionLogsDirectoryLocal.Exists)
                {
                    sessionLogsDirectoryLocal = (new DirectoryInfo(AppSettingsJson.Environment.ContentRootPath)).CreateSubdirectory(Path.Combine("storage", "sessionLogs"));
                }
            }
        }
        private static readonly string storageConnectionString;
        private static readonly CloudFileDirectory sessionLogsDirectoryAzure;
        private static readonly DirectoryInfo sessionLogsDirectoryLocal;
        private static readonly bool enableAzureFileServices = false;
    }
}
