namespace CameraBazaar.Web.Models.Cameras
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using AutoMapper;
    using CameraBazaar.Common.Mapping;
    using CameraBazaar.Data.Models;
    using CameraBazaar.Services.Models.Cameras;
    using CameraBazaar.Web.Controllers;

    public class CameraFormViewModel : IMapFrom<CameraEditDeleteModel>
    {
        public CameraMake Make { get; set; } // enum

        [Required]
        [StringLength(100)]
        [RegularExpression("^[A-Z0-9-]+$",
            ErrorMessage = "The {0} can contain only uppercase letters, digits and a dash ('-').")]
        public string Model { get; set; }

        [Range(0, double.MaxValue,
            ErrorMessage = "The {0} cannot be negative.")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Range(0, 100,
            ErrorMessage = "The {0} must be in the range {1} – {2}.")]
        public int Quantity { get; set; }

        [Range(1, 30,
            ErrorMessage = "The {0} must be in the range {1} – {2}.")]
        [Display(Name = "Min shutter speed")]
        public int MinShutterSpeed { get; set; }

        [Range(2000, 8000,
            ErrorMessage = "The {0} must be in the range {1} – {2}.")]
        [Display(Name = "Max shutter speed")]
        public int MaxShutterSpeed { get; set; }

        [Display(Name = "Min ISO")]
        public MinIso MinIso { get; set; } // enum

        [Range(200, 409600,
            ErrorMessage = "The {0} must be in the range {1} – {2}.")]
        [RegularExpression("^[1-9][0-9]*00$",
            ErrorMessage = "The {0} must be divisible by 100.")]
        [Display(Name = "Max ISO")]
        public int MaxIso { get; set; }

        [Display(Name = "Full frame")]
        public bool IsFullFrame { get; set; }

        [Required]
        [StringLength(15,
            ErrorMessage = "The {0} must be no longer than {1} symbols.")]
        [Display(Name = "Video resolution")]
        public string VideoResolution { get; set; }

        [Required]
        [Display(Name = "Light metering")]
        [IgnoreMap]
        public IEnumerable<LightMetering> LightMeteringSelectList { get; set; } = new List<LightMetering>(); // multiple enums

        [Display(Name = "Light metering")]
        public LightMetering LightMetering { get; set; } // current values

        [Required]
        [StringLength(6000,
            ErrorMessage = "The {0} must be no longer than {1} symbols.")]
        public string Description { get; set; }

        [Required]
        [Url]
        [StringLength(2000,
            ErrorMessage = "The {0} must contain between {2} and {1} symbols.",
            MinimumLength = 10)]
        [RegularExpression(@"^(https?:\/\/).+$",
            ErrorMessage = "The {0} must start with http:// or https://")]
        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; }

        [IgnoreMap]
        public string Action { get; set; } = nameof(CamerasController.Create); // default create
    }
}