using Microsoft.AspNetCore.Authentication.Cookies;
using ShoesShop.Crosscutting.Utilities.PayPal;
using ShoesShop.Domain.Collections;
using ShoesShop.Domain.Modules.Carts.Entities;
using ShoesShop.Domain.Modules.Carts.Services;
using ShoesShop.Domain.Modules.Categories.Entities;
using ShoesShop.Domain.Modules.Categories.Services;
using ShoesShop.Domain.Modules.Commons.Repositories;
using ShoesShop.Domain.Modules.Orders.Entities;
using ShoesShop.Domain.Modules.Orders.Services;
using ShoesShop.Domain.Modules.Products.Entities;
using ShoesShop.Domain.Modules.Products.Services;
using ShoesShop.Domain.Modules.Shares.Entities;
using ShoesShop.Domain.Modules.Users.Entities;
using ShoesShop.Domain.Modules.Users.Services;
using ShoesShop.Domain.Services.Modules.Carts.Services;
using ShoesShop.Domain.Services.Modules.Categories.Services;
using ShoesShop.Domain.Services.Modules.Orders.Services;
using ShoesShop.Domain.Services.Modules.Products.Services;
using ShoesShop.Domain.Services.Modules.Users.Services;
using ShoesShop.Infrastructure.Collections;
using ShoesShop.Infrastructure.Data.Databases;
using ShoesShop.Infrastructure.Data.UOW;
using ShoesShop.Infrastructure.Modules;
using ShoesShop.Web.BuilderAndServices;

namespace ShoesShop.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddDatabaseModule(Configuration);
            services.AddRepositoryModule();
            services.AddHttpContextAccessor();
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddMVCService();

            services.AddScoped<IRepositoryCollection, RepositoryCollection>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IOrderService, OrderService>();

            // services.AddScoped<test>();
            services.AddScoped<IGenericRepository<User, int>>(serviceProvider =>
            {
                var repositoryCollection = serviceProvider.GetRequiredService<IRepositoryCollection>();
                return repositoryCollection.GetRepository<User, int>();
            });

            services.AddScoped<IGenericRepository<Product, int>>(serviceProvider =>
            {
                var repositoryCollection = serviceProvider.GetRequiredService<IRepositoryCollection>();
                return repositoryCollection.GetRepository<Product, int>();
            });

            services.AddScoped<IGenericRepository<Category, int>>(serviceProvider =>
            {
                var repositoryCollection = serviceProvider.GetRequiredService<IRepositoryCollection>();
                return repositoryCollection.GetRepository<Category, int>();
            });

            services.AddScoped<IGenericRepository<CartItem, int>>(serviceProvider =>
            {
                var repositoryCollection = serviceProvider.GetRequiredService<IRepositoryCollection>();
                return repositoryCollection.GetRepository<CartItem, int>();
            });

            services.AddScoped<IGenericRepository<Order, int>>(serviceProvider =>
            {
                var repositoryCollection = serviceProvider.GetRequiredService<IRepositoryCollection>();
                return repositoryCollection.GetRepository<Order, int>();
            });

            services.AddScoped<IGenericRepository<Address, int>>(serviceProvider =>
            {
                var repositoryCollection = serviceProvider.GetRequiredService<IRepositoryCollection>();
                return repositoryCollection.GetRepository<Address, int>();
            });

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/User/Login";
                options.LogoutPath = "/User/Logout";
                options.AccessDeniedPath = "/User/Login";
                options.ExpireTimeSpan = TimeSpan.FromHours(2);
            });

            services.AddSingleton(x =>
            {
                var clientId = Configuration["PayPal:ClientId"] ?? throw new InvalidOperationException("PayPal:ClientId is not configured.");
                var clientSecret = Configuration["PayPal:ClientSecret"] ?? throw new InvalidOperationException("PayPal:ClientSecret is not configured.");
                var mode = Configuration["PayPal:Mode"] ?? throw new InvalidOperationException("PayPal:Mode is not configured.");
                return new PaypalClient(clientId, clientSecret, mode);
            });
            
            services.AddAuthorization();
            
            services.Configure<CloudinarySettings>(Configuration.GetSection("Cloudinary"));
            services.AddSingleton<CloudinaryService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.TestingConnectionDatabase();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            // using (var scope = app.ApplicationServices.CreateScope())
            // {
            //     var tester = scope.ServiceProvider.GetRequiredService<>();
            //     tester.CreateProduct().GetAwaiter().GetResult();
            // }
        }
    }
}