namespace ElectronicsERP.Models
{
    public class RegisterRequest
    {
        public string Name { get; set; }
        public int Role { get; set; }  // change from string → int
        public string Password { get; set; }
    }
}
