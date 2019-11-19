using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace FTPServer
{
    public class FileServer
    {
        private readonly TcpListener listener;
        private readonly IQueryParser parser;
        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();
        public bool IsRunning { get; private set; } = false;

        public FileServer(int port, IQueryParser parser)
        {
            listener = new TcpListener(IPAddress.Any, port);
            this.parser = parser;
        }

        public async Task Run()
        {
            listener.Start(); // исключения?)

            Console.WriteLine("Server is launched.");

            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();
                Console.WriteLine("New client connected.");
                HandleQuery(client);
            }
        }

        public void Shutdown()
        {
            if (!IsRunning)
            {
                throw new InvalidOperationException("Server was not running.");
            }

            Console.WriteLine("Stopping server...");
            tokenSource.Cancel();
            listener.Stop();
            IsRunning = false;
        }

        private void HandleQuery(TcpClient client)
        {
            var token = tokenSource.Token;

            Task.Run(async () =>
            {
                while (true)
                {
                    token.ThrowIfCancellationRequested();

                    try
                    {
                        var reader = new StreamReader(client.GetStream());
                        var data = await reader.ReadLineAsync();
                        Console.WriteLine(data);
                        await parser.ParseQuery(data).Execute(client.GetStream());
                    }
                    catch (Exception e) when ((e is InvalidOperationException || e is ObjectDisposedException
                            || e is IOException))
                    {
                        Console.WriteLine("Client disconnected.");
                        return;
                    }
                }

            }, token);
        }
    }
}