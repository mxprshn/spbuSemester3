using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System;

namespace MyNUnit.Tests
{
    [TestFixture]
    public class TestRunnerTests
    {
        [Test]
        public void TestRunnerSmokeTest()
        {
            var results = TestRunner.Test($"{AppDomain.CurrentDomain.BaseDirectory}\\..\\..\\..\\MyNUnitSmokeTest\\Assembly");
            var methodNames = new List<string>() { "Test1", "Test2", "Test3", "Test4" };
            Assert.AreEqual(4, results.Count);
            Assert.IsTrue(results.Select(r => r.Name).SequenceEqual(methodNames));

            foreach (var result in results)
            {
                Assert.IsTrue(result.IsPassed);
                Assert.IsFalse(result.IsIgnored);
            }
        }

        [Test]
        public void TestRunnerFailedTest()
        {
            var results = TestRunner.Test($"{AppDomain.CurrentDomain.BaseDirectory}\\..\\..\\..\\MyNUnitFailedTest\\Assembly");
            var methodNames = new List<string>() { "Test1", "Test2" };
            Assert.AreEqual(2, results.Count);
            Assert.IsTrue(results.Select(r => r.Name).SequenceEqual(methodNames));

            foreach (var result in results)
            {
                Assert.IsFalse(result.IsPassed);
                Assert.IsFalse(result.IsIgnored);
            }
        }

        [Test]
        public void TestRunnerBeforeAttributeTest()
        {
            var results = TestRunner.Test($"{AppDomain.CurrentDomain.BaseDirectory}\\..\\..\\..\\BeforeAttributeTest\\Assembly");
            var methodNames = new List<string>() { "Test" };
            Assert.AreEqual(1, results.Count);
            Assert.IsTrue(results.Select(r => r.Name).SequenceEqual(methodNames));
            Assert.IsTrue(results.First().IsPassed);
            Assert.IsFalse(results.First().IsIgnored);
        }

        [Test]
        public void TestRunnerAfterAttributeTest()
        {
            var results = TestRunner.Test($"{AppDomain.CurrentDomain.BaseDirectory}\\..\\..\\..\\AfterAttributeTest\\Assembly");
            var methodNames = new List<string>() { "Test1", "Test2" };
            Assert.AreEqual(2, results.Count);
            Assert.IsTrue(results.Select(r => r.Name).SequenceEqual(methodNames));
            Assert.IsTrue(results[0].IsPassed ^ results[1].IsPassed);
        }

        [Test]
        public void TestRunnerExpectedAttributeTest()
        {
            var results = TestRunner.Test($"{AppDomain.CurrentDomain.BaseDirectory}\\..\\..\\..\\ExpectedAttributeTest\\Assembly");
            var methodNames = new List<string>() { "Test" };
            Assert.AreEqual(1, results.Count);
            Assert.IsTrue(results.Select(r => r.Name).SequenceEqual(methodNames));
            Assert.IsTrue(results.First().IsPassed);
        }

        [Test]
        public void TestRunnerIgnoredAttributeTest()
        {
            var results = TestRunner.Test($"{AppDomain.CurrentDomain.BaseDirectory}\\..\\..\\..\\IgnoreAttributeTest\\Assembly");
            var methodNames = new List<string>() { "IgnoredTest" };
            Assert.AreEqual(1, results.Count);
            Assert.IsTrue(results.Select(r => r.Name).SequenceEqual(methodNames));
            Assert.IsNull(results.First().IsPassed);
            Assert.IsTrue(results.First().IsIgnored);
            Assert.AreEqual("ololo", results.First().IgnoreReason);
        }

        [Test]
        public void TestRunnerBeforeClassAttributeTest()
        {
            var results = TestRunner.Test($"{AppDomain.CurrentDomain.BaseDirectory}\\..\\..\\..\\BeforeClassAttributeTest\\Assembly");
            var methodNames = new List<string>() { "Test1", "Test2", "Test3" };
            Assert.AreEqual(3, results.Count);
            Assert.IsTrue(results.Select(r => r.Name).SequenceEqual(methodNames));

            foreach (var result in results)
            {
                Assert.IsTrue(result.IsPassed);
                Assert.IsFalse(result.IsIgnored);
            }
        }

        [Test]
        public void TestRunnerAfterClassAttributeTest()
        {
            var results = TestRunner.Test($"{AppDomain.CurrentDomain.BaseDirectory}\\..\\..\\..\\AfterClassAttributeTest\\Assembly");
            Assert.AreEqual(4, results.Count);

            foreach (var result in results)
            {
                Assert.IsTrue(result.IsPassed);
            }
        }

        [Test]
        public void TestRunnerNoAppropriateConstructorTest()
        {
            var exception = Assert.Throws<AggregateException>(() => TestRunner.Test($"{AppDomain.CurrentDomain.BaseDirectory}" +
                $"\\..\\..\\..\\NoAppropriateConstructorTest\\Assembly"));
            Assert.IsInstanceOf(typeof(TestRunnerException), exception.InnerException);
        }

        [Test]
        public void TestRunnerInvalidTestMethodsTest()
        {
            var exception = Assert.Throws<AggregateException>(() => TestRunner.Test($"{AppDomain.CurrentDomain.BaseDirectory}" +
                $"\\..\\..\\..\\InvalidTestMethodsTest\\Assembly"));
            Assert.IsInstanceOf(typeof(TestRunnerException), exception.InnerException);
        }

        [Test]
        public void TestRunnerInvalidStaticMethodTest()
        {
            var exception = Assert.Throws<AggregateException>(() => TestRunner.Test($"{AppDomain.CurrentDomain.BaseDirectory}" +
                $"\\..\\..\\..\\InvalidStaticMethodTest\\Assembly"));
            Assert.IsInstanceOf(typeof(TestRunnerException), exception.InnerException);
        }
    }
}