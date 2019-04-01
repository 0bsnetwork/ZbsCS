namespace ZbsCS
{

    public class Asset
    {
        public string Id { get; }
        public string Name { get; }
        public byte Decimals { get; }

        public string IdOrNull => Id == "ZBS" ? null : Id;

        public byte[] Script { get; set; }

        private readonly decimal _scale;

        public Asset(string id, string name, byte decimals, byte[] script = null)
        {
            Id = id;
            Name = name;
            Decimals = decimals;
            Script = script;
            _scale = new decimal(1, 0, 0, false, decimals);
        }

        public long AmountToLong(decimal amount)
        {            
            return decimal.ToInt64(amount / _scale);
        }
        
        public decimal LongToAmount(long value)
        {            
            return value * _scale;
        }

        public static long AmountToLong(byte digits, decimal amount)
        {
            var scale = new decimal(1, 0, 0, false, digits);
            return decimal.ToInt64(amount / scale);
        }

        public static long PriceToLong(Asset amountAsset, Asset priceAsset, decimal price)
        {
            var decimals =  8 - amountAsset.Decimals + priceAsset.Decimals;
            var scale = new decimal(1, 0, 0, false, (byte) decimals);
            return decimal.ToInt64(price / scale);
        }
        
        public static decimal LongToPrice(Asset amountAsset, Asset priceAsset, long price)
        {
            var decimals =  8 - amountAsset.Decimals + priceAsset.Decimals;
            var scale = new decimal(1, 0, 0, false, (byte) decimals);
            return price * scale;
        }

        public override bool Equals(object obj)
        {
            return obj is Asset && Id == ((Asset)obj).Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public static class Assets
    {
        public static readonly Asset ZBS = new Asset("ZBS", "ZBS", 8);
        public static readonly Asset EUR = new Asset("Hq6nu99n8U7ZserUU44VL9wi8t8UYjA3mngYRUXniyki", "EUR", 8);
      
    }
}