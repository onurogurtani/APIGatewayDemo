﻿using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;

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


builder.Services.AddOcelot().AddConsul().AddConfigStoredInConsul();

var env = builder.Environment.EnvironmentName;
builder.Configuration.SetBasePath(builder.Environment.ContentRootPath);
builder.Configuration.AddJsonFile($"appsettings.{env}.json", true, true);
builder.Configuration.AddJsonFile($"ocelot.{env}.json", true, true);
builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.UseAuthorization();

app.MapControllers();

app.UseOcelot();

app.Run();
