using BeaconLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sayonara
{
    /// <summary>
    /// Wrapper for Probe for projects not referencing BeaconLib
    /// </summary>
    public class DiscoveryHandler
    {
        private Probe probe;
        private Action<IEnumerable<(string Address, string Message)>> callback;
        public DiscoveryHandler()
        {
            probe = new Probe("Sayonara");
        }

        public void BeginListening(Action<IEnumerable<(string Address, string Message)>> ServersFoundCallback)
        {
            callback = ServersFoundCallback;
            probe.Start();
            probe.BeaconsUpdated += DiscoveryHandler_BeaconsUpdated;
        }

        private void DiscoveryHandler_BeaconsUpdated(IEnumerable<BeaconLocation> obj)
        {            
            callback?.Invoke(obj.Select(x => (x.Address.Address.ToString(), x.Data)));
        }
    }
}
