using ElectronicsERP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ElectronicsERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseController : ControllerBase
    {
        private readonly IConfiguration _config;

        public PurchaseController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("create")]
        public async Task<ActionResult<PurchaseResponseDto>> CreatePurchase([FromBody] PurchaseRequestDto request)
        {
            using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("sp_CreateCompanyPurchase", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@VendorID", request.VendorID);
                    cmd.Parameters.AddWithValue("@InvoiceNo", request.InvoiceNo);
                    cmd.Parameters.AddWithValue("@PaidAmount", request.PaidAmount);
                    cmd.Parameters.AddWithValue("@PaymentStatus", request.PaymentStatus);
                    cmd.Parameters.AddWithValue("@WarehouseID", request.WarehouseID);

                    // Build TVP for Products
                    DataTable productTable = new DataTable();
                    productTable.Columns.Add("ProductID", typeof(int));
                    productTable.Columns.Add("Quantity", typeof(int));
                    productTable.Columns.Add("UnitPrice", typeof(decimal));

                    foreach (var item in request.Products)
                    {
                        productTable.Rows.Add(item.ProductID, item.Quantity, item.UnitPrice);
                    }

                    SqlParameter tvpParam = cmd.Parameters.AddWithValue("@Products", productTable);
                    tvpParam.SqlDbType = SqlDbType.Structured;
                    tvpParam.TypeName = "TVP_PurchaseDetails";

                    var result = await cmd.ExecuteScalarAsync();

                    return Ok(new PurchaseResponseDto
                    {
                        PurchaseID = Convert.ToInt32(result),
                        Message = "Purchase created successfully"
                    });
                }
            }
        }
    }

}
