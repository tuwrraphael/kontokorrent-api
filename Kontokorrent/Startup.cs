using System.IO;
using System.Security.Claims;
using System.Text;
using Kontokorrent.Impl;
using Kontokorrent.Impl.EF;
using Kontokorrent.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Kontokorrent
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            WebHostEnvironment = environment;
            environment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebHostEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<JWTOptions>(Configuration);

            services.AddDbContext<KontokorrentContext>(options =>
                options.UseSqlite($"Data Source={WebHostEnvironment.WebRootPath}\\App_Data\\kontokorrent.db")
            );
            services.AddTransient<IKontokorrentRepository, KontokorrentRepository>();
            services.AddTransient<IPersonRepository, PersonRepository>();
            services.AddTransient<IBezahlungRepository, BezahlungRepository>();
            services.AddTransient<ITokenService, TokenService>();

            services.AddControllers();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["IssuerAudience"],
                        ValidAudience = Configuration["IssuerAudience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Configuration["Secret"])),

                    };
                });
            services.AddAuthorization(o =>
            {
                o.DefaultPolicy = new AuthorizationPolicyBuilder(new[] { JwtBearerDefaults.AuthenticationScheme })
                    .RequireAuthenticatedUser()
                    .RequireClaim(ClaimTypes.Name)
                    .Build();
            });

            services.AddCors(options =>
            {
                options.AddPolicy("P", builder => builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithOrigins("http://localhost:8080", "https://kontokorrent.kesal.at"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("P");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
