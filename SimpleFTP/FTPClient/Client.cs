using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FTPClient
{
    public class Client : IClient, IDisposable
    {
        private TcpClient tcpClient;
        private const int bufferSize = 1024;

        public Client(int port, string hostname = "localhost")
        {
            tcpClient = new TcpClient(hostname, port);
        }

        public void Dispose()
        {
            tcpClient.GetStream().Close();
            tcpClient.Close();
        }

        public async Task<byte[]> Receive()
        {
            var stream = tcpClient.GetStream();
            var result = new byte[0];

            // сделать более умный дилей
            await Task.Delay(100);

            while (stream.DataAvailable)
            {
                var data = new byte[bufferSize];
                var bytesRead = await stream.ReadAsync(data, 0, bufferSize);

                if (bytesRead < bufferSize)
                {
                    Array.Resize(ref data, bytesRead);
                }

                var previousLength = result.Length;
                Array.Resize(ref result, result.Length + data.Length);
                Array.Copy(data, 0, result, previousLength, data.Length);
            }

            return result;
        }

        public async Task Send(string data)
        {
            var writer = new StreamWriter(tcpClient.GetStream());
            await writer.WriteLineAsync(data);
            await writer.FlushAsync();
        }
    }
}