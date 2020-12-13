namespace Tickr
{
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
    using ResiliencePolicyHandlers;
    using Services;
    using Settings;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddConfig<RavenSettings>(Configuration.GetSection("RavenSettings"));
            services.AddConfig<ResilienceSettings>(Configuration.GetSection("Resilience"));
            var authSettings = services.AddConfig<AuthSettings>(Configuration.GetSection("Auth0"));
            
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


            services.AddSingleton<DataSource>();
            services.AddTransient<IDataService, DataService>();
            services.AddTransient<RetryPolicyHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.AddUserSecrets<Startup>();
                app.UseDeveloperExceptionPage();
            }
            
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<TodoService>();

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
