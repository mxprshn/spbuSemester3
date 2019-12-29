using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FTPServer
{
    /// <summary>
    /// Requests a particular client disconnection from server. 
    /// </summary>
    public class DisconnectCommand : IServerCommand
    {
        private IServer server;
        private TcpClient client;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="server">Server with client connected.</param>
        /// <param name="client">Client to disconnect.</param>
        public DisconnectCommand(IServer server, TcpClient client)
        {
            this.server = server;
            this.client = client;
        }

        /// <summary>
        /// Requests a particular client disconnection from server. 
        /// </summary>
        public async Task Execute()
        {
            server.RequestDisconnection(client);
        }
    }
}
