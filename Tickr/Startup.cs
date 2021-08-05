namespace Tickr
{
    using AutoMapper;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Logging;
    using Repositories;
    using ResiliencePolicyHandlers;
    using Services;
    using Settings;
    using Talista.Utilities.Encoding;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
        {
            var authSettings = services.AddConfig<AuthSettings>(Configuration.GetSection("Auth0"));

            services.AddConfig<RavenSettings>(Configuration.GetSection("RavenSettings"));
            services.AddConfig<ResilienceSettings>(Configuration.GetSection("Resilience"));
            
            
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<JwtBearerOptions>, JwtBearerPostConfigureOptions>());
            services.AddAuthentication("Auth0")
                .AddScheme<JwtBearerOptions, JwtAuthenticationHandler>("Auth0", options =>
                {
                    options.Audience = authSettings.Audience;
                    options.Authority = $"https://{authSettings.Domain}/";
                });
               

            IdentityModelEventSource.ShowPII = true;

            services.AddAuthorization(options =>
            {
                options.AddPolicy("HasReadScope",
                    policy => { policy.RequireScope("grpc_read_scope", "grpc_modify_scope"); });
                options.AddPolicy("HasModifyScope",
                    policy => { policy.RequireScope("grpc_modify_scope"); });
                
                
            });
            
            services.AddGrpc(options => options.EnableDetailedErrors = true);
            services.AddAutoMapper(typeof(Startup));
            services.AddSingleton<DataSource>();
            services.AddSingleton<IIdentifierMasking, IdentifierMasking>();
            services.AddTransient<IDataService, DataService>();
            services.AddTransient<RetryPolicyHandler>();
            services.AddTransient<TodoRepository>();
        }
    
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<TodoService>();
                endpoints.MapGrpcReflectionService();

                endpoints.MapGet("/",
                    async context =>
                    {
                        await context.Response.WriteAsync(
                            "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                    });
            });
        }
    }
}
