using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos
{
    /*
    * This file contains Data Transfer Objects (DTOs) for Product.
    */
    
    public class ProductDto
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Image { get; set; } = "default.jpg";

        [Required]
        public string Category { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        public bool IsActive { get; set; }

        public int Stock { get; set; } = 0;

        [Required]
        public string VendorId { get; set; } = string.Empty;

        public string VendorName { get; set; } = string.Empty;
    }

    public class CreateProductRequestDto
    {
        [Required]
        [StringLength(250, ErrorMessage = "Name length can't be more than 250.")]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Image { get; set; } = "default.jpg";

        [Required]
        [StringLength(24, ErrorMessage = "Category Id length can't be more than 24.")]
        public string Category { get; set; } = string.Empty;

        [Required]
        [StringLength(500, ErrorMessage = "Description length can't be more than 500.")]
        public string Description { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; } = 0;

        public bool IsActive { get; set; } = false;

        public int Stock { get; set; } = 0;

        [Required]
        [StringLength(36, ErrorMessage = "Vendor Id length can't be more than 36.")]
        public string VendorId { get; set; } = string.Empty;
    }

    public class UpdateProductRequestDto
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required]
        [StringLength(250, ErrorMessage = "Name length can't be more than 250.")]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Image { get; set; } = "default.jpg";
        
        [Required]
        [StringLength(24, ErrorMessage = "Category Id length can't be more than 24.")]
        public string Category { get; set; } = string.Empty;

        [Required]
        [StringLength(500, ErrorMessage = "Description length can't be more than 500.")]
        public string Description { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        public bool IsActive { get; set; }

        public int Stock { get; set; }

        [Required]
        [StringLength(36, ErrorMessage = "Vendor Id length can't be more than 36.")]
        public string VendorId { get; set; } = string.Empty;
    }


}
