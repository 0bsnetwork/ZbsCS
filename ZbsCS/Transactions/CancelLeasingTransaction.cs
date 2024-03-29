﻿using System.IO;
using DictionaryObject = System.Collections.Generic.Dictionary<string, object>;

namespace ZbsCS
{
    public class CancelLeasingTransaction : Transaction
    {
        public string LeaseId { get; }
        public override byte Version { get; set; } = 2;

        public CancelLeasingTransaction(char chainId, byte[] senderPublicKey, string leaseId, decimal fee = 1m) : 
            base(chainId, senderPublicKey)
        {
            LeaseId = leaseId;
            Fee = fee;
        }

        public CancelLeasingTransaction(DictionaryObject tx) : base (tx)
        {
            LeaseId = tx.GetString("leaseId");
            Fee = Assets.ZBS.LongToAmount(tx.GetLong("fee"));
        }

        public override byte[] GetBody()
        {
            using(var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(TransactionType.LeaseCancel);
                writer.Write(SenderPublicKey);
                writer.WriteLong(Assets.ZBS.AmountToLong(Fee));
                writer.WriteLong(Timestamp.ToLong());
                writer.Write(LeaseId.FromBase58());
                return stream.ToArray();
            }            
        }

        internal override byte[] GetIdBytes()
        {
            return GetBody();
        }

        public override DictionaryObject GetJson()
        {
            var result = new DictionaryObject
            {
                {"type", (byte) TransactionType.LeaseCancel},
                {"senderPublicKey", SenderPublicKey.ToBase58()},
                {"leaseId", LeaseId},
                {"fee", Assets.ZBS.AmountToLong(Fee)},
                {"timestamp", Timestamp.ToLong()}
            };

            if (Sender != null)
                result.Add("sender", Sender);

            return result;
        }

        protected override bool SupportsProofs()
        {
            return false;
        }
    }
}