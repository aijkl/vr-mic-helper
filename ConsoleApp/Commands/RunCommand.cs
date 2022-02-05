using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Aijkl.VR.MicController;
using Aijkl.VR.MicController.Helpers;
using Aijkl.VR.MicController.OpenVRWrapper;
using NAudio.CoreAudioApi;
using Spectre.Console;
using Spectre.Console.Cli;
using Valve.VR;

namespace Aijkl.VRMicController.Commands
{
    [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
    internal class RunCommand : Command
    {
        public override int Execute(CommandContext context)
        {


            AppSettings appSettings;
            try
            {
                appSettings = AppSettings.LoadFromFile();
                Table settingsTable = new Table();
                settingsTable.AddColumn("PropertyName");
                settingsTable.AddColumn("Value");
                foreach (var item in appSettings.GetType().GetProperties())
                {
                    if (item.PropertyType == typeof(string))
                    {
                        settingsTable.AddRow(item.Name, (string)item.GetValue(appSettings));
                    }
                }
                AnsiConsole.Render(settingsTable);
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(new Exception(LanguageDataSet.CONFIGURE_FILE_ERROR, ex));
                return 1;
            }

            CVRSystemWrapper cvrSystemWrapper = null;
            try
            {
                AnsiConsole.Status().Start(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.OpenVRInitializing)), action =>
                {
                    cvrSystemWrapper = new CVRSystemWrapper();
                });
            }
            catch (Exception)
            {
                AnsiConsole.WriteLine(appSettings.LanguageDataSet.GetValue(nameof(LanguageDataSet.OpenVRInitError)));
                throw;
            }

            using MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            using MMDevice mmDevice = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).First(x => x.ID == appSettings.TargetMicId);

            List<uint> pushingControllerIndex = new List<uint>(capacity: 2);
            cvrSystemWrapper.CVREvent += (sender, args) =>
            {
                foreach (var vrEvent in args.VREvents)
                {
                    AnsiConsole.WriteLine(((EVREventType)vrEvent.eventType).ToString());
                    switch ((EVREventType)vrEvent.eventType)
                    {
                        case EVREventType.VREvent_ButtonPress:
                            AnsiConsole.WriteLine($"ButtonId:{vrEvent.data.controller.button}");
                            if (appSettings.ButtonId == vrEvent.data.controller.button)
                            {
                                if (!pushingControllerIndex.Contains(vrEvent.trackedDeviceIndex))
                                {
                                    pushingControllerIndex.Add(vrEvent.trackedDeviceIndex);
                                }
                                if (pushingControllerIndex.Count >= 2)
                                {
                                    mmDevice.AudioEndpointVolume.Mute = true;
                                    AnsiConsoleHelper.WrapMarkupLine("Mute",AnsiConsoleHelper.State.Success);
                                }
                            }
                            break;
                        case EVREventType.VREvent_ButtonUnpress:
                            mmDevice.AudioEndpointVolume.Mute = false;
                            AnsiConsoleHelper.WrapMarkupLine("UnMute", AnsiConsoleHelper.State.Success);
                            if (pushingControllerIndex.Contains(vrEvent.trackedDeviceIndex))
                            {
                                pushingControllerIndex.Remove(vrEvent.trackedDeviceIndex);
                            }
                            break;
                        case EVREventType.VREvent_Quit:
                            cvrSystemWrapper.Dispose();
                            break;
                    }
                }
            };

            cvrSystemWrapper.BeginEventLoop();

            Console.ReadLine();
            return 0;
        }
    }
}
