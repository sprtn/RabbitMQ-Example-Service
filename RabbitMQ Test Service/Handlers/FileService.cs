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
            Workspace ??= Environment.CurrentDirectory + "\\Workspace\\";
            WorkFolders.Add(FolderTypeEnum.Workspace, Workspace);
            WorkFolders.Add(FolderTypeEnum.Incomming, Workspace + "\\Inc\\");
            WorkFolders.Add(FolderTypeEnum.Processed, Workspace + "\\Processed\\");
            WorkFolders.Add(FolderTypeEnum.Error, Workspace + "\\Err\\");
            FoldersCheck();
        }

        public void FoldersCheck()
        {
            foreach (KeyValuePair<FolderTypeEnum, string> entry in WorkFolders)
            {
                var pathArr = entry.Value.Split('\\');
                for (int i = 1; i <= pathArr.Length; i++)
                {
                    var path = string.Join("\\", pathArr.Take(i));
                    if (path.Length == 2) continue;
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                }
            }
        }

        internal void WriteToIncomming(byte[] body)
        {
            File.WriteAllBytes($"{WorkFolders[FolderTypeEnum.Incomming]}\\{GenerateFilename("simp")}", body);
        }

        private string GenerateFilename(string suffix)
        {
            if (suffix[0] == '.')
                suffix.Remove(0);
            return $"Received-{DateTime.Now.Ticks}.{suffix}";
        }

        internal List<SimpleMessage> ScrapeIncommingSimpleMessages()
        {
            var files = Directory.GetFiles(WorkFolders[FolderTypeEnum.Incomming]);
            List<SimpleMessage> simpleMessages = new();
            foreach (string path in files)
            {
                try
                {
                    var body = File.ReadAllText(path);
                    simpleMessages.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<SimpleMessage>(body));
                    MoveToProcessed(path);
                }
                catch
                {
                    MoveToError(path);
                }
            }

            return simpleMessages;
        }

        private static string GetFileName(string path)
        {
            return path.Split('\\').Last();
        }

        private void MoveToError(string path)
        {
            File.Move(path, WorkFolders[FolderTypeEnum.Error] + GetFileName(path) + "simperr");
        }

        private void MoveToProcessed(string path)
        {
            File.Move(path, WorkFolders[FolderTypeEnum.Processed] + GetFileName(path));
        }

        internal void SaveToError(SimpleMessage msg)
        {
            File.WriteAllText($"{WorkFolders[FolderTypeEnum.Error]}\\{GenerateFilename(".simperr")}", Newtonsoft.Json.JsonConvert.SerializeObject(msg));
        }
    }
}
