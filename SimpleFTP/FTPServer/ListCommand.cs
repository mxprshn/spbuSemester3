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

        public ListCommand(string path)
        {
            this.path = path;
        }

        public async Task Execute(NetworkStream stream)
        {
            string responseString = "";

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

                // нужно наверное убрать пробел в конце
                responseString = $"{fileCount} {String.Join(" ", fileNames)} {String.Join(" ", dirNames)}";

            }
            catch (DirectoryNotFoundException exception)
            {
                responseString = "-1";
            }
            finally
            {
                var writer = new StreamWriter(stream);
                await writer.WriteLineAsync(responseString);
                writer.Flush();
            }
        }
    }
}