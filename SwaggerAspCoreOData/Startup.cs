﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using SwaggerAspCoreOData.Controllers;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SwaggerAspCoreOData
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
      readonly IApiVersionDescriptionProvider provider;

      public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) =>
        this.provider = provider;

      public void Configure(SwaggerGenOptions options)
      {
        foreach (var description in provider.ApiVersionDescriptions)
        {
          options.SwaggerDoc(
            description.GroupName,
              new Info()
              {
                Title = $"Sample API {description.ApiVersion}",
                Version = description.ApiVersion.ToString(),
              });
        }
      }
    }
    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMvc(options => options.EnableEndpointRouting = false);

      services.AddODataApiExplorer(options =>
      {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;       

        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = new ApiVersion(1, 0);
      });

      services.AddApiVersioning();
      services.AddOData().EnableApiVersioning();
      services.AddVersionedApiExplorer(options => options.GroupNameFormat = "'v'VVV");
      services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
      services.AddSwaggerGen();

      services.AddMvc(opt =>
      {       
      }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, VersionedODataModelBuilder modelBuilder,
    IApiVersionDescriptionProvider provider)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      var models = modelBuilder.GetEdmModels();

      app.UseMvc(routes =>
      {
        routes.MapVersionedODataRoutes("api", "v{version:apiVersion}", models);
        routes.Count().Filter().OrderBy().Expand().Select().MaxTop(null);
      });
      app.UseSwagger();
      app.UseSwaggerUI(
          options =>
          {
            foreach (var description in provider.ApiVersionDescriptions)
            {
              options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
            }
          });

      app.UseHttpsRedirection();
      app.UseMvc();
    }
  }
}
