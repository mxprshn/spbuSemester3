using System.Threading.Tasks;

namespace FTPServer
{
    /// <summary>
    /// Interface of a command which can be executed by a server.
    /// </summary>
    public interface IServerCommand
    {
        Task Execute();
    }
}