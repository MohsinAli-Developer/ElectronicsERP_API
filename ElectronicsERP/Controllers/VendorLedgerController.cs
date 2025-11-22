using ElectronicsERP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ElectronicsERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorLedgersController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public VendorLedgersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string GetConnectionString()
        {
            return _configuration.GetConnectionString("DefaultConnection");
        }

        // ✅ Get all vendor ledgers
        [HttpGet]
        public ActionResult<IEnumerable<VendorLedgerDto>> GetAll()
        {
            var list = new List<VendorLedgerDto>();

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                string query = @"
            SELECT v.LedgerID, v.VendorID, Ve.Name AS VendorName, 
                   v.Date, v.TransactionType, v.DebitAmount, v.CreditAmount, v.Balance
            FROM VendorLedgers v
            INNER JOIN Vendors Ve ON v.VendorID = Ve.VendorID
            ORDER BY v.Date DESC"; // ✅ Optional: order by date (ledger-style)
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new VendorLedgerDto
                    {
                        LedgerID = (int)reader["LedgerID"],
                        VendorID = (int)reader["VendorID"],
                        VendorName = (string)reader["VendorName"],
                        Date = (DateTime)reader["Date"],
                        TransactionType = reader["TransactionType"].ToString(),
                        DebitAmount = (decimal)reader["DebitAmount"],
                        CreditAmount = (decimal)reader["CreditAmount"],
                        Balance = (decimal)reader["Balance"]
                    });
                }
            }

            return Ok(list);
        }

        // ✅ Get ledger by ID
        [HttpGet("{id}")]
        public ActionResult<VendorLedgerDto> GetById(int id)
        {
            VendorLedgerDto ledger = null;

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                string query = "SELECT * FROM VendorLedgers WHERE LedgerID=@id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    ledger = new VendorLedgerDto
                    {
                        LedgerID = (int)reader["LedgerID"],
                        VendorID = (int)reader["VendorID"],
                        Date = (DateTime)reader["Date"],
                        TransactionType = reader["TransactionType"].ToString(),
                        DebitAmount = (decimal)reader["DebitAmount"],
                        CreditAmount = (decimal)reader["CreditAmount"],
                        Balance = (decimal)reader["Balance"]
                    };
                }
            }

            if (ledger == null) return NotFound();
            return Ok(ledger);
        }

        // ✅ Add new ledger
        [HttpPost]
        public IActionResult Add([FromBody] VendorLedgerDto ledger)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                string query = @"INSERT INTO VendorLedgers (VendorID, TransactionType, DebitAmount, CreditAmount, Balance) 
                                 VALUES (@VendorID, @TransactionType, @DebitAmount, @CreditAmount, @Balance)";
                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@VendorID", ledger.VendorID);
                cmd.Parameters.AddWithValue("@TransactionType", ledger.TransactionType);
                cmd.Parameters.AddWithValue("@DebitAmount", ledger.DebitAmount);
                cmd.Parameters.AddWithValue("@CreditAmount", ledger.CreditAmount);
                cmd.Parameters.AddWithValue("@Balance", ledger.Balance);

                cmd.ExecuteNonQuery();
            }

            return Ok(new { message = "Ledger added successfully" });
        }

        // ✅ Update ledger
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] VendorLedgerDto ledger)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                string query = @"UPDATE VendorLedgers 
                                 SET VendorID=@VendorID, TransactionType=@TransactionType, 
                                     DebitAmount=@DebitAmount, CreditAmount=@CreditAmount, Balance=@Balance 
                                 WHERE LedgerID=@LedgerID";
                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@LedgerID", id);
                cmd.Parameters.AddWithValue("@VendorID", ledger.VendorID);
                cmd.Parameters.AddWithValue("@TransactionType", ledger.TransactionType);
                cmd.Parameters.AddWithValue("@DebitAmount", ledger.DebitAmount);
                cmd.Parameters.AddWithValue("@CreditAmount", ledger.CreditAmount);
                cmd.Parameters.AddWithValue("@Balance", ledger.Balance);

                int rows = cmd.ExecuteNonQuery();
                if (rows == 0) return NotFound();
            }

            return Ok(new { message = "Ledger updated successfully" });
        }

        // ✅ Delete ledger
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                string query = "DELETE FROM VendorLedgers WHERE LedgerID=@id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);

                int rows = cmd.ExecuteNonQuery();
                if (rows == 0) return NotFound();
            }

            return Ok(new { message = "Ledger deleted successfully" });
        }
    }
}
