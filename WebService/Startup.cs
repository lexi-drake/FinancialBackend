using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using FluentValidation;
using FluentValidation.AspNetCore;
using Serilog;

namespace WebService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                // AllowAnyOrigin is incompatible with AllowCredentials so we're just
                // not checking origins.
                var origins = Configuration["ORIGINS"].Split(',');
                options.AddPolicy("allow",
                builder => builder.WithOrigins(origins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                );
            });

            services.AddMemoryCache();
            services.AddMvc().AddFluentValidation();

            var logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File("./log.log")
                .WriteTo.Console()
                .CreateLogger();
            services.AddScoped<ILogger>(s => logger);

            var audience = Configuration["AUDIENCE"];
            var issuer = Configuration["ISSUER"];
            var secret = Configuration["JWT_SECRET"];
            services.AddScoped<IJwtHelper>(s => new JwtHelper(secret, issuer, audience));
            services.AddScoped<ILedgerService, LedgerService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAdminService, AdminService>();

            services.AddScoped<ILedgerRepository>(s =>
                new LedgerRepository(s.GetRequiredService<IMemoryCache>(), Configuration["MONGO_DB"], Configuration["LEDGER_DB"]));
            services.AddScoped<IUserRepository>(s =>
                new UserRepository(Configuration["MONGO_DB"], Configuration["USERS_DB"]));

            services.AddTransient<IValidator<CreateUserRequest>, CreateUserRequestValidator>();
            services.AddTransient<IValidator<IncomeGeneratorRequest>, IncomeGeneratorRequestValidator>();
            services.AddTransient<IValidator<LedgerEntryRequest>, LedgerEntryRequestValidator>();
            services.AddTransient<IValidator<LoginRequest>, LoginRequestValidator>();
            services.AddTransient<IValidator<RecurringTransactionRequest>, RecurringTransactionRequestValidator>();
            services.AddTransient<IValidator<FrequencyRequest>, FrequencyRequestValidator>();
            services.AddTransient<IValidator<SalaryTypeRequest>, SalaryTypeRequestValidator>();
            services.AddTransient<IValidator<TransactionTypeRequest>, TransactionTypeRequestValidator>();
            services.AddTransient<IValidator<UserRoleRequest>, UserRoleRequestValidator>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Cookies["jwt"].ExtractJwtFromCookie();
                        context.Token = token;
                        return Task.CompletedTask;
                    }
                };
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(secret)),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("allow");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
