using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FTPServer
{
    public class DisconnectCommand : IServerCommand
    {
        private IServer server;
        private TcpClient client;

        public DisconnectCommand(IServer server, TcpClient client)
        {
            this.server = server;
            this.client = client;
        }

        public async Task Execute()
        {
            server.RequestDisconnection(client);
        }
    }
}
