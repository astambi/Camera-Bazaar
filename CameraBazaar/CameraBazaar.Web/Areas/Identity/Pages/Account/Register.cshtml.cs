namespace CameraBazaar.Web.Areas.Identity.Pages.Account
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;
    using CameraBazaar.Data.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.Logging;

    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        // IdentityUser => App User
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            // IdentityUser => App User
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._logger = logger;
            this._emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            // Custom User Registration
            [Required]
            [StringLength(20,
                ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = 4)]
            [RegularExpression(@"^[a-zA-Z]{4,20}$",
                ErrorMessage = "The {0} must contain letters only and be at least 4 and at max 20 characters long.")]
            public string Username { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [Phone]
            [RegularExpression(@"^\+\d{10,12}$",
                ErrorMessage = "The {0} must start with '+' and contain between 10 and 12 digits.")]
            [Display(Name = "Phone number")]
            public string Phone { get; set; }

            [Required]
            [StringLength(100,
                ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = 3)]
            [RegularExpression(@"^[a-z0-9]{3,100}$",
                ErrorMessage = "The {0} must be at least 3 symbols long and contain only lowercase letters and digits.")]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password",
                ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet(string returnUrl = null) => this.ReturnUrl = returnUrl;

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? this.Url.Content("~/");
            if (this.ModelState.IsValid)
            {
                // Custom User Registration
                //var user = new IdentityUser
                var user = new User
                {
                    UserName = this.Input.Username,
                    Email = this.Input.Email,
                    PhoneNumber = this.Input.Phone
                };

                var result = await this._userManager.CreateAsync(user, this.Input.Password);
                if (result.Succeeded)
                {
                    this._logger.LogInformation("User created a new account with password.");

                    var code = await this._userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = this.Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: this.Request.Scheme);

                    await this._emailSender.SendEmailAsync(this.Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    await this._signInManager.SignInAsync(user, isPersistent: false);
                    return this.LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return this.Page();
        }
    }
}
