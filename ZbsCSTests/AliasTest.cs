﻿using System;
using System.Threading;
using ZbsCS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;

namespace ZbsCSTests
{
    [TestClass]
    public class AliasTest
    {

        [TestInitialize]
        public void Init()
        {
            Http.Tracing = true;
        }

        public string GenerateRandomAlias(int length = 15)
        {
            Assert.IsTrue(4 <= length && length <= 30);

            Random random = new Random();
            string aliasAlphabet = "-.0123456789@_abcdefghijklmnopqrstuvwxyz";

            return new string(Enumerable.Repeat(aliasAlphabet, length)
                                   .Select(s => s[random.Next(s.Length)]).ToArray());

        }

        [TestMethod]
        public void TestTransferToAlias()
        {
            var node = new Node(Node.TestNetChainId);

            var seed = PrivateKeyAccount.GenerateSeed();
            var account = PrivateKeyAccount.CreateFromSeed(seed, 'T');

            var response = node.Transfer(Accounts.Alice, account.Address, Assets.ZBS, 0.001m);
            node.WaitForTransactionBroadcastResponseConfirmation(response);

            var alias = GenerateRandomAlias();
            response = node.CreateAlias(account, alias, 'T');
            node.WaitForTransactionBroadcastResponseConfirmation(response);

            var amount = 0.0001m;
            var balanceBefore = node.GetBalance(account.Address);

            response = node.Transfer(Accounts.Alice, "alias:T:" + alias, Assets.ZBS, amount);
            node.WaitForTransactionBroadcastResponseConfirmation(response);

            var balanceAfter = node.GetBalance(account.Address);
            Assert.AreEqual(balanceBefore + amount, balanceAfter);
        }

        [TestMethod]
        public void TestMassTransferToAlias()
        {
            var node = new Node(Node.TestNetChainId);

            var seed = PrivateKeyAccount.GenerateSeed();
            var account = PrivateKeyAccount.CreateFromSeed(seed, 'T');

            var response = node.Transfer(Accounts.Alice, account.Address, Assets.ZBS, 0.001m);
            node.WaitForTransactionBroadcastResponseConfirmation(response);

            var alias = GenerateRandomAlias();
            response = node.CreateAlias(account, alias, 'T');
            node.WaitForTransactionBroadcastResponseConfirmation(response);

            var amount = 0.0001m;
            var balanceBefore = node.GetBalance(account.Address);

            var recipients = new List<MassTransferItem>
            {
                new MassTransferItem(account.Address, amount),
                new MassTransferItem("alias:T:" + alias, amount),
                new MassTransferItem(account.Address, amount),
                new MassTransferItem(account.Address, amount),
                new MassTransferItem("alias:T:" + alias, amount)
            };

            var tx = new MassTransferTransaction(node.ChainId, Accounts.Alice.PublicKey, Assets.ZBS, recipients);
            tx.Sign(Accounts.Alice);
            node.BroadcastAndWait(tx.GetJsonWithSignature());

            var balanceAfter = node.GetBalance(account.Address);
            Assert.AreEqual(balanceBefore + amount * recipients.Count, balanceAfter);
        }
    }
}
