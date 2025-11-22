namespace ElectronicsERP.Models
{
    public class PurchaseRequestDto
    {
        public int VendorID { get; set; }
        public string InvoiceNo { get; set; }
        public decimal PaidAmount { get; set; }
        public string PaymentStatus { get; set; }
        public int WarehouseID { get; set; }
        public List<PurchaseProductDto> Products { get; set; }
    }
}
