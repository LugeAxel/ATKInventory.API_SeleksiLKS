using ATKInventory.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection.PortableExecutable;

namespace ATKInventory.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public ItemsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Getitems()
        {
            var connectionString = _configuration.GetConnectionString("ATKInventoryConnection");
            var items = new List<object>();
            using var connection = new SqlConnection(connectionString);

            const string query = "SELECT * FROM items ORDER BY id";

            using var command = new SqlCommand(query, connection);

            connection.Open();

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                items.Add(new
                {
                    id = reader.GetInt32(0),
                    ItemCode = reader.GetString(1),
                    ItemName = reader.GetString(2),
                    Category = reader.GetString(3),
                    Price = reader.GetDecimal(4),
                    Stock = reader.GetInt32(5)

                });
            }
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public IActionResult Getitembyid(int id) {
            var connectionString = _configuration.GetConnectionString("ATKInventoryConnection");
            var items = new List<object>();
            using var connection = new SqlConnection(connectionString);

            const string query = "SELECT * FROM items WHERE id = @id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;

            connection.Open();

            using var reader = command.ExecuteReader();


            if (!reader.Read())
            {
                return NotFound(new
                {
                    message = $"Item dengan {id} Tidak ditemukan"
                });
            }
            items.Add(new
            {
                id = reader.GetInt32(0),
                ItemCode = reader.GetString(1),
                ItemName = reader.GetString(2),
                Category = reader.GetString(3),
                Price = reader.GetDecimal(4),
                Stock = reader.GetInt32(5)

            });
            return Ok(items);

        }

        [HttpPost]

        public IActionResult CreateItem([FromBody] ItemsRequest request)
        {
            var connectionString = _configuration.GetConnectionString("ATKInventoryConnection");
            var items = new List<object>();
            using var connection = new SqlConnection(connectionString);

            const string query = "INSERT INTO items (ItemCode, ItemName,Category,Price,Stock) VALUES(@ItemCode, @ItemName , @Category, @Price,@Stock);";

            using var command = new SqlCommand(query, connection);
            command.Parameters.Add("@ItemCode", SqlDbType.VarChar).Value = request.ItemCode;
            command.Parameters.Add("@ItemName", SqlDbType.VarChar).Value = request.ItemName;
            command.Parameters.Add("@Category", SqlDbType.VarChar).Value = request.Category;
            command.Parameters.Add("@Price", SqlDbType.Decimal).Value = request.Price;
            command.Parameters.Add("@Stock", SqlDbType.Int).Value = request.Stock;
            connection.Open();

            var newid = Convert.ToInt32(command.ExecuteScalar());

            return CreatedAtAction(
            nameof(Getitembyid),
            new { id = newid },
            new
            {
                id = newid,
                ItemCode = request.ItemCode,
                ItemName = request.ItemName,
                Category = request.Category,
                Price = request.Price,
                Stock = request.Stock
            });
        }
        [HttpPut("{id:int}")]
        public IActionResult EditById(int id, [FromBody] ItemsRequest request)
        {
            var connectionString = _configuration.GetConnectionString("ATKInventoryConnection");
            using var connection = new SqlConnection(connectionString);

            const string query = "UPDATE items SET ItemCode = @ItemCode,ItemName = @ItemName,Category = @Category, Price = @Price, Stock = @Stock WHERE id = @id;";

            using var command = new SqlCommand(query, connection);
            command.Parameters.Add("@ItemCode", SqlDbType.VarChar).Value = request.ItemCode;
            command.Parameters.Add("@ItemName", SqlDbType.VarChar).Value = request.ItemName;
            command.Parameters.Add("@Category", SqlDbType.VarChar).Value = request.Category;
            command.Parameters.Add("@Price", SqlDbType.Decimal).Value = request.Price;
            command.Parameters.Add("@Stock", SqlDbType.Int).Value = request.Stock;
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;

            connection.Open();

            var rowsAffected = command.ExecuteNonQuery();


            if (rowsAffected == 0)
            {
                return NotFound(new
                {
                    message = $"Item dengan {id} Tidak ditemukan"
                });
            }
            return Ok(new
            {
                message = "Data item berhasil diperbarui",
                id
            });
        }

        [HttpDelete("{id:int}")]

        public IActionResult DeleteById(int id)
        {
            var connectionString = _configuration.GetConnectionString("ATKInventoryConnection");
            using var connection = new SqlConnection(connectionString);

            const string query = "DELETE FROM items WHERE id = @id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;

            connection.Open();

            var rowsAffected = command.ExecuteNonQuery();


            if (rowsAffected == 0)
            {
                return NotFound(new
                {
                    message = $"Item dengan {id} Tidak ditemukan"
                });
            }
            return Ok(new
            {
                message = "Data item berhasil dihapus",
                id
            });
        }
       
    }
}
