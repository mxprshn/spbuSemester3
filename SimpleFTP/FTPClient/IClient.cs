using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTPClient
{
    public interface IClient : IDisposable
    {
        bool IsConnected { get; }
        Task Send(string data);
        Task<byte[]> Receive();
    }
}