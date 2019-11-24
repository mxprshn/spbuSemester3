using System.Net.Sockets;

namespace FTPServer
{
    /// <summary>
    /// Network server interface for various tasks and protocols.
    /// </summary>
    public interface IServer
    {
        void Run();
        void Shutdown();
        void RequestDisconnection(TcpClient client);
    }
}