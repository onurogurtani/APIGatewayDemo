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
using APIGateway.Modules;

namespace APIGateway.Modules
{
    public static class OcelotModule
    {
        static OcelotModule()
        {
        }

        public static IServiceCollection AddOcelots(this IServiceCollection services, IWebHostEnvironment environment, IConfiguration configuration)
        {
            services.AddOcelot(configuration).AddAppConfiguration().AddCacheManager(x =>
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

            return services;

        }
        public static IApplicationBuilder UseOcelots(this IApplicationBuilder app, IConfiguration configuration)
        {
            app.UseOcelot();
            return app;
        }
    }
}
