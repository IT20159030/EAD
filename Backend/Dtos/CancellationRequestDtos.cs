using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos

{
    // DTO for creating a cancellation request
    public class CreateCancellationRequestDto
    {
        [Required]
        public string OrderId { get; set; } = string.Empty;

        [Required]
        public string CustomerId { get; set; } = string.Empty;

        [Required]
        [StringLength(500, MinimumLength = 2)]
        public string Reason { get; set; } = string.Empty;
    }

    // DTO for processing a cancellation request
    public class ProcessCancellationRequestDto
    {
        [Required]
        public string ProcessedBy { get; set; } // CSR or Admin ID

        [Required]
        public string Status { get; set; } // Approved or Denied

        [StringLength(500)]
        public string? DecisionNote { get; set; } // Optional note for why the decision was made
    }

    // DTO to return a simplified view of a cancellation request
    public class CancellationRequestDto
    {
        public string Id { get; set; }

        public string OrderId { get; set; }

        public string CustomerId { get; set; }

        public DateTime RequestDate { get; set; }

        public string Status { get; set; }

        public string Reason { get; set; }

        public string? ProcessedBy { get; set; }

        public DateTime? ProcessedDate { get; set; }

        public string? DecisionNote { get; set; }
    }
}
