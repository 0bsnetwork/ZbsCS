﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZbsCS;

namespace ZbsCSTests
{
    [TestClass]
    public class Base58Test
    {
        
        [TestMethod]
        public void TestEncodeDecode()
        {
            var address = "3N1JMgUfzYUZinPrzPWeRa6yqN67oo57XR7";

            Assert.AreEqual(address, address.FromBase58().ToBase58());
                       
        }
    }
}