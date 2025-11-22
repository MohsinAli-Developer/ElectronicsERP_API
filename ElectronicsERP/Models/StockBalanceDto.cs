namespace ElectronicsERP.Models
{
    public class StockBalanceDto
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int WarehouseID { get; set; }
        public string WarehouseName { get; set; }
        public int TotalIn { get; set; }
        public int TotalOut { get; set; }
        public int CurrentStock { get; set; }
    }
}
