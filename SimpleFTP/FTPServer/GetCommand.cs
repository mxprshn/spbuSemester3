using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FTPServer
{
    class GetCommand : IServerCommand
    {
        private string path;

        public GetCommand(string path)
        {
            this.path = path;
        }

        public async Task Execute(NetworkStream stream)
        {
            string responseString = "";

            try
            {
                var fileInfo = new FileInfo(path);

                using (var reader = new StreamReader(new FileStream(path, FileMode.Open)))
                {
                    var content = await reader.ReadToEndAsync();
                    responseString = $"{fileInfo.Length} {content}";
                }
            }
            catch (FileNotFoundException exception)
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
