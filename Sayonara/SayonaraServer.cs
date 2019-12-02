using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Owin;
using Microsoft.Owin.StaticFiles;

namespace Sayonara
{
    public class SayonaraServer
    {
        IDisposable Server;
        public string Address
        {
            get; private set;
        }
        public SayonaraServer(string IP = "http://localhost", int port = 37575)
        {
            var address = IP + ":" + port + "/";
            Server = WebApp.Start<FileServerBuilder>(address);
            Address = address;
            Out.PrintLine("Sayonara is hosting at address: " + address);
        }
        
        public void Shutdown()
        {
            Server.Dispose();
        }
    }
}
