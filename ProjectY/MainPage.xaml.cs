using System;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Project.Core;
using Windows.UI.Core;

namespace ProjectY
{

    public sealed partial class MainPage : Page
    {
        private DatagramSocket listener;
        Class1 helper = new Class1();
        public MainPage()
        {
            this.InitializeComponent();
            if (helper.determin())
            {
                helper.InitPWM();
                DatagramSocketConnect();
            }
        }

        private async void DatagramSocketConnect()
        {
            listener = new DatagramSocket();
            listener.MessageReceived += Listener_MessageReceived;
            await listener.BindServiceNameAsync("2213");
        }

        private void Listener_MessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            DataReader reader = args.GetDataReader();
            uint stringLength = reader.UnconsumedBufferLength;
            double brightness = helper.GetBrightness();
            var ignore = this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var result = reader.ReadString(stringLength);
                if (result!="On" && result!="Off" && result!="Brighter" && result!="Darker" )
                {
                    double sliderValue = double.Parse(result);
                    helper.SetActiveDutyCyclePercentage(sliderValue);
                    test.Text = helper.GetPwmInfo();
                }
                switch (result)
                {
                    case "On":
                        helper.SetActiveDutyCyclePercentage(1);
                        test.Text = helper.GetPwmInfo();
                        break;
                    case "Off":
                        helper.SetActiveDutyCyclePercentage(0);
                        test.Text = helper.GetPwmInfo();
                        break;
                    case "Darker":
                        if (brightness != 0 && brightness - 0.1 >= 0)
                        {
                            helper.SetActiveDutyCyclePercentage(brightness - 0.1);
                            test.Text = helper.GetPwmInfo();
                        }
                        break;
                    case "Brighter":
                        if (brightness != 1 && brightness + 0.1 <= 1)
                        {
                            helper.SetActiveDutyCyclePercentage(brightness + 0.1);
                            test.Text = helper.GetPwmInfo();
                        }
                        break;
                    default:
                        break;
                }
            });
        }

        private void slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (helper.determin())
            {
                helper.SetActiveDutyCyclePercentage(e.NewValue/100);
                test.Text = helper.GetPwmInfo();
            }
            else
            {
                helper.SendMessages(((e.NewValue)/100).ToString());
                test.Text = "当前亮度为: " + e.NewValue.ToString() + "%";
            }
        }

        private void On_Click(object sender, RoutedEventArgs e)
        {
            if (helper.determin())
            {
                helper.SetActiveDutyCyclePercentage(1);
                test.Text = helper.GetPwmInfo();
            }
            else
            {
                helper.SendMessages("On");
                test.Text = "当前亮度为: 100%";
            }
        }

        private void OFF_Click(object sender, RoutedEventArgs e)
        {
            if (helper.determin())
            {
                helper.SetActiveDutyCyclePercentage(0);
                test.Text = helper.GetPwmInfo();
            }
            else
            {
                helper.SendMessages("Off");
                test.Text = "当前亮度为: 0%";
            }
        }
    }
}
