using Octokit;
using Simitone.Installer.Driver.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Simitone.Installer.Driver
{
    public class InstallationManager
    {
        public bool HasErrors => Context.HasErrors;
        public IInstallationMessage[] Warnings => Context.Warnings;
        public InstallationContext Context { get; private set; }
        public AsyncStatus CurrentStatus { get; private set; }

        public InstallationManager()
        {
            Context = InstallationContext.GetInstallationContext();
            CurrentStatus = new AsyncStatus() { Status = "Waiting..." };
        }

        private void SetStatus(string text)
        {
            CurrentStatus.Status = text;
        }

        public async Task<bool> BeginInstallation()
        {
            var dir = new DirectoryInfo(Context.SimitonePath);
            if (dir.Exists && (dir.GetFiles().Length > 0 || dir.GetDirectories().Length > 0)) // selected directory has files
                if (!Context.IgnoreSimitonePathHasFiles)
                {
                    Context.PushWarning("Directory Not Empty!", "The selected Simitone installation " +
                        "path (" + Context.SimitonePath + ") is not empty. Please clear the directory of all files " +
                        "or choose a new place to install Simitone.", MessageSeverity.High);
                    return false;
                }
                else
                {
                    try
                    {
                        dir.Delete(true);
                    }
                    catch (Exception)
                    {
                        Context.PushWarning("Couldn't Delete Directory!", "The selected Simitone installation " +
                           "path (" + Context.SimitonePath + ") is not empty, and it could not be cleared automatically. Try reopening " +
                           "the installer with Admin rights, or else clear the directory manually.", MessageSeverity.High);
                        return false;
                    }
                }
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
            SetStatus("Extracting Simitone to: " + Context.SimitonePath);
            ZipFile.ExtractToDirectory(path, Context.SimitonePath);
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
