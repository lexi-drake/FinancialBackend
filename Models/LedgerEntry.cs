using System;

namespace financial_backend
{
    public class LedgerEntry
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string PurchaseCategoryId { get; set; }
        public string Vendor { get; set; }
        public int Amount { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}