using ElectronicsERP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ElectronicsERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly IConfiguration _config;

        public SalesController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("create")]
        public IActionResult CreateSale([FromBody] SaleDto sale)
        {
            try
            {
                int saleId;
                using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_CreateSale", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Standard parameters
                        cmd.Parameters.AddWithValue("@CustomerID", sale.CustomerID);
                        cmd.Parameters.AddWithValue("@InvoiceNo", sale.InvoiceNo);
                        cmd.Parameters.AddWithValue("@PaidAmount", sale.PaidAmount);
                        cmd.Parameters.AddWithValue("@PaymentStatus", sale.PaymentStatus);
                        cmd.Parameters.AddWithValue("@WarehouseID", sale.WarehouseID);

                        // TVP parameter for Products
                        DataTable productTable = new DataTable();
                        productTable.Columns.Add("ProductID", typeof(int));
                        productTable.Columns.Add("Quantity", typeof(int));
                        productTable.Columns.Add("UnitPrice", typeof(decimal));

                        foreach (var item in sale.Products)
                        {
                            productTable.Rows.Add(item.ProductID, item.Quantity, item.UnitPrice);
                        }

                        SqlParameter tvpParam = cmd.Parameters.AddWithValue("@Products", productTable);
                        tvpParam.SqlDbType = SqlDbType.Structured;
                        tvpParam.TypeName = "TVP_SaleDetails"; // Your TVP name in SQL

                        conn.Open();
                        saleId = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }

                return Ok(new { Success = true, SaleID = saleId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }
    }

}
