using NUnit.Framework;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FTPServer.Tests
{
    [TestFixture]
    class ListCommandTests
    {
        [Test]
        public async Task ExecuteTest()
        {
            var targetPath = $"{TestContext.CurrentContext.TestDirectory}\\test";
            var listener = new TcpListener(IPAddress.Any, 8888);
            listener.Start();

            try
            { 
                Directory.CreateDirectory(targetPath);
                File.Create($"{targetPath}\\test1.txt");
                File.Create($"{targetPath}\\test2.txt");
                File.Create($"{targetPath}\\test3.txt");
                File.Create($"{targetPath}\\test4.txt");
                Directory.CreateDirectory($"{targetPath}\\test1");
                Directory.CreateDirectory($"{targetPath}\\test2");
                Directory.CreateDirectory($"{targetPath}\\test3");

                using (var client = new TcpClient("localhost", 8888))
                {
                    var acceptedClient = listener.AcceptTcpClient();
                    var command = new ListCommand(targetPath, acceptedClient);
                    await command.Execute();

                    var reader = new StreamReader(client.GetStream());
                    var actual = await reader.ReadLineAsync();
                    Assert.AreEqual($"7 {targetPath}\\test1.txt false {targetPath}\\test2.txt false {targetPath}\\test3.txt false " +
                        $"{targetPath}\\test4.txt false {targetPath}\\test1 true {targetPath}\\test2 true {targetPath}\\test3 true", actual);
                }
            }
            finally
            {
                listener.Stop();
            }
        }

        [Test]
        public async Task ExecuteDirNotExistTest()
        {
            var targetPath = $"{TestContext.CurrentContext.TestDirectory}\\ololo";
            var listener = new TcpListener(IPAddress.Any, 8888);
            listener.Start();

            try
            {
                using (var client = new TcpClient("localhost", 8888))
                {
                    var acceptedClient = listener.AcceptTcpClient();
                    var command = new ListCommand(targetPath, acceptedClient);
                    await command.Execute();

                    var reader = new StreamReader(client.GetStream());
                    var actual = await reader.ReadLineAsync();
                    Assert.AreEqual("-1", actual);
                }
            }
            finally
            {
                listener.Stop();
            }
        }
    }
}
