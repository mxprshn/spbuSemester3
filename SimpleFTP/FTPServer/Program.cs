using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            FileServer server = null;
            server = new FileServer(8888, new FileQueryParser());
            server.Run();

            var key = Console.ReadKey();
            server.Shutdown();
            var key1 = Console.ReadKey();
        }
    }
}