namespace ElectronicsERP.Models
{
    public class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; }     // e.g. "View Summary Report"
        public string Route { get; set; }    // optional: "/summary-report"
        public ICollection<RolePermission> RolePermissions { get; set; }
    }

}
