using System;
using Unit.Shared.DataTransferObjects;

namespace Unit.Shared.RequestFeatures
{
    public class UserParameters : RequestParameters
    {
        public string? UserName { get; set; }

        public string? Include { get; set; }

        public bool? Followers { get; set; }

        public bool? Following { get; set; }

        public bool? Post { get; set; }

        public bool GetFollowers()
        {
            return Followers is not null &&
                (bool)Followers &&
                !string.IsNullOrWhiteSpace(Include) &&
                string.Equals(Include, nameof(UserDto.Followers), StringComparison.OrdinalIgnoreCase);
        }
        public bool GetFollowing()
        {
            return Following is not null &&
                (bool)Following &&
                !string.IsNullOrWhiteSpace(Include) &&
                string.Equals(Include, nameof(UserDto.Following), StringComparison.OrdinalIgnoreCase);
        }

        public bool GetFollowersOrFollowing()
            => GetFollowers() || GetFollowing();
    }
}
