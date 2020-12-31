using System;

namespace financial_backend
{
    public class LedgerEntryRequest
    {
        public string PurchaseCategory { get; set; }
        public string Vendor { get; set; }
        public int Amount { get; set; }
        public DateTime PurchaseDate { get; set; }
    }
}