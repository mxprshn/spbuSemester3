using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FTPServer
{
    class DisconnectCommand : IServerCommand
    {
        private FileServer server;
        private TcpClient client;

        public DisconnectCommand(FileServer server, TcpClient client)
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
