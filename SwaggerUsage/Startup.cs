using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SwaggerUsage.Controllers;

namespace SwaggerUsage
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
            services.AddControllers();
            
            // Versioning the API
            Settings settings = Configuration.Get<Settings>();
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.ApiVersionReader = new HeaderApiVersionReader("api-version");
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(settings.Version.Major, settings.Version.Minor);
                // Conventions
                options.Conventions.Controller<WeatherController>().HasApiVersions(new ApiVersion[]
                {
                    new ApiVersion(1, 0),
                    new ApiVersion(2, 0)
                });
                options.Conventions.Controller<ValuesController>().HasApiVersions(new ApiVersion[]
                {
                    new ApiVersion(2, 0)
                });
            });
            services.AddVersionedApiExplorer();

            // Adding Swagger
            services.AddSwaggerGen(options =>
            {
                // Old version
                options.SwaggerDoc("v1.0", new OpenApiInfo
                {
                    Title = settings.ExhibitionName,
                    Version = "v1.0",
                });

                // Automatically generated with our AppSettings
                string defaultVersion = $"v{settings.Version.Major}.{settings.Version.Minor}";
                options.SwaggerDoc(defaultVersion, new OpenApiInfo
                {
                    Title = settings.ExhibitionName,
                    Version = defaultVersion
                });

                // Separate endpoints based on ApiVersion
                options.DocInclusionPredicate((version, apiDescription) =>
                {
                    if ($"v{apiDescription.GroupName}" == version)
                        return true;
                    else
                        return false;
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            Settings settings = Configuration.Get<Settings>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(option =>
                {
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        option.SwaggerEndpoint($"{settings.Url}/swagger/v{description.GroupName}/swagger.json", $"v{description.GroupName}");
                    }
                });
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
