using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Simitone.Installer.Driver.Util
{
    public class WindowsLocator // modified version from Simitone
    {

        public static string FindTheSims1(out bool result)
        {
            // Search relative directory similar to how macOS and Linux works; allows portability
            string localDir = @"../The Sims/";
            result = true;
            if (File.Exists(Path.Combine(localDir, "GameData", "Behavior.iff"))) return localDir;
            // Fall back to the default install location if the other two checks fail
            var path = @"C:\Program Files (x86)\Maxis\The Sims\".Replace('\\', '/');
            RegistryKey tsoKey = getTS1Key();
            string installDir = (string)tsoKey.GetValue("InstallPath");
            if (!installDir.EndsWith("/")) // trailing slash check
                installDir += "\\";
            path = installDir.Replace('\\', '/');
            result = TS1ExistsInDirectory(path);
            return path;
        }

        private static RegistryKey getTS1Key()
        {
            using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
            {
                //Find the path to TS1 on the user's system.
                RegistryKey softwareKey = hklm.OpenSubKey("SOFTWARE");

                if (Array.Exists(softwareKey.GetSubKeyNames(), delegate (string s) { return s.Equals("Maxis", StringComparison.InvariantCultureIgnoreCase); }))
                {
                    RegistryKey maxisKey = softwareKey.OpenSubKey("Maxis");
                    if (Array.Exists(maxisKey.GetSubKeyNames(), delegate (string s) { return s.Equals("The Sims", StringComparison.InvariantCultureIgnoreCase); }))
                    {
                        return maxisKey.OpenSubKey("The Sims");
                    }
                }
            }
            return null;
        }

        public static bool[] CheckAllEPKeys()
        {
            int checkThrough = 8;
            var ts1Key = getTS1Key();
            var arr = new bool[checkThrough];
            for(int i = 0; i < checkThrough; i++)
            {
                var keyName = $"{ (i > 0 ? "EP" : "") }{ (i > 1 ? i.ToString() : "") }Installed";
                arr[i] = (string)ts1Key.GetValue(keyName) == "1";
            }
            return arr;
        }

        private static bool TS1ExistsInDirectory(string path)
        {
            return File.Exists(Path.Combine(path, "GameData", "Behavior.iff"));
        }

        private static bool is64BitProcess = (IntPtr.Size == 8);
        private static bool is64BitOperatingSystem = is64BitProcess || InternalCheckIsWow64();

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process(
            [In] IntPtr hProcess,
            [Out] out bool wow64Process
        );

        /// <summary>
        /// Determines if this process is run on a 64bit OS.
        /// </summary>
        /// <returns>True if it is, false otherwise.</returns>
        public static bool InternalCheckIsWow64()
        {
            if ((Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1) ||
                Environment.OSVersion.Version.Major >= 6)
            {
                using (Process p = Process.GetCurrentProcess())
                {
                    bool retVal;
                    if (!IsWow64Process(p.Handle, out retVal))
                    {
                        return false;
                    }
                    return retVal;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
