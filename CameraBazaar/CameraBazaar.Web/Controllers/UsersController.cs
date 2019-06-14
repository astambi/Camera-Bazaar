namespace CameraBazaar.Web.Controllers
{
    using System;
    using System.Threading.Tasks;
    using CameraBazaar.Data.Models;
    using CameraBazaar.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class UsersController : Controller
    {
        private readonly ICameraService cameraService;
        private readonly IUserService userService;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public UsersController(
            ICameraService cameraService,
            IUserService userService,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            this.cameraService = cameraService;
            this.userService = userService;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public IActionResult Index()
            => this.View();

        public async Task<IActionResult> Profile(string username)
        {
            var currentUser = await this.userManager.GetUserAsync(this.User);
            if (currentUser == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");
            }

            username = username ?? currentUser.UserName; // current user profile

            var profileData = this.userService.GetUserDetailsWithCameras(username);
            if (profileData == null)
            {
                return this.NotFound($"User with username {username} does not exist");
            }

            // Is owner
            if (this.User.Identity.Name == username)
            {
                profileData.IsOwner = true;
            }

            return this.View(profileData);
        }

        public async Task<IActionResult> Edit()
        {
            var currentUser = await this.userManager.GetUserAsync(this.User);
            if (currentUser == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");
            }

            return this.View();
        }
    }
}