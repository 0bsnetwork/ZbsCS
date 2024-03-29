﻿using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZbsCS;

namespace ZbsCSTests
{
    [TestClass]
    public class InvokeScriptTest
    {
        [TestInitialize]
        public void Init()
        {
            Http.Tracing = true;
        }

        [TestMethod]
        public void TestInvokeScript()
        {
            Http.Tracing = true;
            var node = new Node(Node.TestNetChainId);

            var Alice = PrivateKeyAccount.CreateFromSeed("seedAlice123", node.ChainId);
            var Bob = PrivateKeyAccount.CreateFromSeed("seedBob123", node.ChainId);

            var script = @"{-# STDLIB_VERSION 3 #-}
{-# CONTENT_TYPE DAPP #-}
{-# SCRIPT_TYPE ACCOUNT #-}

@Callable(inv)
func foo (a:ByteVector) = {
    WriteSet([DataEntry(""a"", a),
    DataEntry(""sender"", inv.caller.bytes)])
}";
            var compiledScript = node.CompileScript(script);

            var response = node.SetScript(Alice, compiledScript);
            node.WaitForTransactionBroadcastResponseConfirmation(response);

            response = node.InvokeScript(Bob, Alice.Address, "foo", new List<object> { 42L }, null);
            node.WaitForTransactionBroadcastResponseConfirmation(response);

            Assert.AreEqual((long)node.GetAddressData(Alice.Address)["a"], 42L);
            Assert.AreEqual(((byte[])node.GetAddressData(Alice.Address)["sender"]).ToBase58(), Bob.Address);

            var dataTx = new DataTransaction(
                chainId: node.ChainId,
                senderPublicKey: Alice.PublicKey,
                entries: new Dictionary<string, object> { { "a", "OOO" } },
                fee: 0.005m
            ).Sign(Alice);

            node.BroadcastAndWait(dataTx);

            Assert.AreEqual(node.GetAddressData(Alice.Address)["a"], "OOO");

            response = node.SetScript(Alice, null);
            node.WaitForTransactionBroadcastResponseConfirmation(response);

            var scriptInfo = node.GetObject("addresses/scriptInfo/{0}", Alice.Address);
            Assert.IsFalse(scriptInfo.ContainsKey("scriptText"));
        }
    }
}