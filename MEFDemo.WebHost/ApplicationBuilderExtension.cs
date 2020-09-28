using System;
using System.Composition;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using MEFDemo.Shared.Plugin;
using Microsoft.AspNetCore.Builder;

namespace MEFDemo.WebHost
{
    public static class ApplicationBuilderExtensions

    {

        public static IApplicationBuilder UsePlugins(this IApplicationBuilder app, string path = null)
        {

            var conventions = new ConventionBuilder();

            conventions
               .ForTypesDerivedFrom<IPlugin>()
               .Export<IPlugin>()
               .Shared();

            path = path ?? AppContext.BaseDirectory;

            var assemblies = Directory.GetFiles( path, "*.dll" )
                            .Select(  AssemblyLoadContext.Default.LoadFromAssemblyPath );

            var configuration = new ContainerConfiguration()
            .WithAssemblies(assemblies, conventions);

            using (var container = configuration.CreateContainer())
            {
                var plugins = container
                    .GetExports<IPlugin>()
                    .OrderBy(p => p.GetType().GetCustomAttributes(typeof(ExportMetadataAttribute), true));
                    // .SingleOrDefault(x => x.Name == "Order")?.Value as IComparable ?? int.MaxValue);

                // foreach (var plugin in plugins)
                // {
                //     app.Use(async (ctx, next) =>
                //     {
                //         await plugin.InvokeAsync(ctx, null);
                //         await next();
                //     });
                // }
            }
            return app;
        }

    }
}