using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Sayonara
{
    public class ConnectionsController : ApiController
    {
        /// <summary>
        /// Returns that the server is online
        /// </summary>
        /// <returns></returns>
        public bool Get()
        {
            return true;
        }

        /// <summary>
        /// Get a value by property name
        /// </summary>
        /// <param name="command">The command to run or property to return if applicable</param>
        /// <returns>null if invalid</returns>
        public bool? Get(string command)
        {
            switch (command.ToLowerInvariant())
            {
                case "canConnect":
                    return true; // possibly in future if clients can be disallowed for any reason.
            }
            return null;
        }
    }
}
