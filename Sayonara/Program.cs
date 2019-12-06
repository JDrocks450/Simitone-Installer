using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sayonara
{
    /// <summary>
    /// The output to the UI
    /// </summary>
    public class Out
    {
        public static Out DefaultOut = new Out();
        public enum MODE
        {
            WRITELINE,
            DIALOG,
        }
        public enum DIALOGBTNS
        {
            OK,
            YESNO
        }
        public class OnOutputEventArgs
        {
            public string Text;
            public DIALOGBTNS Buttons;
        }
        public delegate void OnOutputEventHandler(MODE outMode, OnOutputEventArgs args);
        public event OnOutputEventHandler OnOutput;

        public void WriteLine(string text)
        {
            OnOutput?.Invoke(MODE.WRITELINE, new OnOutputEventArgs() { Text = text });
        }

        public void Dialog(string text, DIALOGBTNS btns = DIALOGBTNS.OK)
        {
            OnOutput?.Invoke(MODE.DIALOG, new OnOutputEventArgs() { Text = text, Buttons = btns });
        }

        /// <summary>
        /// Calls WriteLine on DefaultOut
        /// </summary>
        /// <param name="text"></param>
        public static void PrintLine(string text)
        {
            DefaultOut.WriteLine(text);
        }

        /// <summary>
        /// Calls Dialog on DefaultOut
        /// </summary>
        /// <param name="text"></param>
        /// <param name="btns"></param>
        public static void ShowDialog(string text, DIALOGBTNS btns = DIALOGBTNS.OK)
        {
            DefaultOut.Dialog(text, btns);
        }
    }
    class Program
    {        
        static async Task Main(string[] args)
        {
            Out.DefaultOut.OnOutput += DefaultOut_OnOutput;
            Console.WriteLine("Sayonara Console");
            var fileSystem = new SayonaraServer();
            var client = new SayonaraClient("localhost");
            Console.WriteLine("The Server is running...");
            var command = "";
            while (true)
            {
                command = Console.ReadLine();
                string firstKeyword = "", secondKeyword = "", thirdKeyword = "";
                try
                {
                    firstKeyword = command.Substring(0, command.IndexOf(' '));
                    secondKeyword = command.Substring(command.IndexOf(' ') + 1);
                    if (secondKeyword.Contains(' '))
                    {
                        thirdKeyword = secondKeyword.Substring(secondKeyword.IndexOf(' ') + 1);
                        secondKeyword = secondKeyword.Substring(0, secondKeyword.IndexOf(' '));
                    }
                }
                catch(Exception e)
                {

                }
                if (firstKeyword == "quit")
                    break;
                HttpResponseMessage msg = null;
                switch (firstKeyword)
                {
                    case "get":
                        msg = await client.Client.GetAsync(fileSystem.Address + "api/" + secondKeyword + (thirdKeyword != "" ? "/" : "") + thirdKeyword);
                        break;
                    case "put":
                        //msg = client.RequestDirectory(secondKeyword);
                        break;
                    default:
                        Console.WriteLine("Not recognized");
                        break;
                }
                Console.WriteLine(msg);
                Console.WriteLine(msg.Content.ReadAsStringAsync().Result);
            }            
            fileSystem.Shutdown();
        }

        private static void DefaultOut_OnOutput(Out.MODE outMode, Out.OnOutputEventArgs args)
        {
            Console.WriteLine(args.Text);
            if (outMode == Out.MODE.DIALOG)
            {
                Console.WriteLine("??? (Y/N)");
                _ = Console.ReadKey();
            }
        }
    }    
}
