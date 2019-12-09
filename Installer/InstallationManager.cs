using Octokit;
using Simitone.Installer.Driver.Util;
using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Simitone.Installer.Driver
{
    public class InstallationManager
    {
        public bool HasErrors => InstallContext.HasErrors;
        public IInstallationMessage[] Warnings => InstallContext.Warnings;
        public InstallationContext InstallContext { get; private set; }
        public AsyncStatus CurrentStatus { get; private set; }

        public InstallationManager()
        {
            InstallContext = InstallationContext.GetInstallationContext();
            CurrentStatus = new AsyncStatus() { Status = "Waiting..." };
        }

        private void SetStatus(string text)
        {
            CurrentStatus.Status = text;
        }

        public async Task<bool> BeginInstallation()
        {
            var dir = new DirectoryInfo(InstallContext.SimitonePath);
            using (var f = File.Create(Path.Combine(dir.FullName, "test.txt"))) { } // test directory access
            File.Delete(Path.Combine(dir.FullName, "test.txt"));
            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
            var extractionPath = "simitone.zip";
            var result = await SimitoneDownload(extractionPath);
            if (!result)
                return false;
            result = ExtractSimitone(extractionPath);
            return true;
        }
        
        private bool ExtractSimitone(string path)
        {
            CurrentStatus.Percent = 0;            
            var basePath = InstallContext.SimitonePath;
            int count = 0;            
            using (var a = ZipFile.OpenRead(path))
            {
                a.Entries.Where(o => o.Name == string.Empty && !Directory.Exists(Path.Combine(basePath, o.FullName))).ToList()
                    .ForEach(o =>
                    {
                        var createPath = Path.Combine(basePath, o.FullName);
                        Directory.CreateDirectory(createPath);
                    });
                a.Entries.Where(o => o.Name != string.Empty).ToList()
                    .ForEach(e =>
                    {
                        var createPath = Path.Combine(basePath, e.FullName);
                        e.ExtractToFile(createPath, true);
                        SetStatus("Extracting: " + createPath);
                        count++;
                        CurrentStatus.Percent = (int)((count / (double)a.Entries.Count) * 100);                        
                    });
            }
            CurrentStatus.Percent = 100;
            SetStatus("Installation Completed");
            return true;
        }

        private async Task<bool> SimitoneDownload(string saveTo)
        {
            SetStatus("Finding Latest Version...");
            string url = await FindSimitoneReleaseUrl();
            if (url == null)
            {
                SetStatus("Failed to get latest release...");
                return false;
            }
            SetStatus("Downloading Simitone...");            
            bool downloading = true;
            using (var client = new WebClient())
            {
                client.DownloadFileAsync(new Uri(url), saveTo);
                client.DownloadProgressChanged += (object sender, DownloadProgressChangedEventArgs e) => 
                {
                    lock (CurrentStatus)
                    {
                        CurrentStatus.Percent = (int)(((double)e.BytesReceived / e.TotalBytesToReceive) * 100);
                    }
                };
                client.DownloadFileCompleted += (object s, AsyncCompletedEventArgs args) => downloading = false;
            }
            while (downloading)
            {
                await Task.Delay(1000);
            }
            CurrentStatus.Percent = 1;
            return true;
        }

        private async Task<string> FindSimitoneReleaseUrl()
        {
            var github = new GitHubClient(new ProductHeaderValue(Properties.Constants.Default.GitRepoName));                        
            var latestRelease = await github.Repository.Release.GetLatest(Properties.Constants.Default.GitUser, Properties.Constants.Default.GitRepoName);
            return latestRelease.Assets.FirstOrDefault().BrowserDownloadUrl;
        }
    }
}
