using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simitone.Installer.Driver
{
    public class AsyncStatus
    {
        public event EventHandler DataChanged;
        public string Status {
            get => _status;
            set
            {
                _status = value;
                DataChanged?.Invoke(this, null);
            }
        }

        public int Percent
        {
            get => percent;
            set
            {
                percent = value;
                DataChanged?.Invoke(this, null);
            }
        }

        private int percent;
        private string _status;
    }
}
