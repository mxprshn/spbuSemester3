using System;
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

            await WaitForData(stream);

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

        private async Task WaitForData(NetworkStream stream)
        {
            var delay = TimeSpan.FromMilliseconds(100);

            for (var i = 0; i < 10; ++i)
            {
                if (stream.DataAvailable)
                {
                    return;
                }

                await Task.Delay(delay);
                delay *= 2;
            }
        }

        public async Task Send(string data)
        {
            var writer = new StreamWriter(tcpClient.GetStream(), Encoding.UTF8);
            await writer.WriteLineAsync(data);
            await writer.FlushAsync();
        }
    }
}