using EstateProManager.Models;
using Microsoft.EntityFrameworkCore;

namespace EstateProManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("db_one");

            builder.Services.AddDbContext<EstateProManagerContext>(options => options.UseSqlServer(
                connectionString
                ));

            ConfigureServices(builder.Services);

            var app = builder.Build();

            Configure(app);

            app.Run();
        }


        /// <summary>
        /// Configures services for the ASP.NET Core application, including controllers with views and session services.
        /// </summary>
        /// <param name="services">The collection of services to configure.</param>
        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            // Add session services
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
        }


        /// <summary>
        /// Configures middleware components for the ASP.NET Core application, including error handling, static files, session, routing, and controllers.
        /// </summary>
        /// <param name="app">The application builder used to configure the application's request pipeline.</param>
        private static void Configure(WebApplication app)
        {
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error/E404");
            }

            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404)
                {
                    context.Request.Path = "/Error/E404";
                    await next();
                }
            });

            app.UseStaticFiles();

            app.UseRouting();

            // Use session middleware
            app.UseSession();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=EstatePro}/{action=Home}/{id?}");
        }
    }
}
