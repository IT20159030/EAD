/**
Description: Dtos for Vendor.
The VendorDto class is used to represent the vendor object in the application. It contains the following properties:
    vendorId: the unique identifier of the vendor.
    vendorName: the name of the vendor.
    vendorEmail: the email address of the vendor.
    vendorPhone: the phone number of the vendor.
    vendorAddress: the address of the vendor.
    vendorCity: the city of the vendor.
    vendorRating: the rating of the vendor.
    vendorRatingCount: the number of ratings the vendor has received.
*/

using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos;

public class VendorDto
{
    public string Id { get; set; } = string.Empty;
    public string VendorId { get; set; } = string.Empty;
    public string VendorName { get; set; } = string.Empty;
    public string VendorEmail { get; set; } = string.Empty;
    public string VendorPhone { get; set; } = string.Empty;
    public string VendorAddress { get; set; } = string.Empty;
    public string VendorCity { get; set; } = string.Empty;
    public float VendorRating { get; set; }
    public int VendorRatingCount { get; set; }
}

// public class CreateVendorRequestDto
// {
//     [Required]
//     public string VendorId { get; set; } = string.Empty;

//     [Required]
//     public string VendorName { get; set; } = string.Empty;

//     [Required, EmailAddress]
//     public string VendorEmail { get; set; } = string.Empty;

//     [Required]
//     public string VendorPhone { get; set; } = string.Empty;

//     [Required]
//     public string VendorAddress { get; set; } = string.Empty;

//     [Required]
//     public string VendorCity { get; set; } = string.Empty;

//     [Required]
//     public float VendorRating { get; set; }

//     [Required]
//     public int VendorRatingCount { get; set; }
// }

// public class UpdateVendorRequestDto
// {
//     [Required]
//     public string Id { get; set; } = string.Empty;

//     [Required]
//     public string VendorId { get; set; } = string.Empty;

//     [Required]
//     public string VendorName { get; set; } = string.Empty;

//     [Required, EmailAddress]
//     public string VendorEmail { get; set; } = string.Empty;

//     [Required]
//     public string VendorPhone { get; set; } = string.Empty;

//     [Required]
//     public string VendorAddress { get; set; } = string.Empty;

//     [Required]
//     public string VendorCity { get; set; } = string.Empty;

//     [Required]
//     public float VendorRating { get; set; }

//     [Required]
//     public int VendorRatingCount { get; set; }
// }