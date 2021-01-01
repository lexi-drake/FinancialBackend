using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
                options.AddPolicy(name: "allow", builder =>
                {
                    builder.WithOrigins("*");
                    builder.WithHeaders("Content-Type");
                });
            });
            services.AddLogging();

            services.AddScoped<JwtHelper>(s => new JwtHelper("", "", ""));
            services.AddScoped<ILedgerService, LedgerService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ILedgerRepository>(s =>
                new LedgerRepository(Configuration["ConnectionStrings:MongoDB"], Configuration["ConnectionStrings:LedgerDB"]));
            services.AddScoped<IUserRepository>(s =>
                new UserRepository(Configuration["ConnectionStrings:MongoDB"], Configuration["ConnectionStrings:UsersDB"]));

            services.AddMvc().AddFluentValidation();
            services.AddTransient<IValidator<CreateUserRequest>, CreateUserRequestValidator>();
            services.AddTransient<IValidator<IncomeGeneratorRequest>, IncomeGeneratorRequestValidator>();
            services.AddTransient<IValidator<LedgerEntryRequest>, LedgerEntryRequestValidator>();
            services.AddTransient<IValidator<LoginRequest>, LoginRequestValidator>();
            services.AddTransient<IValidator<RecurringTransactionRequest>, RecurringTransactionRequestValidator>();

            services.AddAuthentication(configuratinOptions =>
            {
                configuratinOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(Configuration["Jwt:Secret"])),
                    ValidateIssuer = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = Configuration["Jwt:Audience"],
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
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
