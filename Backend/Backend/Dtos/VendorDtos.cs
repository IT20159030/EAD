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

public class CreateVendorRequestDto
{
    [Required]
    public string VendorId { get; set; } = string.Empty;

    [Required]
    public string VendorName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string VendorEmail { get; set; } = string.Empty;

    [Required]
    public string VendorPhone { get; set; } = string.Empty;

    [Required]
    public string VendorAddress { get; set; } = string.Empty;

    [Required]
    public string VendorCity { get; set; } = string.Empty;

    [Required]
    public float VendorRating { get; set; }

    [Required]
    public int VendorRatingCount { get; set; }
}

public class UpdateVendorRequestDto
{
    [Required]
    public string Id { get; set; } = string.Empty;

    [Required]
    public string VendorId { get; set; } = string.Empty;

    [Required]
    public string VendorName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string VendorEmail { get; set; } = string.Empty;

    [Required]
    public string VendorPhone { get; set; } = string.Empty;

    [Required]
    public string VendorAddress { get; set; } = string.Empty;

    [Required]
    public string VendorCity { get; set; } = string.Empty;

    [Required]
    public float VendorRating { get; set; }

    [Required]
    public int VendorRatingCount { get; set; }
}