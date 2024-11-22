namespace Unit.Shared.DataTransferObjects.User
{
    public record UserInfoDtoForUpdate
    {
        public string? UserName { get; init; }

        public string? PhoneNumber { get; init; }

        public DateTime? DateOfBirth { get; init; }

        public string? Bio { get; init; }

        public bool? Private { get; init; } // "public" or "private"

        public bool? IsAcceptFollower { get; init; }

        public string? Follower { get; init; } //remove followers

        public string? Follow { get; init; } // follow or  remove following 

    }
}
