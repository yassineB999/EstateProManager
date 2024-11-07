using EstateProManager.Models;
using Microsoft.EntityFrameworkCore;

namespace EstateProManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Set up the connection string and database context
            var connectionString = builder.Configuration.GetConnectionString("db_one");

            builder.Services.AddDbContext<EstateProManagerContext>(options =>
                options.UseSqlServer(connectionString));

            ConfigureServices(builder.Services);

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
        }
    }
}
