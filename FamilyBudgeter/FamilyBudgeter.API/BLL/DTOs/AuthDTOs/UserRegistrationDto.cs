﻿using System.ComponentModel.DataAnnotations;

namespace FamilyBudgeter.API.BLL.DTOs.AuthDTOs
{
    public class UserRegistrationDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [MinLength(6)]
        public required string Password { get; set; }

        [Required]
        public required string FirstName { get; set; }

        [Required]
        public required string LastName { get; set; }
    }
}
