using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductCatergoryController : ControllerBase
{
    private readonly IMongoCollection<ProductCatergory> _productCatergories;
    private readonly ILogger<ProductCatergoryController> _logger;

    public ProductCatergoryController(ILogger<ProductCatergoryController> logger, MongoDBService mongoDBService)
    {
        _logger = logger;
        _productCatergories = mongoDBService.Database.GetCollection<ProductCatergory>("ProductCatergories");
    }

    [HttpPost(Name = "CreateProductCatergory")]
    public async Task<IActionResult> Post([FromBody] ProductCatergory productCatergory)
    {
        await _productCatergories.InsertOneAsync(productCatergory);
        return CreatedAtAction(nameof(Get), new { id = productCatergory.Id }, productCatergory);
    }

    [HttpGet(Name = "GetAllProductCatergories")]
    public async Task<IEnumerable<ProductCatergory>> Get()
    {
        return await _productCatergories.Find(new BsonDocument()).ToListAsync();
    }

    [HttpGet("{id}", Name = "GetProductCatergory")]
    public async Task<ProductCatergory> Get(string id)
    {
        return await _productCatergories.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    [HttpPut("{id}", Name = "UpdateProductCatergory")]
    public async Task<IActionResult> Put(string id, [FromBody] ProductCatergory productCatergory)
    {
        await _productCatergories.ReplaceOneAsync(p => p.Id == id, productCatergory);
        return NoContent();
    }

    [HttpDelete("{id}", Name = "DeleteProductCatergory")]
    public async Task<IActionResult> Delete(string id)
    {
        await _productCatergories.DeleteOneAsync(p => p.Id == id);
        return NoContent();
    }

}
