using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LedStripController
{
    public partial class RgbColorSlidersPage : ContentPage
    {
        TcpClient tcpClient;
        NetworkStream clientStream;
        StreamWriter streamWriter;
        int red;
        int blue;
        int green;

        public RgbColorSlidersPage()
        {
            InitializeComponent();
        }

        void OnSliderValueChanged(object sender, ValueChangedEventArgs args)
        {
            if (sender == redSlider)
            {
                red = (int)args.NewValue;
                redLabel.Text = String.Format("Red = {0:X2}", red);
                
            }
            else if (sender == greenSlider)
            {
                green = (int)args.NewValue;
                greenLabel.Text = String.Format("Green = {0:X2}", green);
            }
            else if (sender == blueSlider)
            {
                blue = (int)args.NewValue;
                blueLabel.Text = String.Format("Blue = {0:X2}", blue);
            }

            boxView.Color = Color.FromRgb((int)redSlider.Value,
                                          (int)greenSlider.Value,
                                          (int)blueSlider.Value);
        }

        void OnButtonClicked(object sender, EventArgs args)
        {
            tcpClient = new TcpClient();
            tcpClient.Connect("192.168.0.143", 9999);
            clientStream = tcpClient.GetStream();
            streamWriter = new StreamWriter(clientStream);
            streamWriter.Write($"rgb " + red + " " + green + " " + blue);
            streamWriter.Flush();
            tcpClient.Close();
        }
    }
}
