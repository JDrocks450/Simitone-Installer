using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sayonara
{
    internal class FileQueue : Queue<(string subpath, bool isDir, byte[] data)>, IDisposable
    {
        /// <summary>
        /// When a directory is requested for creation, this is invoked to handle the file or directory creation.
        /// </summary>        
        public bool IsListening
        {
            get; set;
        }
        public string RootDirectory
        {
            get; set;
        }
        public void StartListening(string rootDirectory)
        {
            RootDirectory = rootDirectory;
            IsListening = true;
            while (IsListening)
            {
                if (Count > 0)
                {
                    var file = Dequeue();
                    if (file.isDir)
                        CreateDirectory(file.subpath);
                    else
                        Task.Run(() => WriteFile(file.subpath, file.data)).
                            ContinueWith((Task<bool> task) =>
                                {
                                    if (!task.Result)
                                    {
                                        IsListening = false;
                                        throw new Exception("A file exception occured");
                                    }
                              });
                }
            }
        }
        private void CreateDirectory(string subpath)
        {
            var path = System.IO.Path.Combine(RootDirectory, subpath);
            Directory.CreateDirectory(path);
            Out.PrintLine($"DIRECTORY: {subpath} was created.");
        }
        private bool WriteFile(string subpath, byte[] data)
        {
            var path = System.IO.Path.Combine(RootDirectory, subpath);
            using (var filehandle = File.Create(path))
            {
                filehandle.Write(data, 0, data.Length);
            }
            Out.PrintLine($"FILE: {subpath} was created.");
            return true;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    IsListening = false;
                    foreach (var file in this)
                    {
                        if (file.isDir)
                            CreateDirectory(file.subpath);
                        else
                            WriteFile(file.subpath, file.data);
                    }
                }
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
