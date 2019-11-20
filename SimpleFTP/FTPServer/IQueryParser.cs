using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FTPServer
{
    public interface IQueryParser
    {
        FileServer Server { get; set; }

        IServerCommand ParseQuery(string source, TcpClient client);
    }
}