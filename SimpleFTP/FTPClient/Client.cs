using System;
using System.Collections.Generic;
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

        public Client(int port, string hostname = "localhost")
        {
            tcpClient = new TcpClient(hostname, port);
        }

        public void Dispose()
        {
            tcpClient.GetStream().Close();
            tcpClient.Close();
        }

        public async Task<string> Receive()
        {
            var reader = new StreamReader(tcpClient.GetStream());
            var data = await reader.ReadLineAsync();
            return data;
        }

        public async Task Send(string query)
        {
            var writer = new StreamWriter(tcpClient.GetStream());
            await writer.WriteLineAsync(query);
            await writer.FlushAsync();
        }
    }
}