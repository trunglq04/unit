

namespace Unit.Shared.DataTransferObjects
{
    public record UserInfoDtoForUpdate
    {
        public string? UserName { get; init; }

        public string? PhoneNumber { get; init; }

        public DateTime? DateOfBirth { get; init; }

        public string? Bio { get; init; }

        public bool? Private { get; init; } // "public" or "private"

    }
}
