using NUnit.Framework;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Text;

namespace FTPClient.Tests
{
    [TestFixture]
    public class ClientTests
    {
        private TcpListener listener;

        [TearDown]
        public void TearDown()
        {
            if (listener != null)
            {
                listener.Stop();
            }            
        }

        [Test, Combinatorial]
        public async Task SendTest([Values("ololo", "")] string data,
                [Values(0, 3, 7)] int repeatAmount)
        {
            listener = new TcpListener(IPAddress.Any, 8888);
            listener.Start();

            var task = Task.Run(async () =>
            {
                var client = await listener.AcceptTcpClientAsync();
                var reader = new StreamReader(client.GetStream());

                for (var i = 0; i < repeatAmount; ++i)
                {
                    var readData = await reader.ReadLineAsync();
                    Assert.AreEqual(data, readData);
                }
            });

            using(var client = new Client(8888, "localhost"))
            {
                for (var i = 0; i < repeatAmount; ++i)
                {
                    await client.Send(data);
                }

                task.Wait();
                listener.Stop();
            }
        }

        [Test]
        public void SendWithNoConnectionTest()
        {
            listener = new TcpListener(IPAddress.Any, 8888);
            listener.Start();
            using (var client = new Client(8888, "localhost"))
            {
                listener.Stop();
                Assert.ThrowsAsync<ConnectionToServerException>(() => client.Send("ololo"));
            }            
        }

        [TestCaseSource("ReceiveTestCases")]
        public async Task ReceiveTest(byte[] data, int repeatAmount)
        {
            var resetEvent = new AutoResetEvent(false);
            listener = new TcpListener(IPAddress.Any, 8888);
            listener.Start();

            var task = Task.Run(async () =>
            {
                var client = await listener.AcceptTcpClientAsync();

                for (var i = 0; i < repeatAmount; ++i)
                {
                    await client.GetStream().WriteAsync(data, 0, data.Length);
                    await client.GetStream().FlushAsync();
                    resetEvent.WaitOne();
                }                
            });

            using (var client = new Client(8888, "localhost"))
            {
                for (var i = 0; i < repeatAmount; ++i)
                {
                    var dataRead = await client.Receive();
                    var stringRead = Encoding.UTF8.GetString(dataRead);
                    var stringExpected = Encoding.UTF8.GetString(data);
                    Assert.AreEqual(stringExpected, stringRead);
                    resetEvent.Set();
                }

                task.Wait();
                listener.Stop();
            }
        }

        [Test]
        public void ReceiveWithNoConnectionTest()
        {
            listener = new TcpListener(IPAddress.Any, 8888);
            listener.Start();
            using (var client = new Client(8888, "localhost"))
            {
                listener.Stop();
                Assert.ThrowsAsync<ConnectionToServerException>(() => client.Receive());
            }
        }

        private static object[] ReceiveTestCases = 
        {
            new object[] { Encoding.UTF8.GetBytes("ololo"), 1 },
            new object[] { Encoding.UTF8.GetBytes("ololo"), 3 },
            new object[] { Encoding.UTF8.GetBytes("\n\n\n\n\n"), 1 },
            new object[] { Encoding.UTF8.GetBytes("\n\n\n\n\n"), 3 },
        };
    }
}