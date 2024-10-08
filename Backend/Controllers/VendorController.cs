/*
* VendorController.cs provides the API endpoints for the Vendor model.
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
[Authorize]
public class VendorController : ControllerBase
{
    private readonly IMongoCollection<Vendor> _vendors;
    private readonly ILogger<VendorController> _logger;

    public VendorController(ILogger<VendorController> logger, MongoDBService mongoDBService)
    {
        _logger = logger;
        _vendors = mongoDBService.Database.GetCollection<Vendor>("Vendors");
    }

    // Convert Vendor model to VendorDto
    private VendorDto ConvertToDto(Vendor vendor) => new VendorDto
    {
        Id = vendor.Id.ToString(),
        VendorName = vendor.VendorName,
        VendorEmail = vendor.VendorEmail,
        VendorPhone = vendor.VendorPhone,
        VendorAddress = vendor.VendorAddress,
        VendorCity = vendor.VendorCity,
        VendorRating = vendor.VendorRating,
        VendorRatingCount = vendor.VendorRatingCount,
        Reviews = vendor.Reviews.Select(review => new ReviewDto
        {
            Id = review.Id!,
            ReviewerId = review.ReviewerId,
            ProductId = review.ProductId,
            ReviewerName = review.ReviewerName,
            ReviewRating = review.ReviewRating,
            ReviewText = review.ReviewText
        }).ToList()
    };

    // Convert UpdateVendorRequestDto to Vendor model
    private Vendor ConvertToModel(UpdateVendorRequestDto dto) => new Vendor
    {
        Id = Guid.Parse(dto.Id),
        VendorRating = dto.VendorRating,
        VendorRatingCount = dto.VendorRatingCount,
        Reviews = dto.Reviews.Select(review => new Review
        {
            Id = review.Id,
            ProductId = review.ProductId,
            ReviewerId = review.ReviewerId,
            ReviewerName = review.ReviewerName,
            ReviewRating = review.ReviewRating,
            ReviewText = review.ReviewText
        }).ToList()

    };

    // Convert CreateReviewRequestDto to Review model
    private Review ConvertToReviewModel(CreateReviewRequestDto dto) => new Review
    {
        Id = ObjectId.GenerateNewId().ToString(),
        ProductId = dto.ProductId,
        ReviewerId = string.Empty,
        ReviewerName = string.Empty,
        ReviewRating = dto.ReviewRating,
        ReviewText = dto.ReviewText
    };

    [HttpGet(Name = "GetVendors")]
    public async Task<IActionResult> GetVendors()
    {
        var vendors = await _vendors.Find(v => true).ToListAsync();
        return Ok(vendors.Select(vendor => ConvertToDto(vendor)));
    }

    [HttpGet("{id}", Name = "GetVendorDetails")]
    public async Task<IActionResult> GetVendor(string id)
    {
        var vendor = await _vendors.Find(v => v.Id == Guid.Parse(id)).FirstOrDefaultAsync();

        if (vendor == null)
        {
            return NotFound("Vendor not found");
        }
        else if (vendor.Reviews == null)
        {
            return Ok(new VendorDto
            {
                Id = vendor.Id.ToString(),
                VendorName = vendor.VendorName,
                VendorEmail = vendor.VendorEmail,
                VendorPhone = vendor.VendorPhone,
                VendorAddress = vendor.VendorAddress,
                VendorCity = vendor.VendorCity,
                VendorRating = vendor.VendorRating,
                VendorRatingCount = vendor.VendorRatingCount,
                Reviews = new List<ReviewDto>()
            });
        }
        else
        {
            return Ok(ConvertToDto(vendor));
        }
    }

    // POST: api/v1/Vendor/rating
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

        if (vendor.Reviews == null)
        {
            vendor.Reviews = new List<Review>();
        }

        // add review to vendor
        vendor.Reviews.Add(review);

        // update vendor rating
        if (vendor.VendorRatingCount == 0 || vendor.VendorRating == 0)
        {
            vendor.VendorRating = review.ReviewRating;
        }
        else
        {
            vendor.VendorRating = (vendor.VendorRating * vendor.VendorRatingCount + review.ReviewRating) / (vendor.VendorRatingCount + 1);
        }
        vendor.VendorRatingCount++;

        await _vendors.UpdateOneAsync(v => v.Id == Guid.Parse(dto.VendorId),
            Builders<Vendor>.Update
            .Set("Reviews", vendor.Reviews)
            .Set("VendorRating", vendor.VendorRating)
            .Set("VendorRatingCount", vendor.VendorRatingCount));
        return Ok(ConvertToDto(vendor));
    }

    // PUT: api/v1/Vendor/rating
    [HttpPut("rating", Name = "UpdateVendorRating")]
    [Authorize(Roles = "customer")]
    public async Task<IActionResult> UpdateRating([FromBody] UpdateReviewRequestDto dto)
    {
        var vendor = await _vendors.Find(v => v.Id == Guid.Parse(dto.VendorId)).FirstOrDefaultAsync();

        if (vendor == null) return NotFound("Vendor not found");
        if (vendor.Reviews == null) return NotFound("Vendor has no reviews");

        var review = vendor.Reviews.Find(r => r.Id == dto.Id);

        if (review == null) return NotFound("Review not found");

        // update vendor rating
        if (vendor.VendorRatingCount == 0 || vendor.VendorRating == 0)
        {
            vendor.VendorRating = dto.ReviewRating;
        }
        else
        {
            vendor.VendorRating = ((vendor.VendorRating * vendor.VendorRatingCount) - review.ReviewRating + dto.ReviewRating) / vendor.VendorRatingCount;
        }

        // update review
        review.ReviewRating = dto.ReviewRating;
        review.ReviewText = dto.ReviewText;

        // update vendor reviews
        int updateIndex = vendor.Reviews.FindIndex(r => r.Id == dto.Id);
        vendor.Reviews[updateIndex] = review;

        await _vendors.ReplaceOneAsync(v => v.Id == Guid.Parse(dto.VendorId), vendor);
        return Ok(ConvertToDto(vendor));
    }

    // DELETE: api/v1/Vendor/rating/{vendorId}/{reviewId}
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
        vendor.VendorRatingCount--;
        if (vendor.VendorRatingCount == 0)
        {
            vendor.VendorRating = 0;
        }
        else
        {
            vendor.VendorRating = (vendor.VendorRating * (vendor.VendorRatingCount + 1) - review.ReviewRating) / vendor.VendorRatingCount;
        }

        await _vendors.ReplaceOneAsync(v => v.Id == Guid.Parse(vendorId), vendor);
        return Ok(ConvertToDto(vendor));
    }

    // PUT: api/v1/Vendor/{id}
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

    // GET: api/v1/Vendor/search/{name}
    [HttpGet("search/{name}", Name = "SearchVendor")]
    public async Task<IActionResult> SearchVendor(string name)
    {
        var vendors = await _vendors.Find(v => v.VendorName.ToLower().Contains(name.ToLower())).ToListAsync();
        return Ok(vendors.Select(vendor => ConvertToDto(vendor)));
    }


}

