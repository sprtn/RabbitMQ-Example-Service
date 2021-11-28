using Microsoft.Extensions.Configuration;
using RabbitMQ_Test_Service.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RabbitMQ_Test_Service
{
    internal enum FolderTypeEnum
    {
        Workspace,
        Incomming,
        Error,
        Processed
    }

    internal class FileService
    {
        private Dictionary<FolderTypeEnum, string> WorkFolders;

        public FileService(IConfiguration configuration)
        {
            WorkFolders = new Dictionary<FolderTypeEnum, string>();
            var Workspace = configuration["Workspace"];
            Workspace ??= Path.Combine(Environment.CurrentDirectory, "Workspace");
            WorkFolders.Add(FolderTypeEnum.Workspace, Workspace);
            WorkFolders.Add(FolderTypeEnum.Incomming, Path.Combine(Workspace, "Inc\\"));
            WorkFolders.Add(FolderTypeEnum.Processed, Path.Combine(Workspace, "Processed\\"));
            WorkFolders.Add(FolderTypeEnum.Error, Path.Combine(Workspace, "Err\\"));
            FoldersCheck();
        }

        public void FoldersCheck()
        {
            foreach (KeyValuePair<FolderTypeEnum, string> entry in WorkFolders)
                Directory.CreateDirectory(entry.Value);
        }

        internal void WriteToIncomming(byte[] body)
        {
            File.WriteAllBytes(Path.Combine(WorkFolders[FolderTypeEnum.Incomming], GenerateFilename("simp")), body);
        }

        private string GenerateFilename(string suffix)
        {
            if (suffix[0] == '.') suffix.Remove(0);
            return $"Received-{DateTime.Now.Ticks}.{suffix}";
        }

        internal List<SimpleMessage> ScrapeIncommingSimpleMessages()
        {
            var filePaths = Directory.GetFiles(WorkFolders[FolderTypeEnum.Incomming]);
            List<SimpleMessage> simpleMessages = new();
            foreach (string filePath in filePaths)
            {
                try
                {
                    var body = File.ReadAllText(filePath);
                    simpleMessages.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<SimpleMessage>(body));
                    MoveToProcessed(filePath);
                }
                catch
                {
                    MoveToError(filePath);
                }
            }

            return simpleMessages;
        }

        private static string GetFileName(string path)
        {
            return path.Split('\\').Last();
        }

        private static string GetSuffix(string path)
        {
            return path.Split('.').Last();
        }

        private void MoveToError(string path)
        {
            File.Move(path, Path.Combine(WorkFolders[FolderTypeEnum.Error], GetFileName(path), "simperr"));
        }

        private void MoveToProcessed(string path)
        {
            File.Move(path, Path.Combine(WorkFolders[FolderTypeEnum.Processed], GetFileName(path)));
        }

        internal void SaveToError(SimpleMessage msg)
        {
            File.WriteAllText(Path.Combine(WorkFolders[FolderTypeEnum.Error], GenerateFilename(".simperr")), Newtonsoft.Json.JsonConvert.SerializeObject(msg));
        }
    }
}
