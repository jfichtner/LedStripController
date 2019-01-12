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
        private LedStrip strip = LedStrip.Both;

        private enum LedStrip { Ceiling, Desk, Both }


        public RgbColorSlidersPage()
        {
            this.InitializeComponent();
            this.tcpClient.Connect("192.168.0.143", 9999);
            this.streamWriter = new StreamWriter(this.tcpClient.GetStream());
            this.streamWriter.AutoFlush = true;
            this.delayTimer.Elapsed += this.DelayTimer_Elapsed;
            this.delayTimer.AutoReset = false;
            this.Both.FontAttributes = FontAttributes.Bold;
        }

        private void DelayTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.delay = false;
            this.SendRgb(this.red, this.green, this.blue);
            this.delayTimer.Stop();
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
                string stripVal = string.Empty;

                if (strip == LedStrip.Ceiling)
                {
                    stripVal = " 0";
                }

                if(strip == LedStrip.Desk)
                {
                    stripVal = " 1";
                }

                this.streamWriter.Write("rgb " + r + " " + g + " " + b + stripVal);
            }

            this.delay = true;
            this.delayTimer.Start();
        }

        private void OnSaveColorButtonClicked(object sender, EventArgs args)
        {
            var button = new Button{ BackgroundColor = Color.FromRgb(this.red, this.green, this.blue)};
            button.Clicked += this.PresetButtonClicked;
            this.SavedColorsLayout.Children.Add(button);
        }

        private void OnCeilingButtonClicked(object sender, EventArgs args)
        {
            strip = LedStrip.Ceiling;
            this.Ceiling.FontAttributes = FontAttributes.Bold;
            this.Both.FontAttributes = FontAttributes.None;
            this.Desk.FontAttributes = FontAttributes.None;
        }

        private void OnBothButtonClicked(object sender, EventArgs args)
        {
            strip = LedStrip.Both;
            this.Ceiling.FontAttributes = FontAttributes.None;
            this.Both.FontAttributes = FontAttributes.Bold;
            this.Desk.FontAttributes = FontAttributes.None;
        }

        private void OnDeskButtonClicked(object sender, EventArgs args)
        {
            strip = LedStrip.Desk;
            this.Ceiling.FontAttributes = FontAttributes.None;
            this.Both.FontAttributes = FontAttributes.None;
            this.Desk.FontAttributes = FontAttributes.Bold;
        }


        private void PresetButtonClicked(object sender, EventArgs e)
        {
            var button = (Button) sender;
            this.redSlider.Value = (int)(button.BackgroundColor.R * 255);
            this.greenSlider.Value = (int)(button.BackgroundColor.G * 255);
            this.blueSlider.Value = (int)(button.BackgroundColor.B * 255);

            //this.SendRgb(
            //    (int)(button.BackgroundColor.R * 255), 
            //    (int)(button.BackgroundColor.G * 255), 
            //    (int)(button.BackgroundColor.B * 255));
        }
    }
}
