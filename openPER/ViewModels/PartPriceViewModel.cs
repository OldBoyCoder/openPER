namespace openPER.ViewModels
{
    public class PartPriceViewModel
    {
        public int MarketCode { get; set; }
        public string DiscountCode { get; set; }
        public double Price { get; set; }
        public double Tax { get; set; }
        public string MarketDescription { get; set; }
        public string CurrencyCode { get; set; }
    }

}