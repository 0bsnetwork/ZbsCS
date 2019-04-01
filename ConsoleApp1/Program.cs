using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZbsCS;
using DictionaryObject = System.Collections.Generic.Dictionary<string, object>;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var node = new Node();

            var data = new DictionaryObject
            {
                { "test string", "Hello, 0bsNetwork!"}
            };

            PrivateKeyAccount Alice = PrivateKeyAccount.CreateFromSeed("test", AddressEncoding.TestNet);

            var tx = new DataTransaction(node.ChainId, Alice.PublicKey, data).Sign(Alice);

            Console.WriteLine("Tx size: " + tx.GetBody().Length);
            Console.WriteLine("Response tx id: " + node.BroadcastAndWait(tx.GetJsonWithSignature()));

            Console.ReadLine();
        }



    
    }
}
