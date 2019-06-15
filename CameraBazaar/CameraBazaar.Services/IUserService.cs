namespace CameraBazaar.Services
{
    using CameraBazaar.Services.Models.Users;

    public interface IUserService
    {
        UserDetailsWithCamerasModel GetUserDetailsWithCameras(string username);

        void UpdateLastLoginTime(string userName);
    }
}
