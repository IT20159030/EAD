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

    public string VendorName { get; set; } = string.Empty;

    public string VendorEmail { get; set; } = string.Empty;

    public string VendorPhone { get; set; } = string.Empty;

    public string VendorAddress { get; set; } = string.Empty;

    public string VendorCity { get; set; } = string.Empty;

    public double VendorRating { get; set; }
    public int VendorRatingCount { get; set; }
    public List<ReviewDto> Reviews { get; set; } = new List<ReviewDto>();
}

public class CreateVendorRequestDto
{

    [Required]
    public double VendorRating { get; set; }

    [Required]
    public int VendorRatingCount { get; set; }

    public List<CreateReviewRequestDto> Reviews { get; set; } = new List<CreateReviewRequestDto>();
}

public class UpdateVendorRequestDto
{
    [Required]
    public string Id { get; set; } = string.Empty;

    [Required]
    public double VendorRating { get; set; }

    [Required]
    public int VendorRatingCount { get; set; }

    public List<UpdateReviewRequestDto> Reviews { get; set; } = new List<UpdateReviewRequestDto>();
}
