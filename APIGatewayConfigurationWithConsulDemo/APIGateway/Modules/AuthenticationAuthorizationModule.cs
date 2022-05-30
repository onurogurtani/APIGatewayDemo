using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace APIGateway.Modules
{
    public static class AuthenticationAuthorizationModule
    {
        static AuthenticationAuthorizationModule()
        {
        }

        public static IServiceCollection AddAuth(this IServiceCollection services, IWebHostEnvironment environment, IConfiguration configuration)
        {
            AdminApiConfiguration adminApiConfiguration = configuration.GetSection("AdminApiConfiguration").Get<AdminApiConfiguration>();

            if (environment.IsProduction() != false)
            {
                IdentityModelEventSource.ShowPII = true;
            }


            Action<JwtBearerOptions> options = o =>
            {
                o.Authority = adminApiConfiguration.IdentityServerBaseUrl;
                o.RequireHttpsMetadata = adminApiConfiguration.RequireHttpsMetadata;
                o.Audience = adminApiConfiguration.OidcApiName;
                o.TokenValidationParameters = new TokenValidationParameters()
                {
                    NameClaimType = JwtClaimTypes.Name,
                    RoleClaimType = JwtClaimTypes.Role, //var isAdmin = User.IsInRole("admin"); kontrol edilir.
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            };

            services.AddAuthentication(a =>
            {
                a.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                a.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer("ApiSecurity", options);

            services.AddAuthorization();
            return services;
        }
        public static IApplicationBuilder UseAuth(this IApplicationBuilder app)
        {
            app.UseAuthentication();

            app.UseAuthorization();
            return app;
        }
    }
}
