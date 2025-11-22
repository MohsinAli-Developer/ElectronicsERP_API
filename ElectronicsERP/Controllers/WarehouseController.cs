using ElectronicsERP.Attributes;
using ElectronicsERP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ElectronicsERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        private readonly IConfiguration _config;

        public WarehouseController(IConfiguration config)
        {
            _config = config;
        }

        private string GetConnectionString()
        {
            return _config.GetConnectionString("DefaultConnection");
        }

        // ✅ Get all warehouses
 
        [HttpGet]
        public IActionResult GetWarehouses()
        {
            var warehouses = new List<dynamic>();

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                string query = "SELECT WarehouseID, Name, Location, Capacity, Manager FROM Warehouse";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    warehouses.Add(new
                    {
                        WarehouseID = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Location = reader.GetString(2),
                        Capacity = reader.GetInt32(3),
                        Manager = reader.GetString(4)
                    });
                }
            }

            return Ok(warehouses);
        }

        // ✅ Add new warehouse
        [HttpPost]
        public IActionResult AddWarehouse([FromBody] Warehouse warehouse)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                string query = "INSERT INTO Warehouse (Name, Location, Capacity, Manager) VALUES (@Name, @Location, @Capacity, @Manager)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", warehouse.Name);
                cmd.Parameters.AddWithValue("@Location", warehouse.Location);
                cmd.Parameters.AddWithValue("@Capacity", warehouse.Capacity);
                cmd.Parameters.AddWithValue("@Manager", warehouse.Manager);

                cmd.ExecuteNonQuery();
            }

            return Ok(new { message = "Warehouse added successfully!" });
        }

        // ✅ Update warehouse
        [HttpPut("{id}")]
        public IActionResult UpdateWarehouse(int id, [FromBody] Warehouse warehouse)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                string query = "UPDATE Warehouse SET Name=@Name, Location=@Location, Capacity=@Capacity, Manager=@Manager WHERE WarehouseID=@Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", warehouse.Name);
                cmd.Parameters.AddWithValue("@Location", warehouse.Location);
                cmd.Parameters.AddWithValue("@Capacity", warehouse.Capacity);
                cmd.Parameters.AddWithValue("@Manager", warehouse.Manager);
                cmd.Parameters.AddWithValue("@Id", id);

                int rows = cmd.ExecuteNonQuery();
                if (rows == 0) return NotFound(new { message = "Warehouse not found!" });
            }

            return Ok(new { message = "Warehouse updated successfully!" });
        }

        // ✅ Delete warehouse
        [HttpDelete("{id}")]
        public IActionResult DeleteWarehouse(int id)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                string query = "DELETE FROM Warehouse WHERE WarehouseID=@Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                int rows = cmd.ExecuteNonQuery();
                if (rows == 0) return NotFound(new { message = "Warehouse not found!" });
            }

            return Ok(new { message = "Warehouse deleted successfully!" });
        }

        [HttpGet("stock-balance")]
        public IActionResult GetStockBalance()
        {
            List<StockBalanceDto> stockList = new List<StockBalanceDto>();

            string connectionString = _config.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT ProductID, ProductName, WarehouseID, WarehouseName, TotalIn, TotalOut, CurrentStock FROM vw_StockBalance";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    StockBalanceDto stock = new StockBalanceDto
                    {
                        ProductID = Convert.ToInt32(reader["ProductID"]),
                        ProductName = reader["ProductName"].ToString(),
                        WarehouseID = Convert.ToInt32(reader["WarehouseID"]),
                        WarehouseName = reader["WarehouseName"].ToString(),
                        TotalIn = Convert.ToInt32(reader["TotalIn"]),
                        TotalOut = Convert.ToInt32(reader["TotalOut"]),
                        CurrentStock = Convert.ToInt32(reader["CurrentStock"])
                    };

                    stockList.Add(stock);
                }
                reader.Close();
            }

            return Ok(stockList);
        }

        // ✅ GET: api/stock/warehouse/Shop
        [HttpGet("stock-balance/{warehouseName}")]
        public IActionResult GetStockByWarehouse(string warehouseName)
        {
            List<StockBalanceDto> stockList = new List<StockBalanceDto>();

            string connectionString = _config.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT ProductID, ProductName, WarehouseID, WarehouseName, TotalIn, TotalOut, CurrentStock 
                             FROM vw_StockBalance WHERE WarehouseName = @WarehouseName";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@WarehouseName", warehouseName);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    StockBalanceDto stock = new StockBalanceDto
                    {
                        ProductID = Convert.ToInt32(reader["ProductID"]),
                        ProductName = reader["ProductName"].ToString(),
                        WarehouseID = Convert.ToInt32(reader["WarehouseID"]),
                        WarehouseName = reader["WarehouseName"].ToString(),
                        TotalIn = Convert.ToInt32(reader["TotalIn"]),
                        TotalOut = Convert.ToInt32(reader["TotalOut"]),
                        CurrentStock = Convert.ToInt32(reader["CurrentStock"])
                    };

                    stockList.Add(stock);
                }
                reader.Close();
            }

            return Ok(stockList);
        }
    }
}
