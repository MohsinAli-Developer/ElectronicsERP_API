using ElectronicsERP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ElectronicsERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly string _connectionString;

        public ProductsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // ✅ Get All Products
        [HttpGet]
        public IActionResult GetProducts()
        {
            List<ProductDTO> products = new List<ProductDTO>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Products", con);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    products.Add(new ProductDTO
                    {
                        ProductID = (int)rdr["ProductID"],
                        ModelNo = rdr["ModelNo"].ToString(),
                        ProductName = rdr["ProductName"].ToString(),
                        Brand = rdr["Brand"].ToString(),
                        Category = rdr["Category"].ToString(),
                        PurchasePrice = Convert.ToDecimal(rdr["PurchasePrice"]),
                        SalePrice = Convert.ToDecimal(rdr["SalePrice"]),
                        QuantityInStock = Convert.ToInt32(rdr["QuantityInStock"]),
                        ReorderLevel = Convert.ToInt32(rdr["ReorderLevel"]),
                        Warehousename = rdr["Name"].ToString()
                    });
                }
            }

            return Ok(products);
        }

        // ✅ Get Product by ID
        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            ProductDTO product = null;

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Products WHERE ProductID=@id", con);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.Read())
                {
                    product = new ProductDTO
                    {
                        ProductID = (int)rdr["ProductID"],
                        ModelNo = rdr["ModelNo"].ToString(),
                        ProductName = rdr["ProductName"].ToString(),
                        Brand = rdr["Brand"].ToString(),
                        Category = rdr["Category"].ToString(),
                        PurchasePrice = Convert.ToDecimal(rdr["PurchasePrice"]),
                        SalePrice = Convert.ToDecimal(rdr["SalePrice"]),
                        QuantityInStock = Convert.ToInt32(rdr["QuantityInStock"]),
                        ReorderLevel = Convert.ToInt32(rdr["ReorderLevel"]),
                        Warehousename = rdr["WarehouseName"].ToString()
                    };
                }
            }

            if (product == null) return NotFound();
            return Ok(product);
        }

        // ✅ Add Product
        [HttpPost]
        public IActionResult AddProduct([FromBody] ProductDTO product)
        {
            using(SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO Products (ModelNo, ProductName, Brand, Category, PurchasePrice, SalePrice, QuantityInStock, ReorderLevel, Name) 
                    VALUES (@ModelNo, @ProductName, @Brand, @Category, @PurchasePrice, @SalePrice, @QuantityInStock, @ReorderLevel, @WarehouseName)", con);

                cmd.Parameters.AddWithValue("@ModelNo", product.ModelNo);
                cmd.Parameters.AddWithValue("@ProductName", product.ProductName);
                cmd.Parameters.AddWithValue("@Brand", product.Brand);
                cmd.Parameters.AddWithValue("@Category", product.Category);
                cmd.Parameters.AddWithValue("@PurchasePrice", product.PurchasePrice);
                cmd.Parameters.AddWithValue("@SalePrice", product.SalePrice);
                cmd.Parameters.AddWithValue("@QuantityInStock", product.QuantityInStock);
                cmd.Parameters.AddWithValue("@ReorderLevel", product.ReorderLevel);
                cmd.Parameters.AddWithValue("@WarehouseName", (object?)product.Warehousename ?? DBNull.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return Ok(new { message = "Product added successfully" });
        }

        // ✅ Update Product
        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] ProductDTO product)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(@"
                    UPDATE Products 
                    SET ModelNo=@ModelNo, ProductName=@ProductName, Brand=@Brand, Category=@Category, 
                        PurchasePrice=@PurchasePrice, SalePrice=@SalePrice, QuantityInStock=@QuantityInStock, 
                        ReorderLevel=@ReorderLevel, Name=@WarehouseName
                    WHERE ProductID=@ProductID", con);

                cmd.Parameters.AddWithValue("@ProductID", id);
                cmd.Parameters.AddWithValue("@ModelNo", product.ModelNo);
                cmd.Parameters.AddWithValue("@ProductName", product.ProductName);
                cmd.Parameters.AddWithValue("@Brand", product.Brand);
                cmd.Parameters.AddWithValue("@Category", product.Category);
                cmd.Parameters.AddWithValue("@PurchasePrice", product.PurchasePrice);
                cmd.Parameters.AddWithValue("@SalePrice", product.SalePrice);
                cmd.Parameters.AddWithValue("@QuantityInStock", product.QuantityInStock);
                cmd.Parameters.AddWithValue("@ReorderLevel", product.ReorderLevel);
                cmd.Parameters.AddWithValue("@WarehouseName", (object?)product.Warehousename ?? DBNull.Value);

                con.Open();
                int rows = cmd.ExecuteNonQuery();
                if (rows == 0) return NotFound();
            }

            return Ok(new { message = "Product updated successfully" });
        }

        // ✅ Delete Product
        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM Products WHERE ProductID=@id", con);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                int rows = cmd.ExecuteNonQuery();
                if (rows == 0) return NotFound();
            }

            return Ok(new { message = "Product deleted successfully" });
        }
    }
}
