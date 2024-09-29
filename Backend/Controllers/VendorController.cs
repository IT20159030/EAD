/*
* VendorController.cs provides the API endpoints for the Vendor model.
* It contains the following routes:
*   1. POST /api/v1/Vendor: Create a new vendor.
*   2. GET /api/v1/Vendor: Get all vendors.
*   3. GET /api/v1/Vendor/{id}: Get a vendor by id.
*   4. PUT /api/v1/Vendor/{id}: Update a vendor by id.
*   5. DELETE /api/v1/Vendor/{id}: Delete a vendor by id.
*/

using Backend.Dtos;
using Backend.Models;
using Backend.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "admin,vendor,csr")]
public class VendorController : ControllerBase
{
    private readonly IMongoCollection<Vendor> _vendors;
    private readonly ILogger<VendorController> _logger;

    public VendorController(ILogger<VendorController> logger, MongoDBService mongoDBService)
    {
        _logger = logger;
        _vendors = mongoDBService.Database.GetCollection<Vendor>("Vendors");
    }

    private VendorDto ConvertToDto(Vendor vendor) => new VendorDto
    {
        Id = vendor.Id!,
        VendorId = vendor.VendorId,
        VendorName = vendor.VendorName,
        VendorEmail = vendor.VendorEmail,
        VendorPhone = vendor.VendorPhone,
        VendorAddress = vendor.VendorAddress,
        VendorCity = vendor.VendorCity,
        VendorRating = vendor.VendorRating,
        VendorRatingCount = vendor.VendorRatingCount
    };

    private Vendor ConvertToModel(CreateVendorRequestDto dto) => new Vendor
    {
        Id = ObjectId.GenerateNewId().ToString(),
        VendorId = dto.VendorId,
        VendorName = dto.VendorName,
        VendorEmail = dto.VendorEmail,
        VendorPhone = dto.VendorPhone,
        VendorAddress = dto.VendorAddress,
        VendorCity = dto.VendorCity,
        VendorRating = dto.VendorRating,
        VendorRatingCount = dto.VendorRatingCount
    };

    private Vendor ConvertToModel(UpdateVendorRequestDto dto) => new Vendor
    {
        Id = dto.Id,
        VendorId = dto.VendorId,
        VendorName = dto.VendorName,
        VendorEmail = dto.VendorEmail,
        VendorPhone = dto.VendorPhone,
        VendorAddress = dto.VendorAddress,
        VendorCity = dto.VendorCity,
        VendorRating = dto.VendorRating,
        VendorRatingCount = dto.VendorRatingCount
    };

    [HttpPost(Name = "CreateVendor")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateVendor([FromBody] CreateVendorRequestDto dto)
    {
        var vendor = ConvertToModel(dto);
        await _vendors.InsertOneAsync(vendor);
        return CreatedAtRoute("GetVendor", new { id = vendor.Id }, ConvertToDto(vendor));
    }

    [HttpGet(Name = "GetAllVendors")]
    public async Task<IEnumerable<VendorDto>> GetAllVendors()
    {
        var vendors = await _vendors.Find(new BsonDocument()).ToListAsync();
        return vendors.Select(ConvertToDto);
    }

    [HttpGet("{id}", Name = "GetVendor")]
    public async Task<IActionResult> GetVendor(string id)
    {
        var vendor = await _vendors.Find(v => v.Id == id).FirstOrDefaultAsync();
        if (vendor == null)
        {
            return NotFound();
        }

        return Ok(ConvertToDto(vendor));
    }

    [HttpPut("{id}", Name = "UpdateVendor")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateVendor(string id, [FromBody] UpdateVendorRequestDto dto)
    {
        var vendor = ConvertToModel(dto);
        var result = await _vendors.ReplaceOneAsync(v => v.Id == id, vendor);
        if (result.MatchedCount == 0)
        {
            return NotFound();
        }

        return Ok(ConvertToDto(vendor));
    }

    [HttpDelete("{id}", Name = "DeleteVendor")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteVendor(string id)
    {
        var result = await _vendors.DeleteOneAsync(v => v.Id == id);
        if (result.DeletedCount == 0)
        {
            return NotFound();
        }

        return NoContent();
    }


}

