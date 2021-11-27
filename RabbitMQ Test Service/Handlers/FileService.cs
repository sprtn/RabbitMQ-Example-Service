using RabbitMQ_Test_Service.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RabbitMQ_Test_Service
{
    internal class FileService
    {
        private string incommingFolder;
        private string errorFolder;
        private string processedFolder;

        public FileService(string rootFolder)
        {
            incommingFolder = rootFolder + "\\incomming\\";
            errorFolder = incommingFolder + "\\error\\";
            processedFolder = incommingFolder + "\\processed\\";
        }
        
        internal List<SimpleMessage> ScrapeIncommingSimpleMessages()
        {
            var files = Directory.GetFiles(incommingFolder);
            List<SimpleMessage> simpleMessages = new List<SimpleMessage>();
            foreach (string path in files)
            {
                string fileName = path.Split('\\').Last();
                try
                {
                    var body = File.ReadAllText(path);
                    SimpleMessage simpleMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<SimpleMessage>(body);
                    simpleMessages.Add(simpleMessage);
                    File.Move(path, processedFolder + fileName + "_p");
                }
                catch
                {
                    File.Move(path, errorFolder + fileName + "_error");
                }
            }

            return simpleMessages;
        }

        internal void LocalStorage(SimpleMessage msg)
        {
            File.WriteAllText(errorFolder + msg.Header + "_f", Newtonsoft.Json.JsonConvert.SerializeObject(msg));
        }
    }
}