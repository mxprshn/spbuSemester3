using System;
using System.Threading.Tasks;

namespace FTPClient
{
    /// <summary>
    /// Network client interface for various tasks and protocols.
    /// </summary>
    public interface IClient : IDisposable
    {
        bool IsConnected { get; }
        Task Send(string data);
        Task<byte[]> Receive();
    }
}