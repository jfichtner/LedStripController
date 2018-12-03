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
            this.SendRgb(this.red, this.blue, this.green);
        }

        private void OnSliderValueChanged(object sender, ValueChangedEventArgs args)
        {
            if (sender == this.redSlider)
            {
                this.red = (int)args.NewValue;
                this.redLabel.Text = String.Format("Red = {0:X2}", this.red);
            }
            else if (sender == this.greenSlider)
            {
                this.green = (int)args.NewValue;
                this.greenLabel.Text = String.Format("Green = {0:X2}", this.green);
            }
            else if (sender == this.blueSlider)
            {
                this.blue = (int)args.NewValue;
                this.blueLabel.Text = String.Format("Blue = {0:X2}", this.blue);
            }

            this.SendRgb(this.red, this.green, this.blue);

            this.boxView.Color = Color.FromRgb(this.red,
                                          this.green,
                                          this.blue);
        }

        private void SendRgb(int r, int g, int b)
        {
            if (!this.delay)
            {
                this.streamWriter.Write("rgb " + r + " " + g + " " + b);
            }

            this.delay = true;
            this.delayTimer.Start();
        }

        private void OnButtonClicked(object sender, EventArgs args)
        {
            //this.SendRgb();
            var button = new Button{ BackgroundColor = Color.FromRgb(this.red, this.green, this.blue)};
            button.Clicked += this.PresetButtonClicked;
            this.SavedColorsLayout.Children.Add(button);
        }

        private void PresetButtonClicked(object sender, EventArgs e)
        {
            var button = (Button) sender;
            this.SendRgb(
                (int)button.BackgroundColor.R * 255, 
                (int)button.BackgroundColor.G * 255, 
                (int)button.BackgroundColor.B * 255);
        }
    }
}
