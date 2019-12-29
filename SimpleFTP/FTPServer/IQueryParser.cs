using System.Net.Sockets;

namespace FTPServer
{
    /// <summary>
    /// Protocol query parser interface.
    /// </summary>
    public interface IQueryParser
    {
        Server Server { get; set; }
        IServerCommand ParseQuery(string source, TcpClient client);
    }
}