namespace ElectronicsERP.Models
{
    public class VendorLedgerDto
    {
        public int LedgerID { get; set; }
        public int VendorID { get; set; }
        public string VendorName { get; set; }
        public DateTime Date { get; set; }
        public string TransactionType { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal Balance { get; set; }
    }
}
