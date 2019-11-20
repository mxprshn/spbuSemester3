using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections.Concurrent;

namespace FTPServer
{
    public class FileServer
    {
        private readonly TcpListener listener;
        private readonly IQueryParser parser;
        private ConcurrentDictionary<TcpClient, (CancellationTokenSource tokenSource, string id)> clients = new ConcurrentDictionary<TcpClient, (CancellationTokenSource, string)>();
        private CancellationTokenSource parentTokenSource;
        private int idCounter;
        private bool isLaunched = false;

        public FileServer(int port, IQueryParser parser)
        {
            listener = new TcpListener(IPAddress.Any, port);
            this.parser = parser;
            parser.Server = this;
        }

        public void Run()
        {
            parentTokenSource = new CancellationTokenSource();
            listener.Start();
            Console.WriteLine("Server is launched.");
            isLaunched = true;

            Task.Run(async () =>
            {
                while (true)
                {
                    var client = await listener.AcceptTcpClientAsync();
                    ++idCounter;
                    clients.TryAdd(client, (CancellationTokenSource.CreateLinkedTokenSource(parentTokenSource.Token), idCounter.ToString()));
                    Console.WriteLine($"{idCounter.ToString()}: Client connected.");
                    HandleQuery(client);
                }
            });
        }

        public void Shutdown()
        {
            if (!isLaunched)
            {
                throw new InvalidOperationException("Server was not running.");
            }

            Console.WriteLine("Stopping server...");
            parentTokenSource.Cancel();
            listener.Stop();
            isLaunched = false;
        }

        private void HandleQuery(TcpClient client)
        {
            clients.TryGetValue(client, out var clientInfo);
            var token = clientInfo.tokenSource.Token;
            var id = clientInfo.id;

            Task.Run(async () =>
            {
                while (true)
                {
                    if (token.IsCancellationRequested)
                    {
                        Console.WriteLine($"{id}: Client disconnected.");
                        clients.TryRemove(client, out var source);
                        return;
                    }

                    try
                    {
                        var reader = new StreamReader(client.GetStream());
                        var data = await reader.ReadLineAsync();

                        Console.WriteLine($"{id}: {data}");

                        var command = parser.ParseQuery(data, client);

                        if (command != null)
                        {
                            await command.Execute();
                        }
                        
                    }
                    catch (Exception e) when (e is InvalidOperationException || e is ObjectDisposedException
                            || e is IOException)
                    {
                        Console.WriteLine($"{id}: Client disconnected with error: {e.Message}");
                        clients.TryRemove(client, out var source);
                        return;
                    }
                }

            }, token);
        }

        public void DisconnectClient(TcpClient client)
        {
            if (!clients.TryGetValue(client, out var clientInfo))
            {
                throw new InvalidOperationException("Client was not connected.");
            }

            clientInfo.tokenSource.Cancel();
        }
    }
}