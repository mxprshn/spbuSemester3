using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FTPClient
{
    /// <summary>
    /// Class implementing a network client for various tasks and protocols.
    /// </summary>
    public class Client : IClient, IDisposable
    {
        private TcpClient tcpClient;
        private const string defaultHost = "localhost";

        /// <summary>
        /// Gets a value indicating whether client is connected to a remote host after the most recent operation.
        /// </summary>
        public bool IsConnected => tcpClient.Connected;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="port">The port number of the remote host to which you intend to connect.</param>
        /// <param name="hostname">The DNS name of the remote host to which you intend to connect.</param>
        /// <exception cref="ConnectionToServerException">Thrown in case of connection error.</exception>
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

        /// <summary>
        /// Gets bytes of data from server asynchronously.
        /// </summary>
        /// <returns>Read bytes.</returns>
        /// <exception cref="ConnectionToServerException">Thrown in case of connection error.</exception>
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

        /// <summary>
        /// Sends data to server asynchronously.
        /// </summary>
        /// <param name="data">Data to send.</param>
        /// <exception cref="ConnectionToServerException">Thrown in case of connection error.</exception>
        public async Task Send(string data)
        {
            try
            {
                var writer = new StreamWriter(tcpClient.GetStream());
                await TryToWriteData(writer, data);
            }
            catch (Exception e) when (e is SocketException || e is IOException)
            {
                throw new ConnectionToServerException(e.Message, e);
            }
        }

        public void Dispose()
        {
            tcpClient.Close();
            tcpClient.Dispose();
        }

        private async Task WaitForData(NetworkStream stream)
        {
            var delay = TimeSpan.FromMilliseconds(100);

            for (var i = 0; i < 5; ++i)
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

            throw new ConnectionToServerException("Server connection timeout.");
        }

        private async Task TryToWriteData(StreamWriter writer, string data)
        {
            var delay = TimeSpan.FromMilliseconds(100);

            for (var i = 0; i < 5; ++i)
            {
                try
                {
                    await WriteData(writer, data);
                    return;
                }
                catch { }

                await Task.Delay(delay);
                delay *= 2;
            }

            await WriteData(writer, data);
        }

        private async Task WriteData(StreamWriter writer, string data)
        {
            await writer.WriteLineAsync(data);
            await writer.FlushAsync();
        }
    }
}