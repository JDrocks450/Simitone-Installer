using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Sayonara.FileQueue;

namespace Sayonara
{
    public class SayonaraClient
    {
        public CancellationTokenSource CancellationToken
        {
            get
            {
                if (_source == null)
                {
                    CancellationToken = new CancellationTokenSource();
                    cancelToken = CancellationToken.Token;
                }
                return _source;
            }
            private set => _source = value;
        }
        private CancellationToken cancelToken;

        public HttpClient Client;
        private CancellationTokenSource _source;
        private FileQueue fileQueue;

        public string ConnectedURL
        {
            get;
            private set;
        }
        public bool ConnectionAllowed
        {
            get; private set;
        }        
        public bool IsPaused
        {
            get; set;
        }
        public bool IsDownloading
        {
            get; private set;
        }

        /// <summary>
        /// Creates a new client which is not started nor connecting to any URL.
        /// </summary>
        public SayonaraClient()
        {

        }

        /// <summary>
        /// Creates and starts a new client that will make requests to the specified URL.
        /// </summary>
        /// <param name="FullIPAddress">The address and port number of the server.</param>
        public SayonaraClient(string URL)
        {
            Task.Run(() => Connect(URL));
        }

        public async Task<bool> Connect(string IP)
        {
            Client = new HttpClient();
            var URL = @"http://" + IP + ":" + SayonaraServer.PORT;
            Out.PrintLine("Sayonara Client started looking for: " + URL);
            Client.BaseAddress = new Uri(URL);
            ConnectedURL = URL;
            ConnectionAllowed = await IsServerConnectable();
            if (ConnectionAllowed)
                Out.PrintLine("Connected to " + URL);
            else
                Out.PrintLine("Connection Unsuccessful! Is a SayonaraServer started at this url? " + URL);            
            return ConnectionAllowed;
        }

        public void DisposeCancelToken()
        {
            CancellationToken.Dispose();
            CancellationToken = null;
        }

        /// <summary>
        /// Start the transfer of the installation to the selected directory.
        /// </summary>
        /// <param name="rootDirectory">The root directory where the installation will be placed</param>
        /// <returns></returns>
        public async Task DownloadAsync(string rootDirectory)
        {
            IsDownloading = true;
            fileQueue = new FileQueue();
            _ = Task.Run(() => fileQueue.StartListening(rootDirectory));
            await RequestDirectory("");
            fileQueue.Dispose();
            fileQueue = null;
            IsDownloading = false;
        }

        /// <summary>
        /// Downloads a directory with all of it's child directories and files.
        /// </summary>
        /// <param name="subpath"></param>
        private async Task<bool> RequestDirectory(string subpath) // this powers the loop of downloading the game
        {
            cancelToken.ThrowIfCancellationRequested();
            subpath = subpath.TrimStart('/');
            var content = new StringContent(subpath, Encoding.UTF8, "application/json");
            if (Client.PostAsJsonAsync(ConnectedURL + "/api/directories", subpath).Result.IsSuccessStatusCode) // switch the directory back to one we're downloading
            {
                fileQueue.Enqueue((subpath, true, null)); // filequeue creates files on a separate thread
                var fileResponse = await Client.GetAsync(ConnectedURL + "/api/files"); // get a list of files in the dir
                var files = await fileResponse.Content.ReadAsAsync<IEnumerable<string>>();
                foreach (var file in files)
                {
                    cancelToken.ThrowIfCancellationRequested();
                    while (IsPaused)
                    {
                        cancelToken.ThrowIfCancellationRequested();
                        await Task.Delay(1000);
                    }
                    var downloadResponse = await Client.GetAsync(ConnectedURL + "/api/downloads/" + file);
                    var downloadData = await downloadResponse.Content.ReadAsStreamAsync();
                    _ = Task.Run(() =>
                    {
                        var bytes = new byte[downloadData.Length];
                        downloadData.ReadAsync(bytes, 0, bytes.Length);
                        downloadData.Dispose();
                        fileQueue.Enqueue((Path.Combine(subpath, file), false, bytes));
                    });
                }
                var dirResponse = Client.GetAsync(ConnectedURL + "/api/directories").Result; // get the directories in the current dir
                var directories = await dirResponse.Content.ReadAsAsync<IEnumerable<string>>();
                foreach (var dir in directories)
                {
                    cancelToken.ThrowIfCancellationRequested();
                    while (IsPaused)
                    {
                        cancelToken.ThrowIfCancellationRequested();
                        await Task.Delay(1000);
                    }
                    if (!await RequestDirectory(subpath + "/" + dir)) // download that directory
                        return false; // safe exit
                }
            }            
            return true;
        }        

        public async Task<bool> IsServerConnectable()
        {
            try
            {
                var response = await Client.GetAsync(ConnectedURL + "/api/connections");
                return response.IsSuccessStatusCode;
            }
            catch(Exception e)
            {
                Out.PrintLine(e.GetBaseException().Message);
                return false;
            }
        }
    }
}
