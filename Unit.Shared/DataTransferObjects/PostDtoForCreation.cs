using System.ComponentModel.DataAnnotations;

namespace Unit.Shared.DataTransferObjects
{
    public record PostDtoForCreation
    {
        public string PostId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(500)]
        public string Content { get; set; } = string.Empty;

        public bool IsHidden { get; set; } = false;
    }
}
