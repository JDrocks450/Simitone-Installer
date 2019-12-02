using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simitone.Installer.Driver.Util
{
    public enum MessageSeverity
    {
        Low, // suggestion
        Warning, // recommend action
        High // installation cannot continue
    }
    public interface IInstallationMessage
    {
        MessageSeverity Severity
        {
            get; set;
        }
        string Title
        {
            get; set;
        }
        string Text
        {
            get; set;
        }
    }

    public class WarningMessage : IInstallationMessage
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public MessageSeverity Severity { get; set; }
    }
}
