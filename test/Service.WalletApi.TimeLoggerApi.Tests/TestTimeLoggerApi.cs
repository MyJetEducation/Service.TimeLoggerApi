﻿using System;
using NUnit.Framework;

namespace Service.WalletApi.TimeLoggerApi.Tests
{
    public class TestTimeLoggerApi
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Console.WriteLine("Debug output");
            Assert.Pass();
        }
    }
}
