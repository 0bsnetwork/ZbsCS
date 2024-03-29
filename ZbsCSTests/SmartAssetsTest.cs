﻿using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZbsCS;

namespace ZbsCSTests
{
    [TestClass]
    public class SmartAssetsTest
    {
        [TestInitialize]
        public void Init()
        {
            Http.Tracing = true;
        }

        [TestMethod]
        public void TestIssueSmartAsset()
        {
            var node = new Node();

            var compiledScript = node.CompileScript("true");

            Asset smartAsset = node.IssueAsset(Accounts.Alice, "SmartAsset",
                                          "Smart Asset", 100, 4,
                                          true, compiledScript);
            Assert.IsNotNull(smartAsset);
            node.WaitForTransactionConfirmation(smartAsset.Id);

            Assert.AreEqual(node.GetBalance(Accounts.Alice.Address, smartAsset), 100);
            Assert.AreEqual(node.GetAsset(smartAsset.Id).Script.ToBase64(), compiledScript.ToBase64());
        }

        [TestMethod]
        public void TestSetAssetScript()
        {
            var node = new Node();

            var zbsBalanceBefore = node.GetBalance(Accounts.Alice.Address, Assets.ZBS);

            Asset smartAsset = node.IssueAsset(Accounts.Alice, "SmartAsset",
                                               "Smart Asset", 100, 8,
                                               true, node.CompileScript("true"));

            node.WaitForTransactionConfirmation(smartAsset.Id);

            var script = $@"                
                match tx {{
                    case t : TransferTransaction => t.amount < 30000000
                    case bs : BurnTransaction | SetAssetScriptTransaction => true
                    case _  => false
                }}";

            var compiledScript = node.CompileScript(script);

            var response = node.SetAssetScript(Accounts.Alice, smartAsset, compiledScript, 'T', 1);
            node.WaitForTransactionBroadcastResponseConfirmation(response);

            var aliceBalanceBefore = node.GetBalance(Accounts.Alice.Address, smartAsset);
            var bobBalanceBefore = node.GetBalance(Accounts.Bob.Address, smartAsset);

            for (decimal amount = 0.01m; amount < 0.5m; amount += 0.1m)
            {
                try
                {
                    response = node.Transfer(Accounts.Alice, Accounts.Bob.Address, smartAsset, amount, 0.005m);
                    node.WaitForTransactionBroadcastResponseConfirmation(response);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            var aliceBalanceAfter = node.GetBalance(Accounts.Alice.Address, smartAsset);
            var bobBalanceAfter = node.GetBalance(Accounts.Bob.Address, smartAsset);

            Assert.AreEqual(aliceBalanceBefore - aliceBalanceAfter, 0.01m + 0.11m + 0.21m);
            Assert.AreEqual(bobBalanceAfter - bobBalanceBefore, 0.01m + 0.11m + 0.21m);

            response = node.SetAssetScript(Accounts.Alice, smartAsset, node.CompileScript("false"), 'T', 1);
            node.WaitForTransactionBroadcastResponseConfirmation(response);

            Assert.AreEqual(node.GetAsset(smartAsset.Id).Script.ToBase64(), node.CompileScript("false").ToBase64());

            var zbsBalanceAfter = node.GetBalance(Accounts.Alice.Address, Assets.ZBS);

            // Check the fee
            Assert.AreEqual(zbsBalanceBefore, zbsBalanceAfter + 1m + 1m + 0.005m * 3 + 1m);
        }
    }
}
