namespace CameraBazaar.Web
{
    using AutoMapper;
    using CameraBazaar.Data;
    using CameraBazaar.Data.Models;
    using CameraBazaar.Web.Infrastructure.Extensions;
    using CameraBazaar.Web.Infrastructure.FIlters;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.UI;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .Configure<CookiePolicyOptions>(options =>
                {
                    options.CheckConsentNeeded = context => true;
                    options.MinimumSameSitePolicy = SameSiteMode.None;
                });

            services
                .AddDbContext<CameraBazaarDbContext>(options => options
                    .UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection")));

            services
                .AddIdentity<User, IdentityRole>() // IdentityUser => App User
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<CameraBazaarDbContext>()
                .AddDefaultTokenProviders(); // Identity

            services
                .Configure<IdentityOptions>(options =>
                {
                    // Password settings
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 3;
                    // User settings
                    options.User.RequireUniqueEmail = true;
                });

            // External Authentication providers
            services
                .AddAuthentication()
                .AddFacebook(options =>
                {
                    options.AppId = this.Configuration[WebConstants.AuthFacebookAppId];
                    options.AppSecret = this.Configuration[WebConstants.AuthFacebookAppSecret];
                })
                .AddGoogle(options =>
                {
                    options.ClientId = this.Configuration[WebConstants.AuthGoogleClientId];
                    options.ClientSecret = this.Configuration[WebConstants.AuthGoogleClientSecret];
                });

            // App Services
            services.AddDomainServices();

            // AutoMapper
            services.AddAutoMapper(typeof(Startup));

            // Routing with lowercase Urls
            services.AddRouting(options => options.LowercaseUrls = true);

            services
                .AddMvc(options =>
                {
                    options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
                    options.Filters.Add<LogAttribute>(); // global Logs
                    options.Filters.Add<TimerAttribute>(); // global Action Timer
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                // Identity
                .AddRazorPagesOptions(options =>
                {
                    options.AllowAreas = true;
                    options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
                    options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
                });

            // Identity
            services
                .ConfigureApplicationCookie(options =>
                {
                    options.LoginPath = $"/Identity/Account/Login";
                    options.LogoutPath = $"/Identity/Account/Logout";
                    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
                });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Database Migrations
            app.UseDatabaseMigration(this.Configuration[WebConstants.AdminPassword]);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                  name: "areas",
                  template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
