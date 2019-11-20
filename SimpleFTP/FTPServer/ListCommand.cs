using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FTPServer
{
    class ListCommand : IServerCommand
    {
        private string path;
        private TcpClient client;

        public ListCommand(string path, TcpClient client)
        {
            this.path = path;
            this.client = client;
        }

        public async Task Execute()
        {
            string responseString = "-2";

            try
            {
                var fileNames = Directory.GetFiles(path);
                var dirNames = Directory.GetDirectories(path);
                var fileCount = fileNames.Length + dirNames.Length;

                for (var i = 0; i < fileNames.Length; ++i)
                {
                    fileNames[i] += " false";
                }

                for (var i = 0; i < dirNames.Length; ++i)
                {
                    dirNames[i] += " true";
                }

                var filesString = $"{(fileNames.Length == 0 ? "" : " ")}{String.Join(" ", fileNames)}";
                var dirsString = $"{(dirNames.Length == 0 ? "" : " ")}{String.Join(" ", dirNames)}";
                responseString = $"{fileCount}{dirsString}";

            }
            catch (DirectoryNotFoundException)
            {
                responseString = "-1";
            }
            finally
            {
                var writer = new StreamWriter(client.GetStream());
                await writer.WriteLineAsync(responseString);
                await writer.FlushAsync();
            }
        }
    }
}