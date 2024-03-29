﻿using System.Collections.Generic;
using ZbsCS;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZbsCSTests
{
    [TestClass]
    public class MassTransferTest
    {
        [TestInitialize]
        public void Init()
        {
            Http.Tracing = true;
        }

        [TestMethod]
        public void TestMassTransferTransaction()
        {
            var node = new Node();

            var recipients = new List<MassTransferItem>
            {
                new MassTransferItem(Accounts.Alice.Address, 0.01m),
                new MassTransferItem(Accounts.Bob.Address, 0.02m),
                new MassTransferItem("3N1JMgUfzYUZinPrzPWeRa6yqN67oo57XR7", 0.003m),
            };

            var tx = new MassTransferTransaction(node.ChainId, Accounts.Alice.PublicKey, Assets.ZBS, recipients, "Shut up & take my money");

            Assert.AreEqual(0.003m, tx.Fee);

            tx.Sign(Accounts.Alice);
            node.BroadcastAndWait(tx.GetJsonWithSignature());
            node.MassTransfer(Accounts.Alice, Assets.ZBS, "masstransfer.txt", "Shut up & take my money");
        }
    }
}
