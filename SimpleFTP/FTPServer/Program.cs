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
        static async Task Main(string[] args)
        {
            var server = new FileServer(8888, new FileQueryParser());
            await server.Run();
        }
    }
}