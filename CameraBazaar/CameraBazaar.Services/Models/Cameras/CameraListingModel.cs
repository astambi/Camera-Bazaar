namespace CameraBazaar.Services.Models.Cameras
{
    using CameraBazaar.Common.Mapping;
    using CameraBazaar.Data.Models;

    public class CameraListingModel : IMapFrom<Camera>
    {
        public int Id { get; set; }

        public CameraMake Make { get; set; }

        public string Model { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public bool InStock => this.Quantity > 0;

        public string ImageUrl { get; set; }

        public bool HasCrud { get; set; } // false
    }
}
