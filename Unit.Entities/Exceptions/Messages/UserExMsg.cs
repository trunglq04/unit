namespace Unit.Entities.Exceptions.Messages
{
    public static class UserExMsg
    {
        public const string UserIsNotExist = "User is not exist.";
        public const string DoNotHavePermissionToView = "Seem like you dont have permission to do this.";
        public const string DoNotHave = "Seem like you dont have any";
        public const string WrongFormatImage = "The file must be a JPEG, PNG or GIF image format.";
        public const string ProfileImageOverSize = "File size must not exceed 10MB.";
    }
}
