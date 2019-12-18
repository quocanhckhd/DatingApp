using System;
using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Models.DTOs
{
    public class RegisterDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "You must specified password between 4 and 8 characters")]
        public string Password { get; set; }
    }
}