﻿using System;
using ZbsCS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DictionaryObject = System.Collections.Generic.Dictionary<string, object>;

namespace ZbsCSTests
{
    [TestClass]
    public class DataTransactionTest
    {
        [TestInitialize]
        public void Init()
        {
            Http.Tracing = true;
        }

        [TestMethod]
        public void TestDataTransaction()
        {
            var node = new Node();

            var data = new DictionaryObject
            {
                { "test long", -1001L },
                { "test true", true },
                { "test false", false },
                { "test bytes", new byte[] { 1, 2, 3, 4, 5}},
                { "test string", "Hello, 0bsNetwork!"},
                { "test russian", "Привет" }
            };

            var tx = new DataTransaction(node.ChainId, Accounts.Alice.PublicKey, data).Sign(Accounts.Alice);            
            
            Console.WriteLine("Tx size: " + tx.GetBody().Length);            
            Console.WriteLine("Response tx id: " + node.BroadcastAndWait(tx.GetJsonWithSignature()));

            var addressData = node.GetAddressData(Accounts.Alice.Address);

            Assert.AreEqual(-1001L, addressData["test long"]);
            Assert.AreEqual(true, addressData["test true"]);
            Assert.AreEqual(false, addressData["test false"]);
            Assert.AreEqual("Hello, 0bsNetwork!", addressData["test string"]);
            Assert.AreEqual("Привет", addressData["test russian"]);
            CollectionAssert.AreEquivalent(new byte[] { 1, 2, 3, 4, 5 }, (byte[])addressData["test bytes"]);

        }
    }
}
