using System;
using System.Collections.Generic;
using System.Linq;
using Aijkl.VR.MicController.Commands;
using Aijkl.VRMicController.Commands;
using NAudio.CoreAudioApi;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Aijkl.VR.MicController
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                CommandApp commandApp = new CommandApp();
                commandApp.Configure(x =>
                {
                    x.AddCommand<ChangeSettingsCommand>("settings");
                    x.AddCommand<RegisterCommand>("register");
                    x.AddCommand<DeRegisterCommand>("deregister");
                    x.AddCommand<RunCommand>("run");
                });
                commandApp.Run(args);
            }
            catch (Exception exception)
            {
                AnsiConsole.WriteException(exception);
                Console.ReadLine();
            }
        }
    }
}
