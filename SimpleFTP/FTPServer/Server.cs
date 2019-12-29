using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections.Concurrent;

namespace FTPServer
{
    /// <summary>
    /// Class implementing a network server for various tasks and protocols.
    /// </summary>
    public class Server : IServer
    {
        private readonly TcpListener listener;
        private readonly IQueryParser parser;
        private ConcurrentDictionary<TcpClient, (CancellationTokenSource tokenSource, string id)> clients = new ConcurrentDictionary<TcpClient, (CancellationTokenSource, string)>();
        private CancellationTokenSource parentTokenSource;
        private Task mainTask;
        private int idCounter;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="port">The port on which to listen for incoming connection attempts.></param>
        /// <param name="parser">Parser used to parse incoming queries.</param>
        public Server(int port, IQueryParser parser)
        {
            listener = new TcpListener(IPAddress.Any, port);
            this.parser = parser;
            parser.Server = this;
        }

        /// <summary>
        /// Runs server listening to incoming connections.
        /// </summary>
        public void Run()
        {
            listener.Start();
            parentTokenSource = new CancellationTokenSource();

            mainTask = Task.Run(async () =>
            {
                while (true)
                {
                    while (!listener.Pending())
                    {
                        if (parentTokenSource.IsCancellationRequested)
                        {
                            return;
                        }
                    }

                    var client = await listener.AcceptTcpClientAsync();
                    ++idCounter;
                    clients.TryAdd(client, (CancellationTokenSource.CreateLinkedTokenSource(parentTokenSource.Token), idCounter.ToString()));
                    Console.WriteLine($"{idCounter.ToString()}: Client connected.");
                    HandleQuery(client);
                }
            }, parentTokenSource.Token);
        }

        /// <summary>
        /// Closes the server for new connections. Remaining clients are disconnected after pending query.
        /// </summary>
        public void Shutdown()
        {
            parentTokenSource.Cancel();
            mainTask.Wait();
            listener.Stop();
            idCounter = 0;
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