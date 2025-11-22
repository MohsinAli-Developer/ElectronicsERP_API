namespace ElectronicsERP.Models
{
    public class ProductDTO
    {
        public int ProductID { get; set; }
        public string ModelNo { get; set; }
        public string ProductName { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public int QuantityInStock { get; set; }
        public int ReorderLevel { get; set; }
        public string Warehousename { get; set; }
    }
}
