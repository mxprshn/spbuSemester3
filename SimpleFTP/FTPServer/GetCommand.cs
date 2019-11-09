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
            var response = new byte[0];
            var fileInfo = new FileInfo(path);

            try
            {
                var content = new byte[fileInfo.Length];
                var header = Encoding.UTF8.GetBytes($"{fileInfo.Length} ");

                using (var fileStream = new FileStream(path, FileMode.Open))
                {
                    await fileStream.ReadAsync(content);
                }

                Array.Resize(ref response, content.Length + header.Length);
                Array.Copy(header, 0, response, 0, header.Length);
                Array.Copy(content, 0, response, header.Length, content.Length);
            }
            catch (FileNotFoundException exception)
            {
                response = Encoding.UTF8.GetBytes("-1");
            }
            finally
            {
                await stream.WriteAsync(response);
                await stream.FlushAsync();
            }
        }
    }
}
