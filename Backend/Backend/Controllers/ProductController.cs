using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly IMongoCollection<Product> _products;
    private readonly ILogger<ProductController> _logger;

    public ProductController(ILogger<ProductController> logger, MongoDBService mongoDBService)
    {
        _logger = logger;
        _products = mongoDBService.Database.GetCollection<Product>("Products");
    }

    [HttpPost(Name = "CreateProduct")]
    public async Task<IActionResult> Post([FromBody] Product product)
    {
        await _products.InsertOneAsync(product);
        return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
    }

    [HttpGet(Name = "GetAllProducts")]
    public async Task<IEnumerable<Product>> Get()
    {
        return await _products.Find(new BsonDocument()).ToListAsync();
    }

    [HttpGet("{id}", Name = "GetProduct")]
    public async Task<Product> Get(string id)
    {
        return await _products.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    [HttpPut("{id}", Name = "UpdateProduct")]
    public async Task<IActionResult> Put(string id, [FromBody] Product product)
    {
        await _products.ReplaceOneAsync(p => p.Id == id, product);
        return NoContent();
    }

    [HttpDelete("{id}", Name = "DeleteProduct")]
    public async Task<IActionResult> Delete(string id)
    {
        await _products.DeleteOneAsync(p => p.Id == id);
        return NoContent();
    }

    [HttpPut("{id}/activate", Name = "ActivateProduct")]
    public async Task<IActionResult> Activate(string id)
    {
        var update = Builders<Product>.Update.Set(p => p.IsActive, true);
        await _products.UpdateOneAsync(p => p.Id == id, update);
        return NoContent();
    }

    [HttpPut("{id}/deactivate", Name = "DeactivateProduct")]
    public async Task<IActionResult> Deactivate(string id)
    {
        var update = Builders<Product>.Update.Set(p => p.IsActive, false);
        await _products.UpdateOneAsync(p => p.Id == id, update);
        return NoContent();
    }
}
