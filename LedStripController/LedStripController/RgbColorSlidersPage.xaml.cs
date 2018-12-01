using System;
using System.IO;
using System.Net.Sockets;
using System.Timers;
using Xamarin.Forms;

namespace LedStripController
{
    public partial class RgbColorSlidersPage : ContentPage
    {
        private readonly TcpClient tcpClient = new TcpClient();
        private readonly StreamWriter streamWriter;
        private int red;
        private int blue;
        private int green;
        private readonly Timer delayTimer = new Timer(200);
        private bool delay;

        public RgbColorSlidersPage()
        {
            this.InitializeComponent();
            this.tcpClient.Connect("192.168.0.143", 9999);
            this.streamWriter = new StreamWriter(this.tcpClient.GetStream());
            this.streamWriter.AutoFlush = true;
            this.delayTimer.Elapsed += this.DelayTimer_Elapsed;
        }

        private void DelayTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.delay = false;
            this.SendRgb();
        }

        private void OnSliderValueChanged(object sender, ValueChangedEventArgs args)
        {
            if (sender == this.redSlider)
            {
                this.red = (int)args.NewValue;
                this.redLabel.Text = String.Format("Red = {0:X2}", this.red);
                this.SendRgb();
            }
            else if (sender == this.greenSlider)
            {
                this.green = (int)args.NewValue;
                this.greenLabel.Text = String.Format("Green = {0:X2}", this.green);
                this.SendRgb();
            }
            else if (sender == this.blueSlider)
            {
                this.blue = (int)args.NewValue;
                this.blueLabel.Text = String.Format("Blue = {0:X2}", this.blue);
                this.SendRgb();
            }

            this.boxView.Color = Color.FromRgb((int) this.redSlider.Value,
                                          (int) this.greenSlider.Value,
                                          (int) this.blueSlider.Value);
        }

        private void SendRgb()
        {
            if (!this.delay)
            {
                this.streamWriter.Write($"rgb " + this.red + " " + this.green + " " + this.blue);
            }

            this.delay = true;
            this.delayTimer.Start();
        }

        private void OnButtonClicked(object sender, EventArgs args)
        {
            this.SendRgb();
        }
    }
}
