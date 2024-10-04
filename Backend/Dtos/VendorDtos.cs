/**
Description: Dtos for Vendor.
The VendorDto class is used to represent the vendor object in the application. It contains the following properties:
    vendorRating: the rating of the vendor.
    vendorRatingCount: the number of ratings the vendor has received.
    reviews: a list of reviews for the vendor.
*/

using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos;

public class VendorDto
{
    public string Id { get; set; } = string.Empty;
    public int VendorRating { get; set; }
    public int VendorRatingCount { get; set; }
    public List<ReviewDto> Reviews { get; set; } = new List<ReviewDto>();
}

public class CreateVendorRequestDto
{

    [Required]
    public int VendorRating { get; set; }

    [Required]
    public int VendorRatingCount { get; set; }

    public List<CreateReviewRequestDto> Reviews { get; set; } = new List<CreateReviewRequestDto>();
}

public class UpdateVendorRequestDto
{
    [Required]
    public string Id { get; set; } = string.Empty;

    [Required]
    public int VendorRating { get; set; }

    [Required]
    public int VendorRatingCount { get; set; }

    public List<UpdateReviewRequestDto> Reviews { get; set; } = new List<UpdateReviewRequestDto>();
}
