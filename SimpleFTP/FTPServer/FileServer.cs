using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text.RegularExpressions;

namespace FTPServer
{
    public class FileServer
    {
        private readonly TcpListener listener;

        public FileServer(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
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

        private void HandleQuery(TcpClient client)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    var reader = new StreamReader(client.GetStream());
                    var data = await reader.ReadLineAsync();
                    Console.WriteLine(data);
                    await ParseQuery(data).Execute(client.GetStream());
                }

            });
        }

        private IServerCommand ParseQuery(string data)
        {
            var match = Regex.Match(data, "(?<code>\\d) (?<path>.+)");
            
            switch (int.Parse(match.Groups["code"].Value))
            {
                case 1:
                {
                    return new ListCommand(match.Groups["path"].Value);
                }
                case 2:
                {
                    return new GetCommand(match.Groups["path"].Value);
                }
            }

            return null;
        }
    }
}