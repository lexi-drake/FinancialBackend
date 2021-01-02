using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace WebService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("allow",
                builder =>
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                );
            });

            services.AddMvc().AddFluentValidation();
            services.AddLogging();

            services.AddScoped<JwtHelper>(s => new JwtHelper(
                Configuration["JWT_SECRET"],
                Configuration["ISSUER"],
                Configuration["AUDIENCE"]));
            services.AddScoped<ILedgerService, LedgerService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ILedgerRepository>(s =>
                new LedgerRepository(Configuration["MONGO_DB"], Configuration["LEDGER_DB"]));
            services.AddScoped<IUserRepository>(s =>
                new UserRepository(Configuration["MONGO_DB"], Configuration["USERS_DB"]));

            services.AddTransient<IValidator<CreateUserRequest>, CreateUserRequestValidator>();
            services.AddTransient<IValidator<IncomeGeneratorRequest>, IncomeGeneratorRequestValidator>();
            services.AddTransient<IValidator<LedgerEntryRequest>, LedgerEntryRequestValidator>();
            services.AddTransient<IValidator<LoginRequest>, LoginRequestValidator>();
            services.AddTransient<IValidator<RecurringTransactionRequest>, RecurringTransactionRequestValidator>();

            services.AddAuthorization();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(Configuration["JWT_SECRET"])),
                    ValidateIssuer = true,
                    ValidIssuer = Configuration["ISSUER"],
                    ValidateAudience = true,
                    ValidAudience = Configuration["AUDIENCE"],
                    ValidateLifetime = true
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["jwt"];
                        return Task.CompletedTask;
                    }
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
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
