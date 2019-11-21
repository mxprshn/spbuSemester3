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

        public FileServer(int port, IQueryParser parser)
        {
            listener = new TcpListener(IPAddress.Any, port);
            this.parser = parser;
            parser.Server = this;
        }

        public async Task Run()
        {
            parentTokenSource = new CancellationTokenSource();
            listener.Start();
            Console.WriteLine("Server is launched.");

            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();
                ++idCounter;
                clients.TryAdd(client, (CancellationTokenSource.CreateLinkedTokenSource(parentTokenSource.Token), idCounter.ToString()));
                Console.WriteLine($"{idCounter.ToString()}: Client connected.");
                HandleQuery(client);
            }
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

        public void RequestDisconnection(TcpClient client)
        {
            if (!clients.TryGetValue(client, out var clientInfo))
            {
                throw new InvalidOperationException("Client was not connected.");
            }

            clientInfo.tokenSource.Cancel();
        }
    }
}