using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using FTPServer;
using System.Threading.Tasks;

namespace FTPServer.Tests
{
    [TestFixture]
    class DisconnectCommandTests
    {
        [Test]
        public async Task ExecuteTest()
        {
            var mockFileServer = new Mock<IServer>();
            var mockClient = new Mock<TcpClient>();
            var command = new DisconnectCommand(mockFileServer.Object, mockClient.Object);
            await command.Execute();
            mockFileServer.Verify(m => m.RequestDisconnection(mockClient.Object));
        }
    }
}
