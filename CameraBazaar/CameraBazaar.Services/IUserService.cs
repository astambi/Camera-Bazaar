namespace CameraBazaar.Services
{
    using CameraBazaar.Services.Models.Users;

    public interface IUserService
    {
        UserDetailsWithCamerasModel GetUserDetailsWithCameras(string username);

        string GetUsernameByEmail(string email);

        void UpdateLastLoginTime(string userName);
    }
}
