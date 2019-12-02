using Microsoft.Owin.FileSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Sayonara
{
    /// <summary>
    /// Provides file info -- note that it does not handle downloading any files, just information
    /// </summary>
    public class FilesController : ApiController
    {
        // GET api/files 
        public IEnumerable<string> Get()
        {
            return NetFileSystem.GetFilesInDirectory();
        }

        // GET api/files/{name}
        public IFileInfo Get(string name)
        {
            var file = NetFileSystem.GetFile(name);
            if (file == null)
            {
                return null;
            }
            return file;
        }

        // POST api/files 
        public void Post([FromBody]string value)
        {

        }

        // PUT api/files/5 
        public void Put(string name, [FromBody]string value)
        {
        }

        // DELETE api/files/5 
        public void Delete(string name)
        {

        }
    }
}
