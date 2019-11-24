using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FTPServer
{
    public interface IServer
    {
        Task Run();
        void RequestDisconnection(TcpClient client);
    }
}
