using System.Collections.Generic;
using System.Linq;
using Aijkl.VR.MicController;
using Aijkl.VR.MicController.Helpers;
using NAudio.CoreAudioApi;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Aijkl.VRMicController.Commands
{
    internal class ChangeSettingsCommand : Command
    {
        public override int Execute(CommandContext context)
        {
            AppSettings appSettings = AppSettings.LoadFromFile();

            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            IEnumerable<MMDevice> captureDevices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).ToArray();
            if (!captureDevices.Any())
            {
                AnsiConsoleHelper.WrapMarkupLine(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.SelectMicNotFoundMic)),AnsiConsoleHelper.State.Failure);
            }

            MMDevice device = AnsiConsole.Prompt(new SelectionPrompt<MMDevice>()
            .Title(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.SelectMicMessage)))
            .UseConverter(x => x.DeviceFriendlyName)
            .AddChoices(captureDevices));
            appSettings.TargetMicId = device.ID;
            appSettings.SaveToFile();
            AnsiConsoleHelper.WrapMarkupLine(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.SelectMicSuccess)), AnsiConsoleHelper.State.Success);

            return 0;
        }
    }
}
