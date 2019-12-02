using System;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;

//[assembly: OwinStartup(typeof(Sayonara.FileServerBuilder))]

namespace Sayonara
{
    public class FileServerBuilder
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            var physicalFileSystem = new PhysicalFileSystem(@"D:\Games\The Sims Complete Collection");
            var options = new FileServerOptions
            {
                EnableDirectoryBrowsing = true,
                EnableDefaultFiles = true,
                FileSystem = physicalFileSystem,
                StaticFileOptions = {
                    FileSystem = physicalFileSystem,
                    ServeUnknownFileTypes = true
                },
                DefaultFilesOptions =
                {
                    DefaultFileNames = new[]
                    {
                        "index.html"
                    }
                }
            };

            appBuilder.UseFileServer(options);

            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "SayonaraTransferApi",
                routeTemplate: "api/{controller}/{name}", //command format
                defaults: new { name = RouteParameter.Optional }
                );

            appBuilder.UseWebApi(config);
        }
    }
}
