using Backend.Dtos;
using Backend.Models;
using Backend.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IMongoCollection<Product> _products;
        private readonly IMongoCollection<ProductCategory> _productsCategory;
        private readonly IMongoCollection<User> _users;

        private readonly ILogger<ProductController> _logger;

        public ProductController(ILogger<ProductController> logger, MongoDBService mongoDBService)
        {
            _logger = logger;
            _products = mongoDBService.Database.GetCollection<Product>("Products");
            _productsCategory = mongoDBService.Database.GetCollection<ProductCategory>("ProductCategories");
            _users = mongoDBService.Database.GetCollection<User>("Users");
        }

        private ProductDto ConvertToDto(Product product) => new ProductDto
        {
            Id = product.Id!,
            Name = product.Name,
            Image = product.Image,
            Category = product.Category,
            CategoryName = string.Empty,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            IsActive = product.IsActive,
            VendorId = product.VendorId,
            VendorName = string.Empty
        };

        private Product ConvertToModel(CreateProductRequestDto dto) => new Product
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Name = dto.Name,
            Image = dto.Image,
            Category = dto.Category,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            IsActive = true,
            VendorId = dto.VendorId
        };


        private Product ConvertToModel(UpdateProductRequestDto dto) => new Product
        {
            Id = dto.Id,
            Name = dto.Name,
            Image = dto.Image,
            Category = dto.Category,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            IsActive = dto.IsActive,
            VendorId = dto.VendorId
        };

        private async Task<ProductDto> ConvertToDtoAsync(Product product)
        {
            var category = await _productsCategory.Find(c => c.Id == product.Category).FirstOrDefaultAsync();
            var categoryName = category != null ? category.Name : string.Empty;

            var vendor = await _users.Find(u => u.Id.ToString() == product.VendorId).FirstOrDefaultAsync();
            var vendorName = vendor != null ? vendor.Name : string.Empty;

            return new ProductDto
            {
                Id = product.Id!,
                Name = product.Name,
                Image = product.Image,
                Category = product.Category,
                CategoryName = categoryName,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                IsActive = product.IsActive,
                VendorId = product.VendorId,
                VendorName = vendorName
            };
        }


        [HttpPost(Name = "CreateProduct")]
        [Authorize(Roles = "admin, vendor")]
        public async Task<IActionResult> Post([FromBody] CreateProductRequestDto dto)
        {
            var product = ConvertToModel(dto);
            await _products.InsertOneAsync(product);
            return CreatedAtAction(nameof(Get), new { id = product.Id }, ConvertToDto(product));
        }

        [HttpGet(Name = "GetAllProducts")]
        [Authorize(Roles = "admin, vendor")]
        public async Task<IEnumerable<ProductDto>> Get()
        {
            var products = await _products.Find(new BsonDocument()).ToListAsync();
            return await Task.WhenAll(products.Select(ConvertToDtoAsync));
        }

        [HttpGet("active", Name = "GetActiveProducts")]
        [Authorize(Roles = "admin, vendor")]
        public async Task<IEnumerable<ProductDto>> GetActive()
        {
            var products = await _products.Find(p => p.IsActive).ToListAsync();
            return products.Select(ConvertToDto);
        }

        [HttpGet("{id}", Name = "GetProduct")]
        public async Task<ActionResult<ProductDto>> Get(string id)
        {
            var product = await _products.Find(p => p.Id == id).FirstOrDefaultAsync();
            if (product == null)
                return NotFound();
            return Ok(ConvertToDto(product));
        }

        [HttpGet("vendor/{vendorId}", Name = "GetProductsByVendor")]
        [Authorize(Roles = "admin, vendor")]
        public async Task<IEnumerable<ProductDto>> GetByVendor(string vendorId)
        {
            var products = await _products.Find(p => p.VendorId == vendorId).ToListAsync();
            return products.Select(ConvertToDto);
        }

        [HttpGet("vendor/{vendorId}/active", Name = "GetActiveProductsByVendor")]
        public async Task<IEnumerable<ProductDto>> GetActiveByVendor(string vendorId)
        {
            var products = await _products.Find(p => p.VendorId == vendorId && p.IsActive).ToListAsync();
            return products.Select(ConvertToDto);
        }

        [HttpGet("vendor/{vendorId}/inactive", Name = "GetInactiveProductsByVendor")]
        [Authorize(Roles = "admin, vendor")]
        public async Task<IEnumerable<ProductDto>> GetInactiveByVendor(string vendorId)
        {
            var products = await _products.Find(p => p.VendorId == vendorId && !p.IsActive).ToListAsync();
            return products.Select(ConvertToDto);
        }

        [HttpGet("category/{categoryId}/active", Name = "GetActiveProductsByCategory")]
        public async Task<IEnumerable<ProductDto>> GetActiveByCategory(string categoryId)
        {
            var products = await _products.Find(p => p.Category == categoryId && p.IsActive).ToListAsync();
            return products.Select(ConvertToDto);
        }

        [HttpGet("search", Name = "SearchProducts")]
        public async Task<IEnumerable<ProductDto>> Search([FromQuery] string query)
        {
            var products = await _products.Find(p => p.Name.ToLower().Contains(query.ToLower())).ToListAsync();
            return products.Select(ConvertToDto);
        }

        [HttpPut("{id}", Name = "UpdateProduct")]
        [Authorize(Roles = "admin, vendor")]
        public async Task<IActionResult> Put(string id, [FromBody] UpdateProductRequestDto dto)
        {
            var existingProduct = await _products.Find(p => p.Id == id).FirstOrDefaultAsync();
            if (existingProduct == null)
                return NotFound();

            var updatedProduct = ConvertToModel(dto);
            await _products.ReplaceOneAsync(p => p.Id == id, updatedProduct);
            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteProduct")]
        [Authorize(Roles = "admin, vendor")]
        public async Task<IActionResult> Delete(string id)
        {
            await _products.DeleteOneAsync(p => p.Id == id);
            return NoContent();
        }

        [HttpPut("{id}/activate", Name = "ActivateProduct")]
        [Authorize(Roles = "admin, vendor")]
        public async Task<IActionResult> Activate(string id)
        {
            var update = Builders<Product>.Update.Set(p => p.IsActive, true);
            await _products.UpdateOneAsync(p => p.Id == id, update);
            return NoContent();
        }

        [HttpPut("{id}/deactivate", Name = "DeactivateProduct")]
        [Authorize(Roles = "admin, vendor")]
        public async Task<IActionResult> Deactivate(string id)
        {
            var update = Builders<Product>.Update.Set(p => p.IsActive, false);
            await _products.UpdateOneAsync(p => p.Id == id, update);
            return NoContent();
        }
    }
}
