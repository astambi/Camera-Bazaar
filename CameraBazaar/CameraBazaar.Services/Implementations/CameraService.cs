namespace CameraBazaar.Services.Implementations
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using CameraBazaar.Data;
    using CameraBazaar.Data.Models;
    using CameraBazaar.Services.Models.Cameras;

    public class CameraService : ICameraService
    {
        private readonly CameraBazaarDbContext db;
        private readonly IMapper mapper;

        public CameraService(CameraBazaarDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public IEnumerable<CameraListingModel> All()
            => this.db
            .Cameras
            .OrderBy(c => c.Make.ToString())
            .ThenBy(c => c.Model)
            .ThenByDescending(c => c.Price)
            .Select(c => this.mapper.Map<CameraListingModel>(c))
            .ToList();

        public void Create(
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
            string userId)
        {
            var camera = new Camera
            {
                Make = make,
                Model = model,
                Price = price,
                Quantity = quantity,
                MinShutterSpeed = minShutterSpeed,
                MaxShutterSpeed = maxShutterSpeed,
                MinIso = minIso,
                MaxIso = maxIso,
                IsFullFrame = isFullFrame,
                VideoResolution = videoResolution,
                LightMetering = (LightMetering)lightMeterings.Cast<int>().Sum(), // NB! multiple enum selection
                Description = description,
                ImageUrl = imageUrl,
                UserId = userId
            };

            this.db.Cameras.Add(camera);
            this.db.SaveChanges();
        }

        public void Remove(int id, string userId)
        {
            var camera = this.db.Cameras.FirstOrDefault(c => c.Id == id && c.UserId == userId);

            if (camera == null)
            {
                return;
            }

            this.db.Cameras.Remove(camera);
            this.db.SaveChanges();
        }

        public bool Exists(int id)
            => this.db.Cameras.Any(c => c.Id == id);

        public bool ExistsWithOwner(int id, string userId)
            => this.db.Cameras.Any(c => c.Id == id && c.UserId == userId);

        public CameraDetailsModel GetByIdDetails(int id)
        {
            if (!this.Exists(id))
            {
                return null;
            }

            var camera = this.db
                .Cameras
                .Where(c => c.Id == id)
                .Select(c => this.mapper.Map<CameraDetailsModel>(c))
                .FirstOrDefault();

            camera.SellerUsername = this.db
                .Users
                .Where(u => u.Id == camera.UserId)
                .Select(u => u.UserName)
                .FirstOrDefault();

            return camera;
        }

        public CameraEditDeleteModel GetById(int id)
        {
            if (!this.Exists(id))
            {
                return null;
            }

            return this.db
                .Cameras
                .Where(c => c.Id == id)
                .Select(c => this.mapper.Map<CameraEditDeleteModel>(c))
                .FirstOrDefault();
        }

        public CameraWithSellerModel GetByIdWithSeller(int id)
            => this.db
            .Cameras
            .Where(c => c.Id == id)
            .Select(c => new CameraWithSellerModel
            {
                Id = id,
                Make = c.Make.ToString(),
                Model = c.Model,
                SellerUsername = c.User.UserName
            })
            .FirstOrDefault();

        public void Update(
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
            string userId)
        {
            var camera = this.db.Cameras.FirstOrDefault(c => c.Id == id && c.UserId == userId);

            if (camera == null)
            {
                return;
            }

            camera.Make = make;
            camera.Model = model;
            camera.Price = price;
            camera.Quantity = quantity;
            camera.MinShutterSpeed = minShutterSpeed;
            camera.MaxShutterSpeed = maxShutterSpeed;
            camera.MinIso = minIso;
            camera.MaxIso = maxIso;
            camera.IsFullFrame = isFullFrame;
            camera.VideoResolution = videoResolution;
            camera.LightMetering = (LightMetering)lightMeterings.Cast<int>().Sum(); // NB! multiple enum selection
            camera.Description = description;
            camera.ImageUrl = imageUrl;

            this.db.SaveChanges();
        }
    }
}
