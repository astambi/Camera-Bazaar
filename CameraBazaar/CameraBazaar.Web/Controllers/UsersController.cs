namespace CameraBazaar.Web.Controllers
{
    using System;
    using System.Threading.Tasks;
    using AutoMapper;
    using CameraBazaar.Data.Models;
    using CameraBazaar.Services;
    using CameraBazaar.Web.Models.Users;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class UsersController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IUserService userService;
        private readonly IMapper mapper;

        public UsersController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IUserService userService,
            IMapper mapper)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.userService = userService;
            this.mapper = mapper;
        }

        public async Task<IActionResult> Profile(string username)
        {
            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.RedirectToAction(nameof(Profile));
            }

            username = username ?? user.UserName; // current user profile

            var profileData = this.userService.GetUserDetailsWithCameras(username);
            if (profileData == null)
            {
                return this.RedirectToAction(nameof(Profile));
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
            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");
            }

            var model = this.mapper.Map<UserEditViewModel>(user);
            model.HasPassword = await this.userManager.HasPasswordAsync(user);

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.RedirectToAction(nameof(Profile));
            }

            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            // Update Email
            if (model.Email != user.Email)
            {
                var setEmailResult = await this.userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                {
                    return this.View(model);
                }
            }

            // Update phonenumber
            if (model.PhoneNumber != user.PhoneNumber)
            {
                var setPhoneResult = await this.userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    return this.View(model);
                }
            }

            // Set / Update password
            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                var hasPassword = await this.userManager.HasPasswordAsync(user);

                if (!hasPassword)
                {
                    // Set Password
                    var addPasswordResult = await this.userManager.AddPasswordAsync(user, model.NewPassword);
                    if (!addPasswordResult.Succeeded)
                    {
                        this.AddResultErrors(addPasswordResult);
                        return this.View(model);
                    }
                }
                else
                {
                    // Update Password
                    var changePasswordResult = await this.userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (!changePasswordResult.Succeeded)
                    {
                        this.AddResultErrors(changePasswordResult);
                        return this.View(model);
                    }
                }
            }

            await this.signInManager.RefreshSignInAsync(user);

            return this.RedirectToAction(nameof(Profile));
        }

        private void AddResultErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}