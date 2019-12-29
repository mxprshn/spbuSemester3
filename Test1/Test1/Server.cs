using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Test1
{
    class Server
    {
        private readonly TcpListener listener;
        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();
        private List<TcpClient> clients = new List<TcpClient>();

        public Server(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
        }

        public async Task Run()
        {
            listener.Start();
            Console.WriteLine("Server is launched. Waiting for client.");

            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();
                clients.Add(client);
                Console.WriteLine("Client connected.");

                SendMessage(client.GetStream());
                GetMessage(client.GetStream());
            }
        }

        public void Shutdown()
        {
            foreach (var client in clients)
            {
                client.Close();
            }

            listener.Stop();
            Environment.Exit(0);
        }

        private void SendMessage(NetworkStream stream)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    var writer = new StreamWriter(stream);
                    var data = Console.ReadLine();
                    await writer.WriteLineAsync(data);
                    await writer.FlushAsync();

                    if (data == "exit")
                    {
                        Shutdown();
                    }
                }
            });
        }

        private void GetMessage(NetworkStream stream)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    var reader = new StreamReader(stream);
                    var data = await reader.ReadLineAsync();
                    Console.WriteLine(data);

                    if (data == "exit")
                    {
                        Shutdown();
                    }
                }
            });
        }
    }
}