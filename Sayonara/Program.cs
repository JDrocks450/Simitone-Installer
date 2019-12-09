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
            YESNO,
            YESNOCANCEL,
        }        
        public class OnOutputEventArgs
        {
            public string Text;
            public DIALOGBTNS Buttons;
            public enum RESULT
            {
                NO_RESULT, OK, YES, NO, CANCEL
            }
            public RESULT Result;
        }
        public delegate void OnOutputEventHandler(MODE outMode, OnOutputEventArgs args);
        public event OnOutputEventHandler OnOutput;

        public void WriteLine(string text)
        {
            OnOutput?.Invoke(MODE.WRITELINE, new OnOutputEventArgs() { Text = text });
        }

        public OnOutputEventArgs Dialog(string text, DIALOGBTNS btns = DIALOGBTNS.OK)
        {
            var e = new OnOutputEventArgs() { Text = text, Buttons = btns };
            OnOutput?.Invoke(MODE.DIALOG, e);
            return e;
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
        public static OnOutputEventArgs ShowDialog(string text, DIALOGBTNS btns = DIALOGBTNS.OK)
        {
            return DefaultOut.Dialog(text, btns);
        }
    }
    class Program
    {        
        static void PrintSeparator()
        {
            Console.WriteLine("===================================");
        }
        static async Task Main(string[] args)
        {
            Out.DefaultOut.OnOutput += DefaultOut_OnOutput;
            Console.WriteLine("Sayonara Console by bISQUICK (JDrocks450)");
            PrintSeparator();
            string installpath = null;
            while (true)
            {
                Console.WriteLine("Where is The Sims installed?");
                installpath = Console.ReadLine();
                if (Out.ShowDialog(installpath + "... Is this OK", Out.DIALOGBTNS.YESNO).Result == Out.OnOutputEventArgs.RESULT.YES)
                    break;
            }
            PrintSeparator();
            var fileSystem = new SayonaraServer(installpath);
            var client = new SayonaraClient("localhost");
            Console.WriteLine("The Server is running...");
            PrintSeparator();
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
                PrintSeparator();
            }            
            fileSystem.Shutdown();
        }

        private static void DefaultOut_OnOutput(Out.MODE outMode, Out.OnOutputEventArgs args)
        {            
            if (outMode == Out.MODE.DIALOG)
            {
                switch (args.Buttons)
                {
                    case Out.DIALOGBTNS.YESNO:
                        while (true)
                        {
                            Console.WriteLine(args.Text + "? (Y/N)");
                            var response = Console.ReadKey();
                            if (char.ToLower(response.KeyChar) == 'y')
                            {
                                args.Result = Out.OnOutputEventArgs.RESULT.YES;
                                break;
                            }
                            else if (char.ToLower(response.KeyChar) == 'n')
                            {
                                args.Result = Out.OnOutputEventArgs.RESULT.NO;
                                break;
                            }
                            else
                                Console.WriteLine(response.KeyChar + " is not a valid response.");
                        }
                        return;
                    case Out.DIALOGBTNS.YESNOCANCEL:
                        {
                            Console.WriteLine(args.Text + "? (Y/N or Any Other Key to Cancel)");
                            var response = Console.ReadKey();
                            if (char.ToLower(response.KeyChar) == 'y')
                                args.Result = Out.OnOutputEventArgs.RESULT.YES;
                            else if (char.ToLower(response.KeyChar) == 'n')
                                args.Result = Out.OnOutputEventArgs.RESULT.NO;
                            else args.Result = Out.OnOutputEventArgs.RESULT.CANCEL;
                        }
                        return;
                    case Out.DIALOGBTNS.OK:
                        {
                            Console.WriteLine(args.Text);
                            Console.WriteLine("Press Any Key to Continue...");
                        }
                        return;
                }                
                
            }
            else
                Console.WriteLine(args.Text);
        }
    }    
}
