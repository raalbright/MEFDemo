using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Composition;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.IO;
using System.Runtime.Loader;
using MEFDemo.Shared.Plugin;

namespace MEFDemo.WebHost
{
    public class Startup
    {
        [ImportMany("ControllerBase")]
        public IEnumerable<IPlugin> controllerBases { get; set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            this.LoadDependencies(@"C:\Users\ralbright\source\repos\MEFDemo\bin\netcoreapp3.1");
        }

        private void LoadDependencies(String path)
        {
            path = path ?? AppContext.BaseDirectory;

            var assemblies = Directory.GetFiles( path, "*.dll" )
                            .Select(  AssemblyLoadContext.Default.LoadFromAssemblyPath );

            var configuration = new ContainerConfiguration()
            .WithAssemblies(assemblies);

            using (var container = configuration.CreateContainer())
            {
                var plugins = container
                    .GetExports<IPlugin>();
            }
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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
