using System.Threading.Tasks;

namespace Test1
{
    class Program
    {
        private static Server server;
        private static Client client;

        public static async Task Main(string[] args)
        {
            switch (args.Length)
            {
                case 1:
                    {
                        server = new Server(int.Parse(args[0]));
                        await server.Run();
                        break;
                    }
                case 2:
                    {
                        client = new Client(args[0], int.Parse(args[1]));
                        await client.Run();
                        break;
                    }
            }          
        }
    }
}
