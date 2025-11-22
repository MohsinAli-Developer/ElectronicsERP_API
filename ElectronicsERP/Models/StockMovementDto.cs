namespace ElectronicsERP.Models
{
    public class StockMovementDto
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int WarehouseID { get; set; }
        public string WarehouseName { get; set; }
        public string TransactionType { get; set; }
        public int Quantity { get; set; }
        public DateTime Date { get; set; }
        public string ReferenceNo { get; set; }
        public int? PurchaseID { get; set; }
        public int? SaleID { get; set; }
        public int? VendorID { get; set; }
        public string VendorName { get; set; }
        public int? CustomerID { get; set; }
        public string CustomerName { get; set; }
    }
}
