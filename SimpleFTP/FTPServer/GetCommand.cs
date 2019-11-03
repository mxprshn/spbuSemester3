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
            var fileInfo = new FileInfo(path);

            var data = File.ReadAllBytes(path);

            try
            {
                
                responseString = $"{fileInfo.Length} {Encoding.Default.GetString(data)}";

                //using (var reader = new StreamReader(new FileStream(path, FileMode.Open)))
                //{
                //    var content = await reader.ReadAllBytes();
                //    responseString = $"{fileInfo.Length} {content}";
                //}
            }
            catch (FileNotFoundException exception)
            {
                responseString = "-1";
            }
            finally
            {
                var writer = new BinaryWriter(stream);
                writer.Write(data, 0, (int)fileInfo.Length);
                writer.Flush();
            }
        }
    }
}
