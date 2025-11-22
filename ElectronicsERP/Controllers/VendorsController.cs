using ElectronicsERP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ElectronicsERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendorsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public VendorsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // ✅ Add Vendor
        [HttpPost]
        public IActionResult AddVendor([FromBody] Vendor vendor)
        {
            string query = @"INSERT INTO Vendors (Name, ContactNo, CompanyName, Address) 
                             VALUES (@Name, @ContactNo, @CompanyName, @Address)";

            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", vendor.Name);
                    cmd.Parameters.AddWithValue("@ContactNo", vendor.ContactNo);
                    cmd.Parameters.AddWithValue("@CompanyName", vendor.CompanyName);
                    cmd.Parameters.AddWithValue("@Address", vendor.Address);
                    cmd.ExecuteNonQuery();
                }
            }
            return Ok(new { message = "Vendor added successfully" });
        }

        // ✅ Get All Vendors
        [HttpGet]
        public IActionResult GetVendors()
        {
            var vendors = new List<object>();
            string query = "SELECT VendorID, Name, ContactNo, CompanyName, Address, CreatedDate FROM Vendors";

            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        vendors.Add(new
                        {
                            VendorID = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            ContactNo = reader.GetString(2),
                            CompanyName = reader.GetString(3),
                            Address = reader.GetString(4),
                            CreatedDate = reader.GetDateTime(5)
                        });
                    }
                }
            }

            return Ok(vendors);
        }

        // ✅ Delete Vendor
        [HttpDelete("{id}")]
        public IActionResult DeleteVendor(int id)
        {
            string query = "DELETE FROM Vendors WHERE VendorID = @id";

            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                        return Ok(new { message = "Vendor deleted" });
                    else
                        return NotFound(new { message = "Vendor not found" });
                }
            }
        }

        // ✅ Update Vendor (EDIT)
        [HttpPut("{id}")]
        public IActionResult UpdateVendor(int id, [FromBody] Vendor vendor)
        {
            string query = @"UPDATE Vendors 
                             SET Name = @Name, 
                                 ContactNo = @ContactNo, 
                                 CompanyName = @CompanyName, 
                                 Address = @Address
                             WHERE VendorID = @id";

            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@Name", vendor.Name);
                    cmd.Parameters.AddWithValue("@ContactNo", vendor.ContactNo);
                    cmd.Parameters.AddWithValue("@CompanyName", vendor.CompanyName);
                    cmd.Parameters.AddWithValue("@Address", vendor.Address);

                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                        return Ok(new { message = "Vendor updated successfully" });
                    else
                        return NotFound(new { message = "Vendor not found" });
                }
            }
        }
    }
}

//using ElectronicsERP.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Data.SqlClient;
//using System.Numerics;

//namespace ElectronicsERP.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class VendorsController : ControllerBase
//    {
//        private readonly IConfiguration _configuration;

//        public VendorsController(IConfiguration configuration)
//        {
//            _configuration = configuration;
//        }
//        [HttpPost]
//        public IActionResult AddVendor([FromBody] Vendor vendor)
//        {
//            string query = @"INSERT INTO Vendors (Name, ContactNo, CompanyName, Address) 
//                     VALUES (@Name, @ContactNo, @CompanyName, @Address)";

//            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
//            {
//                conn.Open();
//                using (SqlCommand cmd = new SqlCommand(query, conn))
//                {
//                    cmd.Parameters.AddWithValue("@Name", vendor.Name);
//                    cmd.Parameters.AddWithValue("@ContactNo", vendor.ContactNo);
//                    cmd.Parameters.AddWithValue("@CompanyName", vendor.CompanyName);
//                    cmd.Parameters.AddWithValue("@Address", vendor.Address);
//                    cmd.ExecuteNonQuery();
//                }
//            }
//            return Ok(new { message = "Vendor added successfully" });
//        }


//        [HttpGet]
//        public IActionResult GetVendors()
//        {
//            var vendors = new List<object>();
//            string query = "SELECT VendorID, Name, ContactNo, CompanyName, Address, CreatedDate FROM Vendors";

//            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
//            {
//                conn.Open();
//                using (SqlCommand cmd = new SqlCommand(query, conn))
//                using (SqlDataReader reader = cmd.ExecuteReader())
//                {
//                    while (reader.Read())
//                    {
//                        vendors.Add(new
//                        {
//                            VendorID = reader.GetInt32(0),
//                            Name = reader.GetString(1),
//                            ContactNo = reader.GetString(2),
//                            CompanyName = reader.GetString(3),
//                            Address = reader.GetString(4),
//                            CreatedDate = reader.GetDateTime(5)
//                        });
//                    }
//                }
//            }

//            return Ok(vendors);
//        }

//        [HttpDelete("{id}")]
//        public IActionResult DeleteVendor(int id)
//        {
//            string query = "DELETE FROM Vendors WHERE VendorID = @id";

//            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
//            {
//                conn.Open();
//                using (SqlCommand cmd = new SqlCommand(query, conn))
//                {
//                    cmd.Parameters.AddWithValue("@id", id);
//                    int rows = cmd.ExecuteNonQuery();
//                    if (rows > 0)
//                        return Ok(new { message = "Vendor deleted" });
//                    else
//                        return NotFound(new { message = "Vendor not found" });
//                }
//            }
//        }
//    }

//}
