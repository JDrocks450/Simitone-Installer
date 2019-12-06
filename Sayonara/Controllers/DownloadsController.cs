using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Sayonara
{
    /// <summary>
    /// Provides file downloads to single files
    /// </summary>
    public class DownloadsController : ApiController
    {
        // GET api/downloads 
        public IEnumerable<string> Get()
        {
            return NetFileSystem.GetFilesInDirectory();
        }

        // GET api/downloads/{name}
        public HttpResponseMessage Get(string name) // returns a binary stream of file data for the requested file
        {
            Out.PrintLine(name + " is being downloaded...");
            var file = NetFileSystem.GetFile(name);
            if (file == null)
            {
                Out.PrintLine(name + " could not be found!");
                return null;
            }
            HttpResponseMessage msg = Request.CreateResponse(HttpStatusCode.Accepted);
            var stream = file.CreateReadStream();
            msg.Content = new StreamContent(stream);
            msg.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");
            Out.PrintLine(name + " was downloaded!");
            return msg;
        }
    }
}
