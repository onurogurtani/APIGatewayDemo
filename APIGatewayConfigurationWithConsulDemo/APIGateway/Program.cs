using APIGateway.Aggregations;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Cache.CacheManager;
using CacheManager.Core;
using MMLib.Ocelot.Provider.AppConfiguration;
using Newtonsoft.Json.Converters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddNewtonsoftJson(p => p.SerializerSettings.Converters.Add(new StringEnumConverter()));
builder.Services.AddMvcCore()
        .AddNewtonsoftJson(p => p.SerializerSettings.Converters.Add(new StringEnumConverter()))
        .AddApiExplorer();
builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

builder.Services.AddSwaggers(builder.Environment, builder.Configuration);
                 

AdminApiConfiguration adminApiConfiguration = builder.Configuration.GetSection("AdminApiConfiguration").Get<AdminApiConfiguration>();

IdentityModelEventSource.ShowPII = true;

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

builder.Services.AddAuthentication(a =>
{
    a.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    a.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer("ApiSecurity", options);

builder.Services.AddAuthorization();

builder.Services
    .AddOcelot(builder.Configuration)
    .AddAppConfiguration()
    //.AddAdministration("/administration", options)
    .AddCacheManager(x =>
    {
        x.WithRedisConfiguration("redis",
                config =>
                {
                    config.WithAllowAdmin()
                    .WithDatabase(0)
                    .WithEndpoint("localhost", 6379);
                })
        .WithJsonSerializer()
        .WithRedisCacheHandle("redis");
    })
    .AddSingletonDefinedAggregator<CustomAggregator>()
    .AddConsul()
    .AddConfigStoredInConsul();

var env = builder.Environment.EnvironmentName;
builder.Configuration.SetBasePath(builder.Environment.ContentRootPath);
builder.Configuration.AddJsonFile($"appsettings.{env}.json", true, true);
builder.Configuration.AddJsonFile($"ocelot.{env}.json", true, true);
builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseAuthentication();
app.UseAuthorization();

app.UseSwaggers(builder.Configuration);

app.MapControllers();

app.UseOcelot();

app.Run();




