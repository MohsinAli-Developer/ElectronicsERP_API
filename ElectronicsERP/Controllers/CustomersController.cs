using ElectronicsERP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ElectronicsERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public CustomersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string GetConnectionString()
        {
            return _configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost("add")]
        public IActionResult AddCustomer([FromBody] CustomerDto customer)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_AddCustomer", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Name", customer.Name);
                        cmd.Parameters.AddWithValue("@ContactNo", customer.ContactNo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Address", customer.Address ?? (object)DBNull.Value);

                        conn.Open();
                        var customerId = cmd.ExecuteScalar();
                        return Ok(new { success = true, message = "Customer added successfully", customerID = customerId });
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetCustomerData()
        {
            var customers = new List<CustomerDto>();

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                string query = "SELECT CustomerID, Name, ContactNo, Address, CreatedDate FROM Customers";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    customers.Add(new CustomerDto
                    {
                        CustomerID = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        ContactNo = reader.GetString(2),
                        Address = reader.GetString(3),
                        CreatedDate = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4)
                    });
                }
            }

            return Ok(customers);
        }
    }
}