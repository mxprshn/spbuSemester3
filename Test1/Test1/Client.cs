using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Test1
{
    class Client
    {
        private readonly TcpClient client;
        private NetworkStream stream;

        public Client(string hostname, int port)
        {
            client = new TcpClient(hostname, port);
        }

        public async Task Run()
        {
            stream = client.GetStream();
            Console.WriteLine("Client is launched.");

            GetMessage();
            await SendMessage();
        }

        private async Task SendMessage()
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
                    break;
                }
            }
        }

        private void GetMessage()
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
                        break;
                    }                    
                }
            });
        }

        private void Shutdown()
        {
            stream.Close();
            client.Close();
            Environment.Exit(0);
        }
    }
}
