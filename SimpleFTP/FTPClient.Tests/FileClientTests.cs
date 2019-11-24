using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace FTPClient.Tests
{
    [TestFixture]
    class FileClientTests
    {
        [TestCaseSource("ListTestCases")]
        public async Task ListTest(string response, IList<FileInformation> expected)
        {
            var mockClient = new Mock<IClient>();
            mockClient.Setup(m => m.Receive()).Returns(Task.FromResult(Encoding.UTF8.GetBytes(response)));
            var client = new FileClient(mockClient.Object);
            var result = await client.List("C:\\ololo");
            mockClient.Verify(m => m.Send("1 C:\\ololo"));
            CollectionAssert.AreEquivalent(expected, result);
        }

        [TestCase("ololo")]
        [TestCase("\n\n\n\n\n\n\n")]
        [TestCase("")]
        [TestCase("\tololo31415\t")]
        public async Task GetTest(string response)
        {
            var mockClient = new Mock<IClient>();
            mockClient.Setup(m => m.Receive())
                    .Returns(Task.FromResult(Encoding.UTF8.GetBytes($"{response.Length} {response}")));
            var client = new FileClient(mockClient.Object);
            var sourcePath = "U:\\test.txt";
            var targetPath = $"{TestContext.CurrentContext.TestDirectory}.\\test.txt";

            try
            {
                await client.Get(sourcePath, targetPath);

                using (var md5 = MD5.Create())
                {
                    var expected = md5.ComputeHash(Encoding.UTF8.GetBytes(response));

                    using (var stream = File.OpenRead(targetPath))
                    {
                        var actual = md5.ComputeHash(stream);
                        CollectionAssert.AreEqual(expected, actual);
                    }
                }

                mockClient.Verify(m => m.Send($"2 {sourcePath}"));
            }
            finally
            {
                File.Delete(targetPath);
            }            
        }

        [Test]
        public void ListDirectoryNotExistTest()
        {
            var mockClient = new Mock<IClient>();
            mockClient.Setup(m => m.Receive()).Returns(Task.FromResult(Encoding.UTF8.GetBytes("-1")));
            var client = new FileClient(mockClient.Object);
            Assert.ThrowsAsync<DirectoryNotFoundException>(() => client.List("C:\\ololo"));
        }

        [Test]
        public void ListInvalidDirectoryTest()
        {
            var mockClient = new Mock<IClient>();
            mockClient.Setup(m => m.Receive()).Returns(Task.FromResult(Encoding.UTF8.GetBytes("-2")));
            var client = new FileClient(mockClient.Object);
            Assert.ThrowsAsync<ArgumentException>(() => client.List("C:\\ololo"));
        }

        [Test]
        public void GetFileNotExistTest()
        {
            var mockClient = new Mock<IClient>();
            mockClient.Setup(m => m.Receive()).Returns(Task.FromResult(Encoding.UTF8.GetBytes("-1")));
            var client = new FileClient(mockClient.Object);
            Assert.ThrowsAsync<FileNotFoundException>(() => client.Get("C:\\ololo", $"{TestContext.CurrentContext.TestDirectory}.\\test1.txt"));
        }

        [Test]
        public void GetInvalidFileTest()
        {
            var mockClient = new Mock<IClient>();
            mockClient.Setup(m => m.Receive()).Returns(Task.FromResult(Encoding.UTF8.GetBytes("-2")));
            var client = new FileClient(mockClient.Object);
            Assert.ThrowsAsync<ArgumentException>(() => client.Get("C:\\ololo", $"{TestContext.CurrentContext.TestDirectory}.\\test1.txt"));
        }

        private static object[] ListTestCases =
        {
            new object[] { "1 ololo true", new List<FileInformation> { new FileInformation("ololo", true) } },
            new object[] { "5 ololo.txt false olo.pdf false lolol true ololo.jpg false olololo true",
                    new List<FileInformation>
                    { 
                        new FileInformation("ololo.txt", false),
                        new FileInformation("olo.pdf", false),
                        new FileInformation("lolol", true),
                        new FileInformation("ololo.jpg", false),
                        new FileInformation("olololo", true)
                    } },
            new object[] { "0", new List<FileInformation>() },
            new object[] { "1 true true", new List<FileInformation> { new FileInformation("true", true) } }
        };
    }
}