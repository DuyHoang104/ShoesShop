using ShoesShop.Crosscutting.Utilities.PayPal;
using ShoesShop.Crosscutting.Utilities.VNpay;
using ShoesShop.Domain.Collections;
using ShoesShop.Domain.Modules.Admin.Admin.Services;
using ShoesShop.Domain.Modules.Messages.Entity;
using ShoesShop.Domain.Modules.Shares.Image.Entities;
using ShoesShop.Domain.Modules.Shares.Messages.Hubs;
using ShoesShop.Domain.Modules.Shares.Messages.Services;
using ShoesShop.Domain.Modules.Shares.Review.Entity;
using ShoesShop.Domain.Modules.Shares.Review.Services;
using ShoesShop.Domain.Modules.User.Carts.Entities;
using ShoesShop.Domain.Modules.User.Carts.Services;
using ShoesShop.Domain.Modules.User.Categories.Entities;
using ShoesShop.Domain.Modules.User.Categories.Services;
using ShoesShop.Domain.Modules.User.Commons.Repositories;
using ShoesShop.Domain.Modules.User.Orders.Entities;
using ShoesShop.Domain.Modules.User.Orders.Services;
using ShoesShop.Domain.Modules.User.Products.Entities;
using ShoesShop.Domain.Modules.User.Products.Services;
using ShoesShop.Domain.Modules.User.Shares.Entities;
using ShoesShop.Domain.Modules.User.Users.Entities;
using ShoesShop.Domain.Modules.User.Users.Services;
using ShoesShop.Domain.Services.Modules.Admins.Admin;
using ShoesShop.Domain.Services.Modules.Messages;
using ShoesShop.Domain.Services.Modules.Users.Carts.Services;
using ShoesShop.Domain.Services.Modules.Users.Categories.Services;
using ShoesShop.Domain.Services.Modules.Users.Orders.Services;
using ShoesShop.Domain.Services.Modules.Users.Products.Services;
using ShoesShop.Domain.Services.Modules.Users.Reviews.Services;
using ShoesShop.Domain.Services.Modules.Users.Users.Services;
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
            services.AddSignalR();
            
            services.AddCors(options =>
            {
                options.AddPolicy("ClientPermission", policy =>
                {
                    policy.AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials() 
                          .SetIsOriginAllowed(origin => true); 
                });
            });

            services.AddMVCService();

            services.AddScoped<IRepositoryCollection, RepositoryCollection>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IReviewService, ReviewService>();

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

            services.AddScoped<IGenericRepository<Cart, int>>(serviceProvider =>
            {
                var repositoryCollection = serviceProvider.GetRequiredService<IRepositoryCollection>();
                return repositoryCollection.GetRepository<Cart, int>();
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
            
            services.AddScoped<IGenericRepository<Message, int>>(serviceProvider =>
            {
                var repositoryCollection = serviceProvider.GetRequiredService<IRepositoryCollection>();
                return repositoryCollection.GetRepository<Message, int>();
            });

            services.AddScoped<IGenericRepository<Image, int>>(serviceProvider =>
            {
                var repositoryCollection = serviceProvider.GetRequiredService<IRepositoryCollection>();
                return repositoryCollection.GetRepository<Image, int>();
            });

            services.AddScoped<IGenericRepository<ImageUser, int>>(serviceProvider =>
            {
                var repositoryCollection = serviceProvider.GetRequiredService<IRepositoryCollection>();
                return repositoryCollection.GetRepository<ImageUser, int>();
            });

            services.AddScoped<IGenericRepository<ImageProduct, int>>(serviceProvider =>
            {
                var repositoryCollection = serviceProvider.GetRequiredService<IRepositoryCollection>();
                return repositoryCollection.GetRepository<ImageProduct, int>();
            });

            services.AddScoped<IGenericRepository<Review, int>>(serviceProvider =>
            {
                var repositoryCollection = serviceProvider.GetRequiredService<IRepositoryCollection>();
                return repositoryCollection.GetRepository<Review, int>();
            });

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "UserScheme";
            })
            .AddCookie("UserScheme", options =>
            {
                options.LoginPath = "/User/Login";
                options.LogoutPath = "/User/Logout";
                options.AccessDeniedPath = "/User/Login";
                options.ExpireTimeSpan = TimeSpan.FromHours(2);
                options.Cookie.Name = "UserCookie";
            })
            .AddCookie("AdminScheme", options =>
            {
                options.LoginPath = "/Admin/Login";
                options.LogoutPath = "/Admin/Logout";
                options.AccessDeniedPath = "/Admin/Login";
                options.Cookie.Name = "AdminCookie";
                options.ExpireTimeSpan = TimeSpan.FromHours(2);
            });

            services.AddSingleton(x =>
            {
                var clientId = Configuration["PayPal:ClientId"] ?? throw new InvalidOperationException("PayPal:ClientId is not configured.");
                var clientSecret = Configuration["PayPal:ClientSecret"] ?? throw new InvalidOperationException("PayPal:ClientSecret is not configured.");
                var mode = Configuration["PayPal:Mode"] ?? throw new InvalidOperationException("PayPal:Mode is not configured.");
                return new PaypalClient(clientId, clientSecret, mode);
            });

            services.AddScoped<IVnPayService, VnPayService>();
            
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
            
            app.UseCors("ClientPermission"); 
            
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chatHub"); 

                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}