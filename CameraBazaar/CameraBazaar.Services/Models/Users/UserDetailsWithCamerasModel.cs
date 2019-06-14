namespace CameraBazaar.Services.Models.Users
{
    using System.Collections.Generic;
    using System.Linq;
    using CameraBazaar.Services.Models.Cameras;

    public class UserDetailsWithCamerasModel
    {
        public UserDetailsModel UserDetails { get; set; }

        public ICollection<CameraListingModel> Cameras { get; set; }

        public bool IsOwner { get; set; }

        public int CamerasInStockCount
            => this.Cameras.Where(c => c.InStock).Count();

        public int CamerasOutOfStockCount
            => this.Cameras.Count - this.CamerasInStockCount;
    }
}
