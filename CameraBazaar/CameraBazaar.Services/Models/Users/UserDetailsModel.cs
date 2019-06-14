namespace CameraBazaar.Services.Models.Users
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using CameraBazaar.Common.Mapping;
    using CameraBazaar.Data.Models;

    public class UserDetailsModel : IMapFrom<User>
    {
        public string Id { get; set; }

        [Display(Name = "Username")]
        public string UserName { get; set; }

        public string Email { get; set; }

        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Last login time")]
        public DateTime? LastLoginTime { get; set; }
    }
}
