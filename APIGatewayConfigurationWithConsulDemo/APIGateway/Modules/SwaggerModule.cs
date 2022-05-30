using APIGateway.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;

public static class SwaggerModule
{
    private static readonly Assembly assembly;

    static SwaggerModule()
    {
        assembly = Assembly.GetExecutingAssembly();
    }

    public static IServiceCollection AddSwaggers(this IServiceCollection services, IWebHostEnvironment environment, IConfiguration configuration)
    {
        var header = environment.IsProduction() ? "" : $"[ {environment.EnvironmentName} ] - ";

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = $"{header} {assembly.GetName().Name} v{assembly.GetName().Version}",
                Version = "v1",
                Description = assembly.GetCustomAttributes<AssemblyDescriptionAttribute>().FirstOrDefault()?.Description,
                Contact = new OpenApiContact
                {
                    Name = "",
                    Email = "",
                }
            });

            string openApiSecurityTypeId = "";

            openApiSecurityTypeId = JwtBearerDefaults.AuthenticationScheme;
            options.AddSecurityDefinition(openApiSecurityTypeId, new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });


            if (string.IsNullOrEmpty(openApiSecurityTypeId) == false)
            {
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = openApiSecurityTypeId
                                    }
                                },
                                new string[] {}
                        }
                    });
            }

            options.UseInlineDefinitionsForEnums();
            options.ExampleFilters();
            options.OperationFilter<AddResponseHeadersFilter>();

            var xmlFile = $"{assembly.GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath, true);
        });
        services.AddSwaggerGenNewtonsoftSupport();
        services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());

        services.AddSwaggerForOcelot(configuration);
        return services;
    }
    public static IApplicationBuilder UseSwaggers(this IApplicationBuilder app, IConfiguration configuration)
    {
        app.UseSwagger(c => c.SerializeAsV2 = true);

        app.UseSwaggerForOcelotUI(opt =>
        {
            opt.DownstreamSwaggerEndPointBasePath = "/swagger/docs";
            opt.PathToSwaggerGenerator = "/swagger/docs";
            opt.DefaultModelsExpandDepth(-1);
        });

        return app;
    }
}