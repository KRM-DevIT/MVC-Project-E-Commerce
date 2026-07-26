using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.Configuration;

namespace MiniECommerce
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                String? ConnectionString = builder.Configuration["ConnectionStrings:ConnectionString"];
                if (ConnectionString != null)
                    options.UseSqlServer(ConnectionString);
                else
                {
                    throw new InvalidConfigurationException("Couldn't Retrieve Connection String Properly from the AppSettings.json");
                }
            });

            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>
                (
                options =>
                {
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
                // Cookie security settings
                options.Cookie.HttpOnly = true; // prevent js from accessing it via document.cookie to mitigate xss attack
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.ExpireTimeSpan = TimeSpan.FromDays(7);

                options.Events.OnRedirectToLogin = context =>
                {
                    var area = context.Request.RouteValues["area"]?.ToString();

                    string loginPath;

                    if (area == "Admin")
                    {
                        loginPath = "/Admin/Account/AdminLogin";
                    }

                    else
                    {
                        loginPath = "/Customer/Account/Login";
                    }

                    context.Response.Redirect(loginPath + "?ReturnUrl=" + Uri.EscapeDataString(context.Request.Path));
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    var area = context.Request.RouteValues["area"]?.ToString();

                    if (area == "Admin")
                    {
                        context.Response.Redirect("/Admin/Account/AccessDenied");
                    }
                    else
                    {
                        context.Response.Redirect("/Customer/Account/AccessDenied");
                    }

                    return Task.CompletedTask;
                };
            });

            //===========================================================================================================================
            //---------------------- Add-Dependency_Injection HERE ----------------------------

            #region InterfacesInjection
            // Unit of Work pattern - centralized repository management
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IAddressRepository, AddressRepository>();

            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IOrderItemService, OrderItemService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IAddressService, AddressService>();
            #endregion

            builder.Services.AddHttpContextAccessor(); // to be able to access the httpcontext in which we have session

            builder.Services.AddSession(); // MiddleWare adds session service to the application, allowing you to store and retrieve user-specific data across multiple requests. 

            builder.Services.AddScoped<CartService>();
            builder.Services.AddScoped<CheckoutService>();

            //======================================

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
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

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

                await DbInitializer.SeedIdentityAsync(
                    userManager,
                    roleManager,
                    app.Configuration);
            }

            app.Run();
        }
    }

    public static class DbInitializer
    {
        public static async Task SeedIdentityAsync(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IConfiguration configuration)
        {
            var rolesToSeed = new[]
            {
                ApplicationRoles.Admin,
                ApplicationRoles.Customer,
                ApplicationRoles.DemoAdmin
            };

            foreach (var roleName in rolesToSeed) // seeding the roles
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new ApplicationRole
                    {
                        Name = roleName,
                        RoleDescription = roleName == ApplicationRoles.DemoAdmin
                            ? "Read-only access to the administration demo."
                            : null,
                        RoleImageURL = roleName == ApplicationRoles.DemoAdmin
                            ? "fa-solid fa-eye"
                            : null
                    });
                }
            }

            var email = configuration["SeedAdmin:Email"];
            var password = configuration["SeedAdmin:Password"];

            var user = await userManager.FindByEmailAsync(email!);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    LastName = "User"
                };

                await userManager.CreateAsync(user, password!);
                await userManager.AddToRoleAsync(user, ApplicationRoles.Admin);
            }

            await SeedDemoAdminAsync(userManager);
        }

        private static async Task SeedDemoAdminAsync(
            UserManager<ApplicationUser> userManager)
        {
            var user = await userManager.FindByEmailAsync(
                ApplicationRoles.DemoAdminEmail);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = ApplicationRoles.DemoAdminEmail,
                    Email = ApplicationRoles.DemoAdminEmail,
                    EmailConfirmed = true,
                    FirstName = "Demo",
                    LastName = "Admin"
                };

                var createResult = await userManager.CreateAsync(user, ApplicationRoles.DemoAdminPassword);

                if (!createResult.Succeeded)
                {
                    var errors = string.Join("; ", createResult.Errors.Select(error => error.Description));

                    throw new InvalidOperationException($"Could not create the Demo Admin account: {errors}");
                }
            }
            else if (!await userManager.CheckPasswordAsync(user, ApplicationRoles.DemoAdminPassword))
            {
                var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);

                var resetResult = await userManager.ResetPasswordAsync(user, resetToken, ApplicationRoles.DemoAdminPassword);

                if (!resetResult.Succeeded)
                {
                    throw new InvalidOperationException("Could not set the Demo Admin password.");
                }
            }

            if (!await userManager.IsInRoleAsync(user, ApplicationRoles.DemoAdmin))
            {
                await userManager.AddToRoleAsync(
                    user,
                    ApplicationRoles.DemoAdmin);
            }
        }
    }
}
