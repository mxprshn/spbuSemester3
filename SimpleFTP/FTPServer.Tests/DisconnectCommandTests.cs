using Moq;
using NUnit.Framework;
using System.Net.Sockets;
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
            var client = new TcpClient();
            var command = new DisconnectCommand(mockFileServer.Object, client);
            await command.Execute();
            mockFileServer.Verify(m => m.RequestDisconnection(client));
        }
    }
}
