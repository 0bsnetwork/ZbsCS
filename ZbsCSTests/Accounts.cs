using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZbsCS;

namespace ZbsCSTests
{
    [TestClass]
    public class Accounts
    {
        public static readonly PrivateKeyAccount Alice = PrivateKeyAccount.CreateFromSeed("seed4Alice", AddressEncoding.TestNet);
        public static readonly PrivateKeyAccount Bob = PrivateKeyAccount.CreateFromSeed("seed4Bob", AddressEncoding.TestNet);
        public static readonly PrivateKeyAccount Carol = PrivateKeyAccount.CreateFromSeed("seed4Carol4", AddressEncoding.TestNet);

        [TestInitialize]
        public void Init()
        {
            Http.Tracing = true;
        }

        [TestMethod]
        public void TestBalance()
        {
            // Use faucet to fill accounts https://explorer.testnet-0bsnetwork.com/faucet
            var node = new Node();

            var aliceBalanceZbs = node.GetBalance(Alice.Address);
            var bobBalanceZbs = node.GetBalance(Bob.Address);
            var carolBalanceZbs = node.GetBalance(Carol.Address);

            Console.WriteLine("Alice address: {0}, balance: {1}", Alice.Address, aliceBalanceZbs);
            Console.WriteLine("Bob address: {0}, balance: {1}", Bob.Address, bobBalanceZbs);
            Console.WriteLine("Carol address: {0}, balance: {1}", Carol.Address, carolBalanceZbs);
            
            Assert.IsTrue(aliceBalanceZbs > 1);
            Assert.IsTrue(bobBalanceZbs > 1);
            Assert.IsTrue(carolBalanceZbs > 1);
        }

        [TestMethod]
        public void TestScript()
        {
            Http.Tracing = false;
            var node = new Node();

            var scriptInfo = node.GetObject("addresses/scriptInfo/{0}", Alice.Address);
            Assert.IsFalse(scriptInfo.ContainsKey("scriptText"));

            scriptInfo = node.GetObject("addresses/scriptInfo/{0}", Bob.Address);
            Assert.IsFalse(scriptInfo.ContainsKey("scriptText"));

            scriptInfo = node.GetObject("addresses/scriptInfo/{0}", Carol.Address);
            Assert.IsFalse(scriptInfo.ContainsKey("scriptText"));
        }
    }
}