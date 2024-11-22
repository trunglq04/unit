
namespace Unit.Shared.DataTransferObjects
{
    public class UserDto
    {
        public string? UserName { get; set; }

        public string? UserId { get; set; }

        public string? PhoneNumber { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? ProfilePicture { get; set; }

        public string? Bio { get; set; }

        public List<string>? Followers { get; set; }

        public int? NumberOfFollwers { get; set; }

        public List<string>? Following { get; set; }

        public int? NumberOfFollowing { get; set; }

        public List<string>? BlockedUsers { get; set; }

        public List<string>? ConversationId { get; set; }

        public bool? Active { get; set; } // e.g., "active", "inactive"

        public bool? Private { get; set; } // "public" or "private"

        public DateTime? CreatedAt { get; set; }

        public DateTime? LastModified { get; set; }

        public List<FollowRequestDto>? FollowRequests { get; set; }
    }
}
