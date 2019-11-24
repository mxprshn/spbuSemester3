using Moq;
using NUnit.Framework;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FTPServer.Tests
{
    [TestFixture]
    public class ServerTests
    {
        [Test]
        public async Task HandleQueryTest()
        {
            var mockParser = new Mock<IQueryParser>();
            var mockCommand = new Mock<IServerCommand>();
            mockParser.Setup(m => m.ParseQuery("ololo", It.IsAny<TcpClient>()))
                    .Returns(mockCommand.Object);

            var server = new Server(8888, mockParser.Object);

            server.Run();

            using (var client = new TcpClient("localhost", 8888))
            {
                var writer = new StreamWriter(client.GetStream());
                await writer.WriteLineAsync("ololo");
                await writer.FlushAsync();
                Thread.Sleep(100);
                mockCommand.Verify(m => m.Execute());
                server.Shutdown();
            }
        }
    }
}