namespace ElectronicsERP.Models
{
    public class CustomerDto
    {
        public int CustomerID { get; set; }
        public string Name { get; set; }
        public string ContactNo { get; set; }
        public string Address { get; set; }
        
        public DateTime? CreatedDate { get; set; }
    }
}