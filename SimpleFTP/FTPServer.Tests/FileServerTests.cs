using Moq;
using NUnit.Framework;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FTPServer.Tests
{
    public class FileServerTests
    {
        [Test]
        public void HandleQueryTest()
        {
            var mockParser = new Mock<IQueryParser>();
            var mockCommand = new Mock<IServerCommand>();
            mockParser.Setup(m => m.ParseQuery(It.IsAny<string>(), It.IsAny<TcpClient>()))
                    .Returns(mockCommand.Object);

            var server = new FileServer(8888, mockParser.Object);

            var task = Task.Run(async () =>
            {
                await server.Run();
            });

            using (var client = new TcpClient("localhost", 8888))
            {
                var writer = new StreamWriter(client.GetStream());
                writer.WriteLine("ololo");
                mockCommand.Verify(m => m.Execute());
            }

            task.
        }
    }
}