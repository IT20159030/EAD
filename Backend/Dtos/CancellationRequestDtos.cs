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
        public required string ProcessedBy { get; set; } // CSR or Admin ID

        [Required]
        public required string Status { get; set; } // Approved or Denied

        [StringLength(500)]
        public string? DecisionNote { get; set; } // Optional note for why the decision was made
    }

    // DTO to return a simplified view of a cancellation request
    public class CancellationRequestDto
    {
        public required string Id { get; set; }

        public required string OrderId { get; set; }

        public required string CustomerId { get; set; }

        public DateTime RequestDate { get; set; }

        public required string Status { get; set; }

        public required string Reason { get; set; }

        public string? ProcessedBy { get; set; }

        public DateTime? ProcessedDate { get; set; }

        public string? DecisionNote { get; set; }
    }
}
