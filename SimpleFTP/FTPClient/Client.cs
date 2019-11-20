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
        private const string defaultHost = "localhost";
        public bool IsConnected => tcpClient.Connected;

        public Client(int port, string hostname = defaultHost)
        {
            try
            {
                tcpClient = new TcpClient(hostname, port);
            }
            catch (SocketException e)
            {
                throw new ConnectionToServerException(e.Message, e);
            }
        }

        public void Dispose()
        {
            tcpClient.Close();
            tcpClient.Dispose();
        }

        public async Task<byte[]> Receive()
        {
            var stream = tcpClient.GetStream();
            var result = new byte[0];
            var bufferSize = tcpClient.ReceiveBufferSize;

            try
            {
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
            }
            catch (Exception e) when (e is SocketException || e is IOException)
            {
                throw new ConnectionToServerException(e.Message, e);
            }

            return result;
        }

        private async Task WaitForData(NetworkStream stream)
        {
            var delay = TimeSpan.FromMilliseconds(100);

            for (var i = 0; i < 10; ++i)
            {
                try
                {
                    if (stream.DataAvailable)
                    {
                        return;
                    }
                }
                catch { }

                await Task.Delay(delay);
                delay *= 2;
            }
        }

        private async Task TryToWriteData(StreamWriter writer, string data)
        {
            var delay = TimeSpan.FromMilliseconds(100);

            for (var i = 0; i < 10; ++i)
            {
                try
                {
                    await writer.WriteLineAsync(data);
                    await writer.FlushAsync();
                    return;
                }
                catch { }

                await Task.Delay(delay);
                delay *= 2;
            }
        }

        public async Task Send(string data)
        {
            var writer = new StreamWriter(tcpClient.GetStream(), Encoding.UTF8);

            try
            {
                await TryToWriteData(writer, data);
            }
            catch (Exception e) when (e is SocketException || e is IOException)
            {
                throw new ConnectionToServerException(e.Message, e);
            }
        }
    }
}