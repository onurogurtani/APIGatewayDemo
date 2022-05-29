using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Ocelot.Authorization;
using Ocelot.Configuration;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using System.Net;
using System.Security.Claims;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    //c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "GW", Version = "v1" });
    //var basePath = PlatformServices.Default.Application.ApplicationBasePath;
    //var xmlPath = Path.Combine(basePath, "APIGatewayConfigurationWithConsulDemo.xml");
    //c.IncludeXmlComments(xmlPath);
});

AdminApiConfiguration adminApiConfiguration = builder.Configuration.GetSection("AdminApiConfiguration").Get<AdminApiConfiguration>();

IdentityModelEventSource.ShowPII = true;
builder.Services.AddAuthentication(a =>
{
    a.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    a.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer("ApiSecurity", options =>
    {
        options.Authority = adminApiConfiguration.IdentityServerBaseUrl;
        options.RequireHttpsMetadata = adminApiConfiguration.RequireHttpsMetadata;
        options.Audience = adminApiConfiguration.OidcApiName;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            NameClaimType = JwtClaimTypes.Name,
            RoleClaimType = JwtClaimTypes.Role, //var isAdmin = User.IsInRole("admin"); kontrol edilir.
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddOcelot(builder.Configuration).AddConsul().AddConfigStoredInConsul();

var a = builder.Services.First(x => x.ServiceType == typeof(IClaimsAuthorizer));

var env = builder.Environment.EnvironmentName;
builder.Configuration.SetBasePath(builder.Environment.ContentRootPath);
builder.Configuration.AddJsonFile($"appsettings.{env}.json", true, true);
builder.Configuration.AddJsonFile($"ocelot.{env}.json", true, true);
builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
 
app.UseOcelot();

app.Run();




