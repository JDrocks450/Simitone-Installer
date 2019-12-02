using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Sayonara
{
    /// <summary>
    /// Provides directory navigation and enumeration
    /// </summary>
    public class DirectoriesController : ApiController
    {
        // GET api/directories
        public IEnumerable<string> Get() //fetch directories
        {
            return NetFileSystem.GetDirectoriesInCurrentDirectory();
        }

        // GET api/directories/{name}
        public HttpResponseMessage Get(string name) // gets directory in current
        {
            var directory = NetFileSystem.NavigateToDirectoryInCurrent(name, out bool result);
            var response = Request.CreateResponse(result ? HttpStatusCode.Accepted : HttpStatusCode.BadRequest);
            response.Content = new StringContent(directory);
            return response;
        }

        // POST api/directories 
        public HttpResponseMessage Post([FromBody]string value)
        {
            Out.PrintLine("POST directory " + value);
            var result = NetFileSystem.JumpToDirectory(value);
            var response = Request.CreateResponse(result ? HttpStatusCode.Accepted : HttpStatusCode.BadRequest);
            response.Content = new StringContent(NetFileSystem.GetCurrentDirectory());
            return response;
        }

        // PUT api/directories/subpath
        public void Put(string name, [FromBody]string value)
        {

        }

        // DELETE api/directories/5 
        public void Delete(string name)
        {

        }
    }
}
