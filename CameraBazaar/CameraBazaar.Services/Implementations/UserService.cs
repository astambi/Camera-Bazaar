namespace CameraBazaar.Services.Implementations
{
    using System;
    using System.Linq;
    using AutoMapper;
    using CameraBazaar.Data;
    using CameraBazaar.Services.Models.Cameras;
    using CameraBazaar.Services.Models.Users;

    public class UserService : IUserService
    {
        private readonly CameraBazaarDbContext db;
        private readonly IMapper mapper;

        public UserService(CameraBazaarDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public UserDetailsWithCamerasModel GetUserDetailsWithCameras(string username)
            => this.db
            .Users
            .Where(u => u.UserName == username)
            .Select(u => new UserDetailsWithCamerasModel
            {
                UserDetails = this.mapper.Map<UserDetailsModel>(u),
                Cameras = u.Cameras
                    .OrderBy(c => c.Make)
                    .ThenBy(c => c.Model)
                    .ThenByDescending(c => c.Price)
                    .Select(c => this.mapper.Map<CameraListingModel>(c))
                    .ToList()
            })
            .FirstOrDefault();

        public void UpdateLoginTime(string userName)
        {
            var user = this.db.Users.Where(u => u.UserName == userName).FirstOrDefault();

            if (user == null)
            {
                return;
            }

            user.LastLoginTime = DateTime.UtcNow;
            this.db.SaveChanges();
        }
    }
}
