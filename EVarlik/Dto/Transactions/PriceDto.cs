namespace EVarlik.Dto.Transactions
{
    public class PriceDto
    {
        public string IdCoinType { get; set; }
        public decimal CoinUnitPrice { get; set; }
        public bool IsIncreasing { get; set; }
        public decimal Ratio { get; set; }
    }
}