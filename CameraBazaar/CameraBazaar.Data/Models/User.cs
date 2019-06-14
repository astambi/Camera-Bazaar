namespace CameraBazaar.Data.Models
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Identity;

    public class User : IdentityUser // Custom App User
    {
        public DateTime? LastLoginTime { get; set; }

        public ICollection<Camera> Cameras => new List<Camera>();
    }
}
