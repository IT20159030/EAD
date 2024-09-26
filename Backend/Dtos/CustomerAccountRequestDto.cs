using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos
{
    // DTO for creating a new customer account request
    public class CreateCustomerAccountRequestDto
    {
        [Required]
        public string CustomerId { get; set; }

        [Required]
        public DateTime RequestDate { get; set; }

        public string? Status { get; set; } = "Pending";
    }

    // DTO for processing a customer account request
    public class ProcessCustomerAccountRequestDto
    {
        [Required]
        public string Status { get; set; } // Approved or Denied

        [Required]
        public string ProcessedBy { get; set; }

        public DateTime? ProcessedDate { get; set; } = DateTime.UtcNow;
    }

    // DTO to return a simplified view of the customer account request
    public class CustomerAccountRequestDto
    {
        public string Id { get; set; }

        public string CustomerId { get; set; }

        public DateTime RequestDate { get; set; }

        public string? Status { get; set; }

        public string? ProcessedBy { get; set; }

        public DateTime? ProcessedDate { get; set; }
    }
}
