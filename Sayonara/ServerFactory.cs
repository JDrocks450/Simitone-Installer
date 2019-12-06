using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sayonara
{
    public class ServerFactory
    {
        public static SayonaraServer HostLocalServer()
        {
            return SayonaraServer.HostLocalServer();
        }
    }
}
