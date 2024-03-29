﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZbsCS;
using System.Threading;
using System.Text;

namespace ZbsCSTests
{
    [TestClass]
    public class NodeTest
    {        
        private static readonly Asset ZEUR = new Asset("Fmg13HEHJHuZYbtJq8Da8wifJENq8uBxDuWoP9pVe2Qe", "ZEUR", 8);

        [TestInitialize]
        public void Init()
        {
            Http.Tracing = true;
        }

        [TestMethod]
        public void TestGetters()
        {
            var node = new Node();
            Assert.IsTrue(node.GetHeight() > 0);
            Assert.IsTrue(node.GetBalance(Accounts.Bob.Address) >= 0);
            Assert.IsTrue(node.GetBalance(Accounts.Bob.Address, 100) >= 0);
            Assert.IsTrue(node.GetBalance(Accounts.Bob.Address, ZEUR) >= 0);
            Assert.IsTrue(node.GetUnconfirmedPoolSize() >= 0);            
        }

        [TestMethod]
        public void TestGetAsset()
        {
            var node = new Node(Node.MainNetChainId);
            var assetId = "725Yv9oceWsB4GsYwyy4A52kEwyVrL5avubkeChSnL46";

            var asset = node.GetAsset(assetId);

            Assert.AreEqual(assetId, asset.Id);
            Assert.AreEqual("EFYT", asset.Name);
            Assert.AreEqual(8, asset.Decimals);
            
            Assert.AreEqual(200000, asset.AmountToLong(0.002m));
            Assert.AreEqual(0.03m, asset.LongToAmount(3000000));
        }

        [TestMethod]
        public void TestGetTransactionHeight()
        {
            var node = new Node(Node.MainNetChainId);
            var height = node.GetTransactionHeight("5CZV9RouJs7uaRkZY741WDy9zV69npX1FTZqxo5fsryL");
            Assert.AreEqual(height, 1371063);
        }

        [TestMethod]
        public void TestAssetBalances()
        {
            Http.Tracing = false;
            
            var node = new Node(Node.MainNetChainId);
 
            var portfolio = node.GetAssetBalances("3PPF1JfQLJLVd6v4ewmuDbjDLcxBCUe5GSu");
            
            Assert.IsTrue(portfolio.Count > 0);

            foreach (var pair in portfolio)
            {
                Console.WriteLine("Asset: {0}, balance: {1}", pair.Key.Name, pair.Value);
            }
        }
        
        [TestMethod]
        public void TestBalance()
        {
            var node = new Node(Node.MainNetChainId);
 
            var balance = node.GetBalance("3PJaDyprvekvPXPuAtxrapacuDJopgJRaU3", Assets.ZBS);            
            
            Assert.IsTrue(balance > 1000);
        }

        [TestMethod]
        public void TestGetTransactionsByAddress()
        {
            var node = new Node();
            var transactions = node.GetTransactionsByAddress(Accounts.Alice.Address, 10);
            
            Assert.IsTrue(transactions.Count() == 10);
            Assert.IsTrue(transactions.All(t => t.GetByte("type") < 20));
            Assert.IsTrue(transactions.All(t => t.GetString("sender").Length > 30));
        }

        [TestMethod]
        public void TestTransfer()
        {
            var node = new Node();
            
            var transferResponse = node.Transfer(Accounts.Alice, Accounts.Bob.Address, Assets.ZBS, 0.2m, "Hi Bob!");
            Assert.IsNotNull(transferResponse);

            // transfer back so that Alice's balance is not drained
            var transferTxId = node.Transfer(Accounts.Bob, Accounts.Alice.Address, Assets.ZBS, 0.2m, "Thanks, Alice").ParseJsonObject().GetString("id");
            node.WaitForTransactionConfirmation(transferTxId);

            var fee = node.CalculateFee(node.GetTransactionById(transferTxId));
            Assert.IsNotNull(fee);            
        }
        
        [TestMethod]
        public void TestHash()
        {
            var node = new Node();
            var message = "lalala";
            var messageBytes = Encoding.UTF8.GetBytes(message);

            var hashedByNodeMessage = node.SecureHash(message);
            var hashedMessage = AddressEncoding.SecureHash(messageBytes, 0, messageBytes.Length);
            Assert.IsTrue(hashedByNodeMessage.SequenceEqual(hashedMessage));
            var fastHashedByNodeMessage = node.FastHash(message);
            var fasthashedMessage = AddressEncoding.FastHash(messageBytes, 0, messageBytes.Length);
            Assert.IsTrue(fastHashedByNodeMessage.SequenceEqual(fasthashedMessage));
        }

        [TestMethod]
        public void TestUnconfirmed()
        {
            var node = new Node();
            node.GetUnconfirmedTransactions();
        }

        [TestMethod]
        public void TestBatchBroadcast()
        {
            var node = new Node();

            var transactons = new[]
            {
                new TransferTransaction(node.ChainId, Accounts.Alice.PublicKey, Accounts.Bob.Address, Assets.ZBS, 0.3m).Sign(Accounts.Alice),
                new TransferTransaction(node.ChainId, Accounts.Bob.PublicKey, Accounts.Alice.Address, Assets.ZBS, 0.3m).Sign(Accounts.Bob),
            };
            
            var result = node.BatchBroadcast(transactons);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestGetTransactionsFromTimestamp()
        {
            var node = new Node();
            var address = "3NAqFmMtm2msHcnFDCLFJRn3MbfFwGM3ZHo";
            var timestamp = node.GetBlockTransactionsAtHeight(node.GetHeight() - 5).Last().Timestamp.ToLong();
            var packageSize = 2;
            var txs = node.GetTransactionsByAddressAfterTimestamp(address, timestamp, packageSize);
            bool test = false;
            foreach (var tx in txs.Reverse().Take(packageSize))
            {
                if (tx.Timestamp.ToLong() <= timestamp)
                {
                    test = true;
                    break;
                }
            }
            Assert.IsTrue(test);
            Assert.IsNotNull(txs);
        }

        [TestMethod]
        public void TestGetTransactionsByAddressAfterId()
        {
            var node = new Node();
            var address = "3NAqFmMtm2msHcnFDCLFJRn3MbfFwGM3ZHo";
            var txId = "13v6hDYxkGgR3NWwTzC75UhK3kEdT4KHuH51qnBMz88Z";
            var count = 10;
            
            var result = node.GetTransactionsByAddressAfterId(address, txId, count);
            Assert.IsTrue(result.Length == count);
            Assert.IsNotNull(result.Select(tx => tx.GenerateId() == txId));
        }
    }
}
