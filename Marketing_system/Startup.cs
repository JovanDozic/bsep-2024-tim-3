using Marketing_system.BL.Contracts.IService;
using Marketing_system.BL.Hubs;
using Marketing_system.BL.Mapper;
using Marketing_system.BL.Service;
using Marketing_system.DA;
using Marketing_system.DA.Contexts;
using Marketing_system.DA.Contracts;
using Marketing_system.DA.Contracts.IRepository;
using Marketing_system.DA.Contracts.Model;
using Marketing_system.DA.Repository;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using System.Threading.RateLimiting;

namespace Marketing_system
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.Configure<SMTPConfig>(Configuration.GetSection("SMTPConfig"));
            services.Configure<HMACConfig>(Configuration.GetSection("HMACConfig"));

            services.AddDbContext<DataContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"),
                    x => x.MigrationsHistoryTable("__EFMigrationsHistory", "marketingsystem"));

            });
            services.AddHttpContextAccessor();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddSerilog(dispose: true);
            });

            services.AddAutoMapper(typeof(UserProfile));
            services.AddSwaggerGen(setup =>
            {
                setup.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Marketing API",
                    Version = "v1"
                });
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    BearerFormat = "JWT",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    Description = "Put **ONLY** your JWT Bearer token in the text box below!",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };
                setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
                setup.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { jwtSecurityScheme, Array.Empty<string>() }
            });
            });

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 16;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.User.RequireUniqueEmail = true;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+!?";
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
                options.LoginPath = "/Login";
                options.SlidingExpiration = true;
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().
                 AllowAnyHeader());
            });
            services.AddCors(options =>
            {
                options.AddPolicy("AllowHttps", options => options.WithOrigins("https://localhost:4200").AllowAnyMethod());
            });
            var key = Environment.GetEnvironmentVariable("JWT_KEY") ?? "marketingsystem_superssecret_key";
            var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "marketingsystem";
            var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "marketingsystem-front.com";
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("AuthenticationTokens-Expired", "true");
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            //Authorization
            services.AddAuthorization(options =>
            {
                options.AddPolicy("adminPolicy", policy => policy.RequireRole("Admin"));
                options.AddPolicy("clientPolicy", policy => policy.RequireRole("Client"));
                options.AddPolicy("employeePolicy", policy => policy.RequireRole("Employee"));
            });

            services.AddControllersWithViews();
            services.AddSignalR();

            BindServices(services);

            services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("fixed-basic", options =>
                {
                    options.PermitLimit = 10;
                    options.Window = TimeSpan.FromSeconds(30);
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = 0;
                });

                options.AddFixedWindowLimiter("fixed-standard", options =>
                {
                    options.PermitLimit = 100;
                    options.Window = TimeSpan.FromSeconds(30);
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = 0;
                });

                options.AddFixedWindowLimiter("fixed-golden", options =>
                {
                    options.PermitLimit = 10000;
                    options.Window = TimeSpan.FromSeconds(300);
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = 0;
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("AllowOrigin");

            app.UseCors("AllowHttps");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseRateLimiter();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NotificationHub>("/notificationHub");
            });
        }

        private void BindServices(IServiceCollection services)
        {
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<ITokenGeneratorRepository, TokenGeneratorRepository>();
            services.AddTransient<IPasswordHasher, PasswordHasher>();
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<IEmailHandler, EmailHandler>();
            services.AddTransient<IAdvertisementService, AdvertisementService>();
            services.AddTransient<IAlertService, AlertService>();
            services.AddScoped<IEncryptionService, EncryptionService>(provider =>
            {
                var encryptionKey = Configuration["EncryptionKey"]; // Ključ za šifrovanje, čuvajte ga u sigurnom okruženju
                return new EncryptionService(encryptionKey);
            });
            services.AddSingleton<ITempTokenManagerService, TempTokenManagerService>();
            services.AddHttpClient();
            services.AddSingleton<IReCAPTCHAService, ReCAPTCHAService>();
        }

    }
}
