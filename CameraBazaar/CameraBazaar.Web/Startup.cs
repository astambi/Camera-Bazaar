namespace CameraBazaar.Web
{
    using CameraBazaar.Data;
    using CameraBazaar.Data.Models;
    using CameraBazaar.Web.Infrastructure.Extensions;
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
                //.AddDefaultIdentity<IdentityUser>()
                //.AddIdentity<IdentityUser, IdentityRole>() // Identity
                .AddIdentity<User, IdentityRole>() // IdentityUser => App User
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<CameraBazaarDbContext>()
                .AddDefaultTokenProviders(); // Identity

            services
                .Configure<IdentityOptions>(options =>
                {
                    // Password settings.
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    // User settings.
                    options.User.RequireUniqueEmail = true;
                });

            services
                .AddMvc(options =>
                {
                    options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
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

            // App Services
            services.AddDomainServices(); // register services with reflexion

            // Routing with lowercase Urls
            services.AddRouting(options => options.LowercaseUrls = true);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDatabaseMigration(); // Database Migrate

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
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
