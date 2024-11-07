using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Unit.Shared.CustomValidationAttribute
{
    public class PasswordValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var message = new StringBuilder();
            if (value is string password)
            {
                password = password.Trim();
                if (!password.Any(char.IsLower)) message.Append("Password must contain a lower case letter.\r\n");
                if (!password.Any(char.IsUpper)) message.Append("Password must contain an upper case letter.\r\n");
                if (!password.Any(char.IsNumber)) message.Append("Password must contain a number.\r\n");
                if (password.Length < 8) message.Append("Password must contain at least 8 characters.\r\n");
                if (!password.Any(char.IsPunctuation) && !password.Any(char.IsWhiteSpace)) message.Append("Password must contain a special character or a space.");
            }
            if (message.Length > 0)
                return new ValidationResult(message.ToString());

            return ValidationResult.Success;
        }
    }
}
