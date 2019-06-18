namespace CameraBazaar.Web.Models.Users
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using AutoMapper;
    using CameraBazaar.Common.Mapping;
    using CameraBazaar.Data.Models;

    public class UserEditViewModel : IMapFrom<User>
    {
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        [RegularExpression(@"^\+\d{10,12}$",
            ErrorMessage = "The {0} must start with '+' and contain between 10 and 12 digits.")]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        [IgnoreMap]
        public string OldPassword { get; set; }

        [StringLength(100,
            ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 3)]
        [RegularExpression(@"^[a-z0-9]{3,100}$",
            ErrorMessage = "The {0} must be at least 3 symbols long and contain only lowercase letters and digits.")]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        [IgnoreMap]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare(nameof(NewPassword),
            ErrorMessage = "The new password and confirmation password do not match.")]
        [IgnoreMap]
        public string ConfirmPassword { get; set; }

        public DateTime? LastLoginTime { get; set; }

        [IgnoreMap]
        public bool HasPassword { get; set; }
    }
}
