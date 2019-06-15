namespace CameraBazaar.Services
{
    using System.Collections.Generic;
    using CameraBazaar.Data.Models;
    using CameraBazaar.Services.Models.Cameras;

    public interface ICameraService
    {
        IEnumerable<CameraListingModel> All();

        void Create(
            CameraMake make,
            string model,
            decimal price,
            int quantity,
            int minShutterSpeed,
            int maxShutterSpeed,
            MinIso minIso,
            int maxIso,
            bool isFullFrame,
            string videoResolution,
            IEnumerable<LightMetering> lightMeterings,
            string description,
            string imageUrl,
            string userId);

        bool Exists(int id);

        bool Exists(int id, string userId);

        CameraDetailsModel GetByIdDetails(int id);

        CameraEditDeleteModel GetById(int id);

        CameraWithSellerModel GetByIdWithSeller(int id);

        void Remove(int id, string userId);

        void Update(
            int id,
            CameraMake make,
            string model,
            decimal price,
            int quantity,
            int minShutterSpeed,
            int maxShutterSpeed,
            MinIso minIso,
            int maxIso,
            bool isFullFrame,
            string videoResolution,
            IEnumerable<LightMetering> lightMeterings,
            string description,
            string imageUrl,
            string userId);
    }
}
