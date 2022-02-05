using System;
using System.IO;
using Spectre.Console;
using Spectre.Console.Cli;
using Valve.VR;
using Aijkl.VR.MicController.Helpers;
using Aijkl.VR.MicController.OpenVRWrapper;

namespace Aijkl.VR.MicController.Commands
{
    public class DeRegisterCommand : Command
    {
        public override int Execute(CommandContext context)
        {
            AppSettings appSettings = AppSettings.LoadFromFile();

            try
            {
                CVRSystemWrapper cvrSystemhelper = new CVRSystemWrapper(EVRApplicationType.VRApplication_Utility);
                EVRApplicationError vrApplicationError = cvrSystemhelper.CVRApplications.RemoveApplicationManifest(Path.GetFullPath(appSettings.ApplicationManifestPath));
                AnsiConsoleHelper.WrapMarkupLine(appSettings.LanguageDataSet.GetValue(vrApplicationError == EVRApplicationError.None ? nameof(LanguageDataSet.StreamVRRemoveManifestSuccess) : nameof(LanguageDataSet.StreamVRRemoveManifestFailure)), vrApplicationError == EVRApplicationError.None ? AnsiConsoleHelper.State.Success : AnsiConsoleHelper.State.Failure);
            }
            catch (Exception ex)
            {
                AnsiConsoleHelper.WrapMarkupLine(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.StreamVRRemoveManifestFailure)), AnsiConsoleHelper.State.Failure);
                AnsiConsole.WriteException(ex);
                return 1;
            }
            return 0;
        }
    }
}
