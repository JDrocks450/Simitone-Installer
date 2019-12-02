using Microsoft.Owin.FileSystems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sayonara
{
    public class NetFileSystemContext
    {
        public string directory = "";
        public PhysicalFileSystem fileSystem;
    }
    public static class NetFileSystem
    {
        const int DirNotFoundErr = 101;
        const string TheSimsDir = @"D:\Games\The Sims Complete Collection";
        public static NetFileSystemContext Context
        {
            get; private set;
        } = new NetFileSystemContext();
        public static PhysicalFileSystem fileSystem
        {
            get => GetFileSystem();
            private set => Context.fileSystem = value;
        }
        
        public static PhysicalFileSystem GetFileSystem()
        {
            if (Context.fileSystem == null)
                fileSystem = new PhysicalFileSystem(TheSimsDir);
            return Context.fileSystem;
        }

        public static string GetCurrentDirectory()
        {
            return Context.directory;
        }

        public static string NavigateToDirectoryInCurrent(string name, out bool result)
        {
            result = false;
            if (GetDirectoriesInCurrentDirectory().Contains(name))
            {
                result = true;
                return Context.directory = Context.directory + "/" + name;
            }
            else
                return "Directory doesn't exist in the current directory.";
        }
        public static bool JumpToDirectory(string subpath)
        {
            
            if (subpath != null)
                if (fileSystem.TryGetDirectoryContents(subpath, out _))
                {
                    Context.directory = subpath;
                    Out.PrintLine("The current directory is now: root/" + Context.directory);
                    return true;
                }
            Out.PrintLine("Directory doesn't exist in the filesystem.");
            return false;
        }
        public static string[] GetDirectoriesInCurrentDirectory()
        {
            fileSystem.TryGetDirectoryContents(GetCurrentDirectory(), out var contents);
            return contents.Where(x => x.IsDirectory).Select(x => x.Name).ToArray();
        }

        public static string[] GetFilesInDirectory()
        {
            fileSystem.TryGetDirectoryContents(GetCurrentDirectory(), out var contents);
            return contents.Where(x => !x.IsDirectory).Select(x => x.Name).ToArray();
        }
        public static IFileInfo GetFile(string name)
        {            
            var files = GetFilesInDirectory();           
            if (files.Contains(name))
            {
                if (fileSystem.TryGetFileInfo(Path.Combine(GetCurrentDirectory(), name), out var info))
                    return info;
            }
            return null;
        }
        public static IFileInfo GetFileBySubpath(string subpath)
        {
            if (fileSystem.TryGetFileInfo(subpath, out var info))
                return info;
            return null;
        }
    }
}
