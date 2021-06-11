using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extras.DynamicProxy;
using autofac_interceptors.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace autofac_interceptors
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .AddControllersAsServices(); // important !

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "autofac_interceptors", Version = "v1" });
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {

            var types = typeof(Startup).Assembly
                .GetTypes()
                .SelectMany(s => s.GetMethods(), (k, c) => new { Type = k, Method = c })
                .GroupBy(s => s.Type, s => s.Method, (t, m) => new { Type = t, Methods = m })
                .Where(q => q.Methods.Any(s => s.GetCustomAttribute<TraceAttribute>() != null))
                .Select(s => s.Type);

            foreach (var type in types)
            {
                builder.RegisterType(type)
                    .EnableClassInterceptors()
                    .InterceptedBy(typeof(TraceInterceptor));
            }

            builder.RegisterType<TraceInterceptor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "autofac_interceptors v1"));
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
