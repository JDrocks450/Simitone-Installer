using Simitone.Installer.Driver.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simitone.Installer.Driver
{
    public class InstallationContext
    {        
        private Queue<IInstallationMessage> _warnings;
        public IInstallationMessage[] Warnings => _warnings.ToArray();
        public bool HasErrors => Warnings.Length != 0;

        public bool[] InstalledComponents; // TS1, House Party, etc. (Not Patches)
        public string[] InstalledComponentsStrings;
        public bool IgnoreSimitonePathHasFiles = true;

        public bool TS1Installed;
        public string TS1InstallationPath;
        public string SimitonePath = "C:/Program Files/Simitone";

        public InstallationContext()
        {
            _warnings = new Queue<IInstallationMessage>();
        }

        public void PushWarning(string Title, string Message, MessageSeverity severity)
        {
            _warnings.Enqueue(new WarningMessage() { Title = Title, Text = Message, Severity = severity });
        }

        public IInstallationMessage PopWarning()
        {            
            return _warnings.Dequeue();
        }

        public static InstallationContext GetInstallationContext()
        {
            var context = new InstallationContext();
            if (!context.CheckTS1())
                return context;
            if (!context.CheckEPs())
                return context;
            context.PushWarning("A Note On Custom Content...", "Some custom content objects are not entirely " +
                "supported by Simitone yet. If you run into issues relating to to custom content, submit a bug" +
                " report.", MessageSeverity.Low);
            context.SimitonePath = context.PredictSimitonePath();
            return context;
        }

        private string PredictSimitonePath()
        {
            try
            {
                var dirName = Path.GetDirectoryName(TS1InstallationPath);
                return Path.Combine(Path.GetDirectoryName(dirName), "Simitone");
            }
            catch (Exception)
            {
                return null;
            }
        }

        private bool CheckTS1()
        {
            TS1InstallationPath = WindowsLocator.FindTheSims1(out TS1Installed);
            if (!TS1Installed)
            {
                PushWarning("The Sims 1 Is Not Installed! ",
                    "A complete installation of The Sims 1 and all of its expansion packs " +
                    "is required to use Simitone.", MessageSeverity.High);
                TS1InstallationPath = null;
            }
            return TS1Installed;
        }

        private bool CheckEPs()
        {
            InstalledComponents = WindowsLocator.CheckAllEPKeys();
            InstalledComponentsStrings = new string[InstalledComponents.Length];
            int amountMissing = InstalledComponents.Count(x => x == false), count = 0;
            string WarningString = "The following expansion(s) are missing: ";
            foreach (var EPinstalled in InstalledComponents)
            {
                string gameTitle = count.ToString();
                switch (count)
                {
                    case 0: count++; continue;
                    case 1: gameTitle = "The Sims: House Party"; break;
                    default: gameTitle = $"Expansion Pack: {count}"; break;
                }
                InstalledComponentsStrings[count] = gameTitle;
                if (!EPinstalled) WarningString += $"{gameTitle}, ";
                count++;
            }
            if (amountMissing > 0)
            {
                PushWarning($"Missing {amountMissing} Expansion Packs!", WarningString + " all expansion packs are required to use Simitone.", MessageSeverity.High);
                return false;
            }
            return true;
        }
    }
}
