using Dapper;
using ElectronicsERP.Attributes;
using ElectronicsERP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace ElectronicsERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockMovementController : ControllerBase
    {
        private readonly IConfiguration _config;

        public StockMovementController(IConfiguration config)
        {
            _config = config;
        }

        private string GetConnectionString()
        {
            return _config.GetConnectionString("DefaultConnection");
        }

        // ✅ GET: api/stockmovement
        [Authorize]
        [HasPermission("View Stock Movement")]
        [HttpGet]
        public IActionResult GetStockMovements()
        {
            var movements = new List<StockMovementDto>();

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                string query = "SELECT * FROM vw_StockMovement ORDER BY Date DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    movements.Add(new StockMovementDto
                    {
                        ProductID = Convert.ToInt32(reader["ProductID"]),
                        ProductName = reader["ProductName"].ToString(),
                        WarehouseID = Convert.ToInt32(reader["WarehouseID"]),
                        WarehouseName = reader["WarehouseName"].ToString(),
                        TransactionType = reader["TransactionType"].ToString(),
                        Quantity = Convert.ToInt32(reader["Quantity"]),
                        Date = Convert.ToDateTime(reader["Date"]),
                        ReferenceNo = reader["ReferenceNo"].ToString(),
                        PurchaseID = reader["PurchaseID"] != DBNull.Value ? Convert.ToInt32(reader["PurchaseID"]) : (int?)null,
                        SaleID = reader["SaleID"] != DBNull.Value ? Convert.ToInt32(reader["SaleID"]) : (int?)null,
                        VendorID = reader["VendorID"] != DBNull.Value ? Convert.ToInt32(reader["VendorID"]) : (int?)null,
                        VendorName = reader["VendorName"]?.ToString(),
                        CustomerID = reader["CustomerID"] != DBNull.Value ? Convert.ToInt32(reader["CustomerID"]) : (int?)null,
                        CustomerName = reader["CustomerName"]?.ToString()
                    });
                }
                reader.Close();
            }

            return Ok(movements);
        }

        // ✅ GET: api/stockmovement/warehouse/Shop
        [HttpGet("warehouse/{warehouseName}")]
        public IActionResult GetStockMovementsByWarehouse(string warehouseName)
        {
            var movements = new List<StockMovementDto>();

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                string query = @"SELECT * FROM vw_StockMovement 
                                 WHERE WarehouseName = @WarehouseName 
                                 ORDER BY Date DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@WarehouseName", warehouseName);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    movements.Add(new StockMovementDto
                    {
                        ProductID = Convert.ToInt32(reader["ProductID"]),
                        ProductName = reader["ProductName"].ToString(),
                        WarehouseID = Convert.ToInt32(reader["WarehouseID"]),
                        WarehouseName = reader["WarehouseName"].ToString(),
                        TransactionType = reader["TransactionType"].ToString(),
                        Quantity = Convert.ToInt32(reader["Quantity"]),
                        Date = Convert.ToDateTime(reader["Date"]),
                        ReferenceNo = reader["ReferenceNo"].ToString(),
                        PurchaseID = reader["PurchaseID"] != DBNull.Value ? Convert.ToInt32(reader["PurchaseID"]) : (int?)null,
                        SaleID = reader["SaleID"] != DBNull.Value ? Convert.ToInt32(reader["SaleID"]) : (int?)null,
                        VendorID = reader["VendorID"] != DBNull.Value ? Convert.ToInt32(reader["VendorID"]) : (int?)null,
                        VendorName = reader["VendorName"]?.ToString(),
                        CustomerID = reader["CustomerID"] != DBNull.Value ? Convert.ToInt32(reader["CustomerID"]) : (int?)null,
                        CustomerName = reader["CustomerName"]?.ToString()
                    });
                }
                reader.Close();
            }

            return Ok(movements);
        }

        [HttpGet("GetSalesSummary")]
        public async Task<IActionResult> GetSalesSummary(DateTime? saleDate = null)
        {
            using (var connection = new SqlConnection(GetConnectionString()))
            {
                string spName = "sp_GetSalesSummary";
                var result = await connection.QueryAsync<SalesSummaryDto>(
                    spName,
                    new { SaleDate = saleDate },
                    commandType: CommandType.StoredProcedure
                );

                return Ok(result);
            }
        }


    }
}
