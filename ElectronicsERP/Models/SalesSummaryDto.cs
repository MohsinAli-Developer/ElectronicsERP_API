namespace ElectronicsERP.Models
{
    public class SalesSummaryDto
    {
        public DateTime SaleDate { get; set; }
        public decimal TotalSalesAmount { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalProfit { get; set; }
        public int TotalInvoices { get; set; }
    }
}