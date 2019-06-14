namespace CameraBazaar.Services.Models.Cameras
{
    using System.ComponentModel.DataAnnotations;
    using CameraBazaar.Data.Models;

    public class CameraDetailsModel : CameraListingModel
    {
        [Display(Name = "Full frame")]
        public bool IsFullFrame { get; set; }

        [Display(Name = "Min shutter speed")]
        public int MinShutterSpeed { get; set; }

        [Display(Name = "Max shutter speed")]
        public int MaxShutterSpeed { get; set; }

        [Display(Name = "Min ISO")]
        public MinIso MinISO { get; set; }

        [Display(Name = "Max ISO")]
        public int MaxISO { get; set; }

        [Display(Name = "Video resolution")]
        public string VideoResolution { get; set; }

        [Display(Name = "Light metering")]
        public LightMetering LightMetering { get; set; }

        public string Description { get; set; }

        public string UserId { get; set; }

        public string SellerUsername { get; set; }
    }
}
