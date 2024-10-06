/**
* This is a Data Transfer Object for the Review model. It defines how data will be sent over the network.
*/

using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos;

public class ReviewDto
{
    public string Id { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ReviewerId { get; set; } = string.Empty;
    public string ReviewerName { get; set; } = string.Empty;
    public int ReviewRating { get; set; }
    public string? ReviewText { get; set; }
}

public class CreateReviewRequestDto
{
    [Required]
    public string ProductId { get; set; } = string.Empty;

    [Required]
    public string VendorId { get; set; } = string.Empty;

    [Required]
    public int ReviewRating { get; set; }

    public string? ReviewText { get; set; }
}

public class UpdateReviewRequestDto
{
    [Required]
    public string Id { get; set; } = string.Empty;

    [Required]
    public string ProductId { get; set; } = string.Empty;

    [Required]
    public string VendorId { get; set; } = string.Empty;

    [Required]
    public string ReviewerId { get; set; } = string.Empty;

    [Required]
    public string ReviewerName { get; set; } = string.Empty;

    [Required]
    public int ReviewRating { get; set; }

    public string? ReviewText { get; set; }
}