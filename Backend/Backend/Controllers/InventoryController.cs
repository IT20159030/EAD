using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson; 

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IMongoCollection<Inventory> _inventory;
    private readonly IMongoCollection<Product>? _products;
    private readonly ILogger<InventoryController> _logger;

    public InventoryController(ILogger<InventoryController> logger, MongoDBService mongoDBService)
    {
        _logger = logger;
        _inventory = mongoDBService.Database.GetCollection<Inventory>("Inventory");
    }

    [HttpPost(Name = "AddInventoryByProductId")]
    public async Task<IActionResult> Post([FromBody] Inventory inventory)
    {
        var product = await _products.Find(p => p.Id == inventory.ProductId).FirstOrDefaultAsync();
        if (product == null)
        {
            return BadRequest("Invalid ProductId: Product does not exist.");
        }

        await _inventory.InsertOneAsync(inventory);
        return CreatedAtAction(nameof(Get), new { id = inventory.Id }, inventory);
    }

    [HttpGet(Name = "GetInventoryByProductId")]
    public async Task<IEnumerable<Inventory>> Get(string productId)
    {
        return await _inventory.Find(i => i.ProductId == productId).ToListAsync();
    }


    [HttpPut("{id}", Name = "UpdateInventoryByProductId")]
    public async Task<IActionResult> Put(string id, [FromBody] Inventory inventory)
    {
        await _inventory.ReplaceOneAsync(i => i.Id == id, inventory);
        return NoContent();
    }

    [HttpDelete("{id}", Name = "DeleteInventoryByProductId")]
    public async Task<IActionResult> Delete(string id)
    {
        await _inventory.DeleteOneAsync(i => i.Id == id);
        return NoContent();
    }

    [HttpPut("{id}/lowStockAlert", Name = "SetLowStockAlert")]
    public async Task<IActionResult> SetLowStockAlert(string id, bool lowStockAlert)
    {
        var update = Builders<Inventory>.Update.Set(i => i.LowStockAlert, lowStockAlert);
        await _inventory.UpdateOneAsync(i => i.Id == id, update);
        return NoContent();
    }

}
    