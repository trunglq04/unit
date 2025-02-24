namespace Unit.Entities.Exceptions.Messages
{
    public static class UserExMsg
    {
        public const string UserIsNotExist = "User is not exist.";
        public const string UserIsUnActive = "User is not active.";
        public const string DoNotHavePermissionToView = "Seem like you dont have permission to do this.";
        public const string DoNotHave = "Seem like you dont have any";
        public const string WrongFormatImage = "The file must be a JPEG, PNG or GIF image format.";
        public const string ProfileImageOverSize10Mb = "File size must not exceed 10MB.";
        public const string ImageOverSize30Mb = "Photo size must not exceed 30MB.";
        public const string MaximumFileUpload = "Users can only upload a maximum number of files is";
        public const string MaximumVideoDuration = "Video length must not exceed 5 minutes.";
        public const string WrongPostFormatImage = "Only photo (JPEG, PNG, GIF) and video (MP4, MPEG) uploads are allowed.";
        public const string UserHasBeenDisable = "User have been deactivated.";
        public const string AlreadyLikedPost = "You had liked this post already";
        public const string AlreadyUnLikedPost = "You had unliked this post already";
    }
}
