using Backend.Models;
using Backend.Dtos;
using Backend.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Backend.Controllers
{
    /*
    * This controller is responsible for handling requests related to product categories.
    */
    
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProductCategoryController : ControllerBase
    {
        private readonly IMongoCollection<ProductCategory> _productCategories;
        private readonly ILogger<ProductCategoryController> _logger;

        public ProductCategoryController(ILogger<ProductCategoryController> logger, MongoDBService mongoDBService)
        {
            _logger = logger;
            _productCategories = mongoDBService.Database.GetCollection<ProductCategory>("ProductCategories");
        }

        [HttpPost(Name = "CreateProductCategory")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Post([FromBody] CreateProductCategoryRequestDto dto)
        {
            var productCategory = new ProductCategory
            {
                Name = dto.Name
            };

            await _productCategories.InsertOneAsync(productCategory);

            var responseDto = new ProductCategoryDto
            {
                Id = productCategory.Id!,
                Name = productCategory.Name
            };

            return CreatedAtAction(nameof(Get), new { id = productCategory.Id }, responseDto);
        }

        [HttpGet(Name = "GetAllProductCategories")]
        public async Task<IEnumerable<ProductCategoryDto>> Get()
        {
            var categories = await _productCategories.Find(new BsonDocument()).ToListAsync();
            return categories.Select(c => new ProductCategoryDto
            {
                Id = c.Id!,
                Name = c.Name
            });
        }

        [HttpGet("{id}", Name = "GetProductCategory")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Get(string id)
        {
            var category = await _productCategories.Find(p => p.Id == id).FirstOrDefaultAsync();
            if (category == null) return NotFound();

            var responseDto = new ProductCategoryDto
            {
                Id = category.Id!,
                Name = category.Name
            };

            return Ok(responseDto);
        }

        [HttpPut("{id}", Name = "UpdateProductCategory")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Put(string id, [FromBody] UpdateProductCategoryRequestDto dto)
        {
            var existingCategory = await _productCategories.Find(p => p.Id == id).FirstOrDefaultAsync();
            if (existingCategory == null) return NotFound();

            existingCategory.Name = dto.Name;

            await _productCategories.ReplaceOneAsync(p => p.Id == id, existingCategory);

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteProductCategory")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _productCategories.DeleteOneAsync(p => p.Id == id);
            if (result.DeletedCount == 0) return NotFound();

            return NoContent();
        }
    }
}
