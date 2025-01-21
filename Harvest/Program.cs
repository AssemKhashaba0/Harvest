using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DataAccess;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Models;
namespace Harvest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
     options.UseSqlServer(connectionString));


            //builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddIdentity<IdentityUser, IdentityRole>(
              Option => {
                  Option.Password.RequiredLength = 8;
                  Option.Password.RequireDigit = false;
                  Option.SignIn.RequireConfirmedAccount = false;
              })
           .AddEntityFrameworkStores<ApplicationDbContext>()
           .AddDefaultTokenProviders();

            builder.Services.AddScoped<IRepository<Category>, CategoriesRepository>();
            builder.Services.AddScoped<IRepository<Product>, ProductRepository>();
            builder.Services.AddScoped<IRepository<Message>, MessageRepository>();
            builder.Services.AddScoped<IRepository<BlogPost>, BlogPostRepository>();
            builder.Services.AddScoped<IRepository<HomePage>, HomePageRepository>();

            builder.Services.AddAuthorization(); // إضافة هذه الخدمة

            // Add Controllers (if needed for MVC controllers)
            builder.Services.AddControllers(); // إضافة هذه الخدمة إذا كنت تستخدم MVC controllers

            // If you use Razor Pages, also add this
            builder.Services.AddRazorPages();
            builder.Services.AddControllersWithViews();

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

            app.Run();
        }
    }
}
