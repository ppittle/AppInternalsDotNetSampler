﻿using System;
using System.Net;
using AppInternalsDotNetSampler.Core;
using AppInternalsDotNetSampler.Core.Console;

namespace AppInternalsDotNetSampler.TcpServer
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Write Preamble
            Console.WriteLine("");
            Console.WriteLine(HeaderPrinter.Stars);
            Console.WriteLine(HeaderPrinter.HeaderPrint("WELCOME TO THE APP INTERNALS .NET SAMPLER", HeaderPrinter.Style.CenterCaps));
            Console.WriteLine(HeaderPrinter.HeaderPrint("TCP SERVER", HeaderPrinter.Style.CenterCaps));
            Console.WriteLine(HeaderPrinter.HeaderPrint("", HeaderPrinter.Style.Left));
            Console.WriteLine(HeaderPrinter.HeaderPrint(" Source Code Available At:", HeaderPrinter.Style.Left));
            Console.WriteLine(HeaderPrinter.HeaderPrint(" https://github.com/ppittle/AppInternalsDotNetSampler", HeaderPrinter.Style.Left));
            Console.WriteLine(HeaderPrinter.HeaderPrint("", HeaderPrinter.Style.Left));
            System.Console.WriteLine(HeaderPrinter.HeaderPrint(" -- Philip Pittle philip.pittle@gmail.com", HeaderPrinter.Style.Left));
            Console.WriteLine(HeaderPrinter.Stars);

            Console.WriteLine();
            Console.WriteLine();
            #endregion

            #region Write Usage

            Console.WriteLine("Usage: " +
                AppDomain.CurrentDomain.FriendlyName + 
                " [ip address] [port] [simulated packet delay in milliseconds]");
            Console.WriteLine();
            #endregion

            var address = ParseCommandArgument<IPAddress>(
                args, 0, "IP Address", IPAddress.Parse("127.0.0.1"), s => IPAddress.Parse(s));

            var port = ParseCommandArgument<int>(
                args, 1, "Port", 8080, s => int.Parse(s));

            var simulatedPacketDelayInMilliseconds = ParseCommandArgument<int>(
                args, 2, "Simulated Packet Delay In Milliseconds", 0, s => int.Parse(s));

            Console.WriteLine();
            Console.WriteLine();

            try
            {
                using (var server = new EchoTcpServer(
                    new TcpServerLogger(), address, port, simulatedPacketDelayInMilliseconds))
                {
                    Console.WriteLine("");
                    Console.WriteLine("Starting TCP Server");
                    Console.WriteLine("");
                    Console.WriteLine("Press Ctrl+C to exit.");
                    Console.WriteLine("");

                    server.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Fatal Error.  Server will now shutdown: " +
                    e.Message + Environment.NewLine + e.StackTrace);   
            }
        }

        private static T ParseCommandArgument<T>(string[] args, int index, string name, T @default, Func<string, T> parse)
        {
            if (args.Length > index)
            {
                try
                {
                    var result = parse(args[index]);

                    Console.WriteLine("{0}: {1}",
                        name, result);

                    return result;
                }
                catch (Exception e)
                {
                    Console.WriteLine(
                        "WARNING: Failed to parse {0}.  {1} is not valid: {2}.  Will use default.",
                        name, args[index], e.Message);
                }
            }
            
            Console.WriteLine(
                "{0}: {1} (default)",
                name, @default);

            return @default;
        }
    }
}
