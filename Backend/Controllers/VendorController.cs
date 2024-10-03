/*
* VendorController.cs provides the API endpoints for the Vendor model.
* It contains the following routes:
*   1. GET /api/v1/Vendor: Get all vendors.
*   2. GET /api/v1/Vendor/{id}: Get a vendor by id.
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
        VendorName = vendor.VendorName,
        VendorEmail = vendor.VendorEmail,
        VendorPhone = vendor.VendorPhone,
        VendorAddress = vendor.VendorAddress,
        VendorCity = vendor.VendorCity,
        VendorRating = vendor.VendorRating,
        VendorRatingCount = vendor.VendorRatingCount
    };


    [HttpGet(Name = "GetAllVendors")]
    public async Task<IEnumerable<VendorDto>> GetAllVendors()
    {
        var vendors = await _vendors.Find(new BsonDocument()).ToListAsync();
        return vendors.Select(ConvertToDto);
    }

    [HttpGet("{id}", Name = "GetVendor")]
    public async Task<IActionResult> GetVendor(string id)
    {
        var vendor = await _vendors.Find(v => v.Id == Guid.Parse(id)).FirstOrDefaultAsync();
        if (vendor == null)
        {
            return NotFound();
        }

        return Ok(ConvertToDto(vendor));
    }

}

