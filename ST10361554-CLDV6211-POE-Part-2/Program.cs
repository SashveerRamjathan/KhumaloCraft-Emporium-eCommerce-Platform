using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ST10361554_CLDV6211_POE_Part_2.Data;
using ST10361554_CLDV6211_POE_Part_2.Services;

namespace ST10361554_CLDV6211_POE_Part_2
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("AzureDatabaseConnection") ?? throw new InvalidOperationException("Connection string 'AzureDatabaseConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            //Register the other DbContext class
            builder.Services.AddDbContext<KhumaloCraftDatabaseContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                //add roles 
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();

            //Register the shopping cart service
            builder.Services.AddSingleton<ShoppingCartService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            //add scope
            using (var scope = app.Services.CreateScope())
            {
                //seed some initial data
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                var roles = new[] { "Admin", "Artist", "Customer" };

                foreach (var role in roles)
                {
                    //don't want duplicate roles to be created every time app runs
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }
            }

            //add scope
            using (var scope = app.Services.CreateScope())
            {
                //seed some initial data
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                string email = "admin@admin.com";
                string password = "Admin@123";

                //check to see if user is created already
                if (await userManager.FindByEmailAsync(email) == null)
                {
                    //create new user
                    var user = new IdentityUser();
                    user.Email = email;
                    user.UserName = email;

                    //register user
                    await userManager.CreateAsync(user, password);

                    //adding a role to the user
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }

            app.Run();
        }
    }
}
