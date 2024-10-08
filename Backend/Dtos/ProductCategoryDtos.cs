using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos
{
    /*
    * This file contains Data Transfer Objects (DTOs) for ProductCategory.
    */
    
    public class ProductCategoryDto
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;
    }

    public class CreateProductCategoryRequestDto
    {
        [Required]
        [StringLength(100, ErrorMessage = "Name length can't be more than 100.")]
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateProductCategoryRequestDto
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "Name length can't be more than 100.")]
        public string Name { get; set; } = string.Empty;
    }
}
