using NUnit.Framework;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FTPServer.Tests
{
    [TestFixture]
    class GetCommandTests
    {
        [TestCase("ollolo")]
        [TestCase("")]
        [TestCase("We started living in an old house\n" +
                "My ma gave birth and we were checking it out\n" +
                "It was a baby boy, so we bought him a toy\n" +
                "It was a ray gun, and it was 1981\n")]
        public async Task ExecuteTest(string data)
        {
            var targetPath = $"{TestContext.CurrentContext.TestDirectory}.\\test2.txt";
            var listener = new TcpListener(IPAddress.Any, 8888);

            try
            {
                using (var writer = new StreamWriter(File.OpenWrite(targetPath)))
                {
                    await writer.WriteAsync(data);
                }

                listener.Start();

                using (var client = new TcpClient("localhost", 8888))
                {
                    var acceptedClient = listener.AcceptTcpClient();
                    var command = new GetCommand(targetPath, acceptedClient);
                    await command.Execute();

                    using (var md5 = MD5.Create())
                    {
                        var expectedString = $"{data.Length} {data}";
                        var buffer = new byte[expectedString.Length];
                        await client.GetStream().ReadAsync(buffer, 0, expectedString.Length);
                        var actual = md5.ComputeHash(buffer);
                        var expected = md5.ComputeHash(Encoding.UTF8.GetBytes($"{data.Length} {data}"));
                        CollectionAssert.AreEqual(expected, actual);
                    }
                }
            }
            finally
            {
                listener.Stop();
                File.Delete(targetPath);
            }
        }
    }
}
