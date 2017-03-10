using System;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Networking;
using Windows.UI.Core;
using Windows.Storage.Streams;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Devices.Gpio;
using Windows.Devices.Pwm;
using Microsoft.IoT.DeviceCore.Pwm;
using Microsoft.IoT.Devices.Pwm;

namespace Project.Core
{
    public class Class1
    {
        private DatagramSocket sender;
        private DataWriter dataWriter;
        private GpioController gpioController;
        private PwmController pwmController;
        private PwmPin pwmPin;

        public static async Task showProgressScreen(VoiceCommandServiceConnection voiceServiceConnection, string message)
        {
            VoiceCommandUserMessage userProgressMessage = new VoiceCommandUserMessage();
            userProgressMessage.DisplayMessage = userProgressMessage.SpokenMessage = message;
            VoiceCommandResponse response = VoiceCommandResponse.CreateResponse(userProgressMessage);
            await voiceServiceConnection.ReportProgressAsync(response);
            return;//
        }

        public async void SendMessages(string message)
        {
            if (sender == null)
            {
                HostName hostname = new HostName("192.168.1.1");
                sender = new DatagramSocket();
                await sender.ConnectAsync(hostname, "2213");
            }
            if (dataWriter == null)
                dataWriter = new DataWriter(sender.OutputStream);
            dataWriter.WriteString(message);
            await dataWriter.StoreAsync();
        }

        public bool determin()
        {
            Boolean isIoTorNot = Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Devices.Gpio");
            return isIoTorNot;
        }

        public async void InitPWM()
        {
            gpioController = GpioController.GetDefault();
            var pwmProviderManager = new PwmProviderManager();
            pwmProviderManager.Providers.Add(new SoftPwm());
            var pwmControllers = await pwmProviderManager.GetControllersAsync();
            pwmController = pwmControllers[0];
            pwmPin = pwmController.OpenPin(2);
            pwmController.SetDesiredFrequency(1000);
            pwmPin.Start();
        }

        public string GetPwmInfo()
        {
            string text = "当前亮度为： " + (GetBrightness() * 100).ToString() + "%";
            return text;
        }

        public async void SetActiveDutyCyclePercentage(double brightness)
        {
           pwmPin.SetActiveDutyCyclePercentage(brightness);
            
        }

        public double GetBrightness()
        {
            double brightness = pwmPin.GetActiveDutyCyclePercentage();
            return brightness;
        }

        public async void ReportSuccess(VoiceCommandServiceConnection voiceCommandServiceConnection)
        {
            VoiceCommandUserMessage userMessage = new VoiceCommandUserMessage();
            userMessage.SpokenMessage = userMessage.DisplayMessage = "已成功";
            VoiceCommandResponse response = VoiceCommandResponse.CreateResponse(userMessage);
            await voiceCommandServiceConnection.ReportSuccessAsync(response);
        }
    }
}
