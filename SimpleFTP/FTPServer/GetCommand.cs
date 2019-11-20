﻿using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FTPServer
{
    class GetCommand : IServerCommand
    {
        private string path;
        private TcpClient client;

        public GetCommand(string path, TcpClient client)
        {
            this.path = path;
            this.client = client;
        }

        public async Task Execute()
        {
            var response = Encoding.UTF8.GetBytes("-2");
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
            catch (FileNotFoundException)
            {
                response = Encoding.UTF8.GetBytes("-1");
            }
            finally
            {
                var stream = client.GetStream();
                await stream.WriteAsync(response);
                await stream.FlushAsync();
            }
        }
    }
}