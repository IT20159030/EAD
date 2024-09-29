using Backend.Models;
using Backend.Dtos;
using Backend.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IMongoCollection<Inventory> _inventory;
        private readonly IMongoCollection<Product>? _products;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(ILogger<InventoryController> logger, MongoDBService mongoDBService)
        {
            _logger = logger;
            _inventory = mongoDBService.Database.GetCollection<Inventory>("Inventory");
            _products = mongoDBService.Database.GetCollection<Product>("Product");
        }

        [HttpPost(Name = "AddInventoryByProductId")]
        [Authorize(Roles = "Admin, Vendor")]
        public async Task<IActionResult> Post([FromBody] AddInventoryByProductIdDto dto)
        {
            var product = await _products.Find(p => p.Id == dto.ProductId).FirstOrDefaultAsync();
            if (product == null)
            {
                return BadRequest("Invalid ProductId: Product does not exist.");
            }

            var inventory = new Inventory
            {
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                AlertThreshold = dto.AlertThreshold,
                VendorId = dto.VendorId
            };

            await _inventory.InsertOneAsync(inventory);

            var responseDto = new InventoryDto
            {
                Id = inventory.Id!,
                ProductId = inventory.ProductId,
                Quantity = inventory.Quantity,
                AlertThreshold = inventory.AlertThreshold,
                LowStockAlert = inventory.LowStockAlert,
                VendorId = inventory.VendorId
            };

            return CreatedAtAction(nameof(Get), new { id = inventory.Id }, responseDto);
        }

        [HttpGet(Name = "GetAllInventories")]
        [Authorize(Roles = "Admin, Vendor")]
        public async Task<IEnumerable<InventoryDto>> Get()
        {
            var inventories = await _inventory.Find(new BsonDocument()).ToListAsync();
            return inventories.Select(i => new InventoryDto
            {
                Id = i.Id!,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                AlertThreshold = i.AlertThreshold,
                LowStockAlert = i.LowStockAlert,
                VendorId = i.VendorId
            });
        }

        [HttpGet("{id}", Name = "GetInventoryById")]
        [Authorize(Roles = "Admin, Vendor")]
        public async Task<IActionResult> Get(string id)
        {
            var inventory = await _inventory.Find(i => i.Id == id).FirstOrDefaultAsync();
            if (inventory == null) return NotFound();

            var responseDto = new InventoryDto
            {
                Id = inventory.Id!,
                ProductId = inventory.ProductId,
                Quantity = inventory.Quantity,
                AlertThreshold = inventory.AlertThreshold,
                LowStockAlert = inventory.LowStockAlert,
                VendorId = inventory.VendorId
            };

            return Ok(responseDto);
        }

        [HttpGet("product/{productId}", Name = "GetInventoryByProductId")]
        public async Task<IEnumerable<InventoryDto>> GetByProductId(string productId)
        {
            var inventories = await _inventory.Find(i => i.ProductId == productId).ToListAsync();
            return inventories.Select(i => new InventoryDto
            {
                Id = i.Id!,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                AlertThreshold = i.AlertThreshold,
                LowStockAlert = i.LowStockAlert,
                VendorId = i.VendorId
            });
        }

        [HttpPut("{id}", Name = "UpdateInventoryById")]
        [Authorize(Roles = "Admin, Vendor")]
        public async Task<IActionResult> Put(string id, [FromBody] UpdateInventoryByProductIdDto dto)
        {
            var existingInventory = await _inventory.Find(i => i.Id == id).FirstOrDefaultAsync();
            if (existingInventory == null) return NotFound();

            var update = Builders<Inventory>.Update
                .Set(i => i.Quantity, dto.Quantity)
                .Set(i => i.AlertThreshold, dto.AlertThreshold);

            var result = await _inventory.UpdateOneAsync(i => i.Id == id, update);
            if (result.ModifiedCount == 0) return NotFound();

            return NoContent();
        }

        [HttpPut("{id}/lowStockAlert", Name = "SetLowStockAlert")]
        [Authorize(Roles = "Admin, Vendor")]
        public async Task<IActionResult> SetLowStockAlert(string id, [FromBody] SetLowStockAlertDto dto)
        {
            var update = Builders<Inventory>.Update.Set(i => i.LowStockAlert, dto.LowStockAlert);
            var result = await _inventory.UpdateOneAsync(i => i.Id == id, update);
            if (result.ModifiedCount == 0) return NotFound();

            return NoContent();
        }
    }
}
