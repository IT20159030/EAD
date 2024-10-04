/*
* VendorController.cs provides the API endpoints for the Vendor model.
* It contains the following routes:
*   1. GET /api/v1/Vendor: Get all vendors.
*   2. GET /api/v1/Vendor/{id}: Get a vendor by id.
*   3. POST /api/v1/Vendor/rating: Add a rating to a vendor.
*   4. PUT /api/v1/Vendor/rating: Update a rating for a vendor.
*/

using System.Security.Claims;
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
        Id = vendor.Id.ToString(),
        VendorRating = vendor.VendorRating,
        VendorRatingCount = vendor.VendorRatingCount,
        Reviews = vendor.Reviews.Select(review => new ReviewDto
        {
            Id = review.Id!,
            ReviewerId = review.ReviewerId,
            ReviewerName = review.ReviewerName,
            ReviewRating = review.ReviewRating,
            ReviewText = review.ReviewText
        }).ToList()
    };

    private Vendor ConvertToModel(UpdateVendorRequestDto dto) => new Vendor
    {
        Id = Guid.Parse(dto.Id),
        VendorRating = dto.VendorRating,
        VendorRatingCount = dto.VendorRatingCount,
        Reviews = dto.Reviews.Select(review => new Review
        {
            Id = review.Id,
            ReviewerId = review.ReviewerId,
            ReviewerName = review.ReviewerName,
            ReviewRating = review.ReviewRating,
            ReviewText = review.ReviewText
        }).ToList()

    };

    private Review ConvertToReviewModel(CreateReviewRequestDto dto) => new Review
    {
        Id = ObjectId.GenerateNewId().ToString(),
        ReviewerId = string.Empty,
        ReviewerName = string.Empty,
        ReviewRating = dto.ReviewRating,
        ReviewText = dto.ReviewText
    };

    [HttpGet("{id}", Name = "GetVendorDetails")]
    public async Task<IActionResult> GetVendor(string id)
    {
        var vendor = await _vendors.Find(v => v.Id == Guid.Parse(id)).FirstOrDefaultAsync();

        if (vendor == null)
        {
            return Ok("Vendor does not have any reviews yet.");
        }
        else
        {
            return Ok(ConvertToDto(vendor));
        }
    }

    [HttpPost("rating", Name = "CreateVendor")]
    [Authorize(Roles = "customer")]
    public async Task<IActionResult> AddRating([FromBody] CreateReviewRequestDto dto)
    {
        var vendor = await _vendors.Find(v => v.Id == Guid.Parse(dto.VendorId)).FirstOrDefaultAsync();

        if (vendor == null)
        {
            return NotFound("Vendor not found");
        }

        // create a new review
        var review = ConvertToReviewModel(dto);
        review.ReviewerId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
        review.ReviewerName = User.FindFirstValue(ClaimTypes.Email) ?? "anonymous";

        // add review to vendor
        vendor.Reviews.Add(review);

        // update vendor rating
        vendor.VendorRating = (vendor.VendorRating * vendor.VendorRatingCount + review.ReviewRating) / (vendor.VendorRatingCount + 1);
        vendor.VendorRatingCount++;

        await _vendors.InsertOneAsync(vendor);
        return Ok(ConvertToDto(vendor));
    }

    [HttpPut("rating", Name = "UpdateVendorRating")]
    [Authorize(Roles = "customer")]
    public async Task<IActionResult> UpdateRating([FromBody] UpdateReviewRequestDto dto)
    {
        var vendor = await _vendors.Find(v => v.Id == Guid.Parse(dto.VendorId)).FirstOrDefaultAsync();

        if (vendor == null)
        {
            return NotFound("Vendor not found");
        }

        var review = vendor.Reviews.Find(r => r.Id == dto.Id);

        if (review == null)
        {
            return NotFound("Review not found");
        }

        // update vendor rating
        vendor.VendorRating = ((vendor.VendorRating * vendor.VendorRatingCount) - review.ReviewRating + dto.ReviewRating) / vendor.VendorRatingCount;

        // update review
        review.ReviewRating = dto.ReviewRating;
        review.ReviewText = dto.ReviewText;

        await _vendors.ReplaceOneAsync(v => v.Id == Guid.Parse(dto.VendorId), vendor);
        return Ok(ConvertToDto(vendor));
    }

    [HttpDelete("rating/{vendorId}/{reviewId}", Name = "DeleteReview")]
    [Authorize(Roles = "customer")]
    public async Task<IActionResult> DeleteReview(string vendorId, string reviewId)
    {
        var vendor = await _vendors.Find(v => v.Id == Guid.Parse(vendorId)).FirstOrDefaultAsync();

        if (vendor == null)
        {
            return NotFound("Vendor not found");
        }

        var review = vendor.Reviews.Find(r => r.Id == reviewId);

        if (review == null)
        {
            return NotFound("Review not found");
        }

        vendor.Reviews.Remove(review);

        // update vendor rating
        vendor.VendorRating = ((vendor.VendorRating * vendor.VendorRatingCount) - review.ReviewRating) / (vendor.VendorRatingCount - 1);
        vendor.VendorRatingCount--;

        await _vendors.ReplaceOneAsync(v => v.Id == Guid.Parse(vendorId), vendor);
        return Ok(ConvertToDto(vendor));
    }

    [HttpPut("{id}", Name = "UpdateVendor")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateVendor(string id, [FromBody] UpdateVendorRequestDto dto)
    {
        var vendor = ConvertToModel(dto);
        var result = await _vendors.ReplaceOneAsync(v => v.Id == Guid.Parse(id), vendor);
        if (result.MatchedCount == 0)
        {
            return NotFound();
        }

        return Ok(ConvertToDto(vendor));
    }

}

