using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace FTPServer
{
    public class FileServer
    {
        private readonly TcpListener listener;
        private readonly IQueryParser parser;
        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();

        public FileServer(int port, IQueryParser parser)
        {
            listener = new TcpListener(IPAddress.Any, port);
            this.parser = parser;
        }

        public async Task Run()
        {
            listener.Start();
            Console.WriteLine("Server is launched. Press F to pay respects.");
            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();
                HandleQuery(client);
            }
        }

        public void Shutdown()
        {

        }

        private void HandleQuery(TcpClient client)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    var reader = new StreamReader(client.GetStream());
                    var data = await reader.ReadLineAsync();
                    Console.WriteLine(data);
                    await parser.ParseQuery(data).Execute(client.GetStream());
                }

            }, tokenSource.Token);
        }
    }
}