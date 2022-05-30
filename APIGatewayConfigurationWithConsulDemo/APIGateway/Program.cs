using Ocelot.Middleware;
using Newtonsoft.Json.Converters;
using APIGateway.Modules;

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

builder.Services.AddOcelots(builder.Environment, builder.Configuration);

builder.Services.AddAuth(builder.Environment, builder.Configuration);

var env = builder.Environment.EnvironmentName;
builder.Configuration.SetBasePath(builder.Environment.ContentRootPath);
builder.Configuration.AddJsonFile($"appsettings.{env}.json", true, true);
builder.Configuration.AddJsonFile($"ocelot.{env}.json", true, true);
builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();

app.UseAuth();

app.UseSwaggers(builder.Configuration);

app.MapControllers();

app.UseOcelots(builder.Configuration);

app.Run();




