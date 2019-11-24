using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace FTPServer.Tests
{
    [TestFixture]
    class FileQueryParserTests
    {
        [TestCaseSource("TestCases")]
        public void ParseQueryTest(string query, Type resultType)
        {
            var parser = new FileQueryParser();
            var mockClient = new Mock<TcpClient>();
            var command = parser.ParseQuery(query, mockClient.Object);

            if (resultType == null)
            {
                Assert.IsTrue(command == null);
                return;
            }            

            Assert.AreEqual(resultType, command.GetType());
        }

        private static object[] TestCases =
        {
            new object[] { "$bye", typeof(DisconnectCommand) },
            new object[] { "1 ololo", typeof(ListCommand) },
            new object[] { "2 ololo", typeof(GetCommand) },
            new object[] { "ololo", null }
        };
    }
}