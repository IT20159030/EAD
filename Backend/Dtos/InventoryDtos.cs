using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos
{
    public class InventoryDto
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required]
        public string ProductId { get; set; } = string.Empty;

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a positive number.")]
        public int Quantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Alert Threshold must be a positive number.")]
        public int AlertThreshold { get; set; } = 10;

        public bool LowStockAlert { get; set; } = false;

        public string VendorId { get; set; } = string.Empty;
    }

    public class AddInventoryByProductIdDto
    {
        [Required]
        public string ProductId { get; set; } = string.Empty;

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a positive number.")]
        public int Quantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Alert Threshold must be a positive number.")]
        public int AlertThreshold { get; set; } = 10;

        public string VendorId { get; set; } = string.Empty;
    }

    public class UpdateInventoryByProductIdDto
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a positive number.")]
        public int Quantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Alert Threshold must be a positive number.")]
        public int AlertThreshold { get; set; } = 10;
    }

    public class SetLowStockAlertDto
    {
        [Required]
        public bool LowStockAlert { get; set; }
    }
}
