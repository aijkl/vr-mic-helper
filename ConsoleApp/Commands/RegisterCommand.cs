using System;
using System.IO;
using Aijkl.VR.MicController.Helpers;
using Aijkl.VR.MicController.OpenVRWrapper;
using Spectre.Console;
using Spectre.Console.Cli;
using Valve.VR;

namespace Aijkl.VR.MicController.Commands
{
    public class RegisterCommand : Command
    {
        public override int Execute(CommandContext context)
        {
            AppSettings appSettings = AppSettings.LoadFromFile();

            try
            {
                CVRSystemWrapper cvrSystemHelper = new CVRSystemWrapper(EVRApplicationType.VRApplication_Utility);
                EVRApplicationError vrApplicationError = cvrSystemHelper.CVRApplications.AddApplicationManifest(Path.GetFullPath(appSettings.ApplicationManifestPath), false);
                AnsiConsoleHelper.WrapMarkupLine($"{(appSettings.LanguageDataSet.GetValue(vrApplicationError == EVRApplicationError.None ? nameof(LanguageDataSet.StreamVRAddManifestSuccess) : nameof(LanguageDataSet.StreamVRAddManifestFailure)), vrApplicationError == EVRApplicationError.None ? AnsiConsoleHelper.State.Success : AnsiConsoleHelper.State.Failure)}", vrApplicationError == EVRApplicationError.None ? AnsiConsoleHelper.State.Success : AnsiConsoleHelper.State.Failure);
                if (vrApplicationError != (int)EVREventType.VREvent_None)
                {
                    AnsiConsoleHelper.WrapMarkupLine(vrApplicationError.ToString(), AnsiConsoleHelper.State.Failure);
                }
            }
            catch (Exception ex)
            {
                AnsiConsoleHelper.WrapMarkupLine(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.StreamVRAddManifestFailure)), AnsiConsoleHelper.State.Failure);
                AnsiConsole.WriteException(ex);
                return 1;
            }
            return 0;
        }
    }
}
