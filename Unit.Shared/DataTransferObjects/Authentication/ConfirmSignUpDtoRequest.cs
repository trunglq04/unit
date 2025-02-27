﻿using System.ComponentModel.DataAnnotations;

namespace Unit.Shared.DataTransferObjects.Authentication
{
    public record ConfirmSignUpDtoRequest
    {
        [Required]
        public required string Email { get; init; }

        [Required]
        public required string ConfirmCode { get; init; }
    }

}
