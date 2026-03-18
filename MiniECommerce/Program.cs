using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.Configuration;
using MiniECommerce.Data;
using MiniECommerce.Models.IdentityModels;

namespace MiniECommerce
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ApplicationDbContext>(options=>
            {
                String? ConnectionString = builder.Configuration["ConnectionStrings:ConnectionString"];
                if(ConnectionString != null)
                    options.UseSqlServer(ConnectionString);
                else
                {
                    throw new InvalidConfigurationException("Couldn't Retrieve Connection String Properly from the AppSettings.json"); 
                }
            });
            
            builder.Services.AddIdentity<ApplicationUser,ApplicationRole>
                (options=>{
                    options.Password.RequireDigit = true;              // Require at least one number (0-9)
                    options.Password.RequireLowercase = true;          // Require at least one lowercase letter
                    options.Password.RequireUppercase = true;          // Require at least one uppercase letter
                    options.Password.RequireNonAlphanumeric = true;    // Require at least one special character (!@#$%^&*)
                    options.Password.RequiredLength = 8;               // Minimum length
                    options.Password.RequiredUniqueChars = 4;          // Must use at least 4 different characters

                    // LOCKOUT OPTIONS - prevent brute force attacks
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);  // Lock duration
                    options.Lockout.MaxFailedAccessAttempts = 5;                       // Attempts before lock
                    options.Lockout.AllowedForNewUsers = true;                         // Enable for new users

                    // USER OPTIONS
                    options.User.RequireUniqueEmail = true;            // One account per email address
                    options.User.AllowedUserNameCharacters =
                        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

                    
                    options.SignIn.RequireConfirmedEmail = false;      

                    // TOKEN OPTIONS
                    options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
                    options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
                })
                 .AddEntityFrameworkStores<ApplicationDbContext>()
                 .AddDefaultTokenProviders(); // To Generate Token for EmailConfirmation

            builder.Services.ConfigureApplicationCookie(options =>
            {
                // LoginPath: Where to redirect when [Authorize] fails due to anonymous user
                options.LoginPath = "/Account/Login";

                // AccessDeniedPath: Where to redirect when user is authenticated but lacks required role/permission
                options.AccessDeniedPath = "/Account/AccessDenied";

                // Cookie security settings
                options.Cookie.HttpOnly = true;  // Prevents JavaScript from accessing cookie (XSS protection)
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;  // Require HTTPS in production
                options.Cookie.SameSite = SameSiteMode.Lax;  // CSRF protection 'balance becasue it allow get request from other sites'  
                options.ExpireTimeSpan = TimeSpan.FromDays(7);  // How long persistent cookies last
            });
//===========================================================================================================================
            //---------------------- Add-Dependency_Injection HERE ----------------------------


            //---------------------------------------------------


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");


            //=================================================
            // SEED INITIAL DATA 
            // ============================================
            
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

                await DbInitializer.SeedAdminAsync(userManager, roleManager);
            }

            app.Run();
        }
    }

    public static class DbInitializer
    {
        public static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager,
                                                RoleManager<ApplicationRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new ApplicationRole {Name="Admin",NormalizedName="ADMIN"});
            }

            var email = "admin@commerce.com";
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user, "Admin123!@#");
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}
