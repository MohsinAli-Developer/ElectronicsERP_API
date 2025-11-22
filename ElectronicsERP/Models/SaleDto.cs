namespace ElectronicsERP.Models
{
    public class SaleDto
    {
        public int CustomerID { get; set; }
        public string InvoiceNo { get; set; }
        public decimal PaidAmount { get; set; }
        public string PaymentStatus { get; set; }
        public int WarehouseID { get; set; }
        public List<SaleItemDto> Products { get; set; }
    }
}
