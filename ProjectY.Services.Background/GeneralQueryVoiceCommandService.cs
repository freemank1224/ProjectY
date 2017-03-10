using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Project.Core;

namespace ProjectY.Services.Background
{
    public sealed class GeneralQueryVoiceCommandService : IBackgroundTask
    {
        VoiceCommandServiceConnection voiceCommandServiceConnection;
        BackgroundTaskDeferral serviceDeferral;
        Class1 helper = new Class1();

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            if (helper.determin())
            {
                helper.InitPWM();
            }
            serviceDeferral = taskInstance.GetDeferral();
            taskInstance.Canceled += TaskInstance_Canceled;
            var triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            if (triggerDetails!=null && triggerDetails.Name == "GeneralQueryVoiceCommandService")
            {
                try
                {
                    voiceCommandServiceConnection = VoiceCommandServiceConnection.FromAppServiceTriggerDetails(triggerDetails);
                    voiceCommandServiceConnection.VoiceCommandCompleted += VoiceCommandServiceConnection_VoiceCommandCompleted;
                    VoiceCommand voiceCommand = await voiceCommandServiceConnection.GetVoiceCommandAsync();
                    switch (voiceCommand.CommandName)
                    {
                        case "On":
                            await Class1.showProgressScreen(voiceCommandServiceConnection,"正在处理");
                            if (helper.determin())
                            {
                                helper.SetActiveDutyCyclePercentage(1);
                                helper.ReportSuccess(voiceCommandServiceConnection);
                            }
                            else
                            {
                                helper.SendMessages("On");
                                helper.ReportSuccess(voiceCommandServiceConnection);
                            }          
                            break;
                        case "Off":
                            await Class1.showProgressScreen(voiceCommandServiceConnection, "正在处理");
                            if (helper.determin())
                            {
                                helper.SetActiveDutyCyclePercentage(0);
                                helper.ReportSuccess(voiceCommandServiceConnection);
                            }
                            else
                            {
                                helper.SendMessages("Off");
                                helper.ReportSuccess(voiceCommandServiceConnection);
                            }
                            break;
                        case "Brighter":
                            await Class1.showProgressScreen(voiceCommandServiceConnection, "正在处理");
                            if (helper.determin())
                            {
                                double brightness = helper.GetBrightness();
                                if (brightness != 1 && brightness + 0.1 <= 1)
                                {
                                    helper.SetActiveDutyCyclePercentage(brightness + 0.1);
                                }
                                helper.ReportSuccess(voiceCommandServiceConnection);
                            }
                            else
                            {
                                helper.SendMessages("Brighter");
                                helper.ReportSuccess(voiceCommandServiceConnection);
                            }
                            break;
                        case "Darker":
                            await Class1.showProgressScreen(voiceCommandServiceConnection, "正在处理");
                            if (helper.determin())
                            {
                                double brightness = helper.GetBrightness();
                                if (brightness != 0 && brightness - 0.1 >= 0)
                                {
                                    helper.SetActiveDutyCyclePercentage(brightness - 0.1);
                                }
                                helper.ReportSuccess(voiceCommandServiceConnection);
                            }
                            else
                            {
                                helper.SendMessages("Darker");
                                helper.ReportSuccess(voiceCommandServiceConnection);
                            }
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        
        private void VoiceCommandServiceConnection_VoiceCommandCompleted(VoiceCommandServiceConnection sender, VoiceCommandCompletedEventArgs args)
        {
            if (this.serviceDeferral !=null)
            {
                this.serviceDeferral.Complete();
            }
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            if (this.serviceDeferral != null)
            {
                this.serviceDeferral.Complete();
            }
        }
    }
}
