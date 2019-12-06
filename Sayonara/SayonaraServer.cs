﻿using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Owin;
using Microsoft.Owin.StaticFiles;
using System.Net.Sockets;
using System.Net;

namespace Sayonara
{
    public class SayonaraServer
    {
        public const int PORT = 37575;
        IDisposable Server;
        BeaconLib.Beacon serverBeacon;
        public string Address
        {
            get; private set;
        }
        public bool Hosting
        {
            get;
            private set;
        } = false;
        public SayonaraServer(string IP = "localhost", int port = PORT)
        {
            var address = @"http://" + IP + ":" + port + "/";
            try
            {
                Server = WebApp.Start<FileServerBuilder>(address);
            }
            catch (Exception e)
            {
                Out.PrintLine(e.ToString());
                Out.ShowDialog("The Server could not be started! Please try again in a little while.");
                return;
            }
            Address = address;
            Hosting = true;
            Out.PrintLine("Sayonara is hosting at address: " + address);
            SetupDiscovery();
        }
        /// <summary>
        /// Gets a Server instance on the local IP address
        /// </summary>
        /// <returns></returns>
        public static SayonaraServer HostLocalServer()
        {
            var ip = GetLocalIP();
            if (ip == null)
                return null;
            return new SayonaraServer(ip);
        }

        public static string GetLocalIP()
        {
            string localIP = null;
            try
            {
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    socket.Connect("8.8.8.8", 65530);
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    localIP = endPoint.Address.ToString();
                }
            }
            catch (Exception)
            {
                return null; // not connected to the internet
            }
            return localIP;
        }

        private void SetupDiscovery()
        {
            if (serverBeacon == null)
            {
                serverBeacon = new BeaconLib.Beacon("Sayonara", PORT);
                serverBeacon.BeaconData = "Transfer from PC: " + Environment.MachineName;
                serverBeacon.Start(); // beacon Stopped is not false after creation
                return;
            }
            if (serverBeacon.Stopped)
                serverBeacon.Start();
        }

        private void StopDiscovery() // this should be called when the download starts, not when a user connects
        {
            serverBeacon?.Stop();
        }
        
        public void Shutdown()
        {
            Out.PrintLine("Sayonara is no longer hosting...");
            Server?.Dispose();
            StopDiscovery();
            Hosting = false;
        }
    }
}