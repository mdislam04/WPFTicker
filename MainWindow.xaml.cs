﻿using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ticker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        dynamic koinexPrice;
        dynamic binancePrice;
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        DispatcherTimer dispatcherTimerRefresh = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                GetBinancePrice();
                dispatcherTimer.Tick += DispatcherTimer_Tick;
                dispatcherTimer.Interval = new TimeSpan(0, 0, int.Parse(txtTimer.Text));
                dispatcherTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void getPrices()
        {
            try
            {
                //GetKoinexPrice();
                GetBinancePrice();
            }
            catch (Exception ex)
            {
                // lblBinance.Content = ex.Message;
            }
        }



        private void DispatcherTimer_Tick(object sender, System.EventArgs e)
        {
            GetBinancePrice();
        }

        private async void GetKoinexPrice()
        {
            using (var client = new HttpClient())
            {
                var url = "https://koinex.in/api/ticker";
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", "C# App");
                client.Timeout = TimeSpan.FromSeconds(60);
                var result = await client.GetStringAsync(url);
                koinexPrice = JsonConvert.DeserializeObject(result);
                setPrices();
            }
        }

        private async void GetBinancePrice()
        {
            using (var client = new HttpClient())
            {
                var url = "https://api.binance.com/api/v3/ticker/price";
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", "C# App");
                client.Timeout = TimeSpan.FromSeconds(60);
                var result = await client.GetStringAsync(url);
                binancePrice = JsonConvert.DeserializeObject(result);
                setPrices();
            }
        }

        private void MouseDown_Handler(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void TxtCoin_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            setPrices();
        }

        void setPrices()
        {
            if (binancePrice != null)
            {


                var coins = txtCoin.Text.Split(',');
                if (coins.Length > 0)
                {
                    stack.Children.Clear();
                    foreach (var item in coins)
                    {
                        Label lbl = new Label();
                        lbl.Name = item;
                        lbl.FontSize = 33;
                        lbl.FontWeight = FontWeights.ExtraBold;
                        lbl.Foreground = new SolidColorBrush(Colors.White);
                        lbl.Content = fetchBinancePricebyPara(item.ToUpper());
                        stack.Children.Add(lbl);

                        Label lblSep = new Label();
                        lblSep.Name = "sep";
                        lblSep.FontSize = 33;
                        lblSep.FontWeight = FontWeights.ExtraBold;


                        var bc = new BrushConverter();

                        lblSep.Foreground = (Brush)bc.ConvertFrom("#FF86640B");

                        lblSep.Content = "|";
                        stack.Children.Add(lblSep);
                    }
                    stack.Children.RemoveAt(stack.Children.Count - 1);
                }
            }
        }

        private string fetchBinancePrice(string coin = null)
        {
            string price = string.Empty;
            if (binancePrice != null)
            {
                foreach (var item in binancePrice)
                {
                    if (item.symbol == coin.ToUpper() + "USDT")
                    {
                        string s = item.price;
                        price = string.Format("{0:N5}", decimal.Parse(s));
                    }
                    else if (item.symbol == coin.ToUpper() + "BTC")
                        price = item.price;
                }
            }
            return price;
        }

        private string fetchBinancePricebyPara(string coin)
        {
            string price = string.Empty;
            if (binancePrice != null)
            {
                foreach (var item in binancePrice)
                {
                    if (item.symbol == coin.ToUpper() + "USDT")
                    {
                        string s = item.price;
                        if (decimal.Parse(s) > 1)
                            price = string.Format("{0:N2}", decimal.Parse(s));
                        else
                            price = string.Format("{0:N5}", decimal.Parse(s));
                    }
                    else if (item.symbol == coin.ToUpper() + "BTC")
                        price = item.price;
                }
            }
            return price;
        }

        private void TxtTimer_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            int sec = 6;
            if (txtTimer.Text != null && int.TryParse(txtTimer.Text, out sec))
            {
                dispatcherTimer.Stop();
                dispatcherTimer.Interval = new TimeSpan(0, 0, int.Parse(txtTimer.Text));
                dispatcherTimer.Start();
            }
        }



        private void btnZoomIn_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in stack.Children)
            {
                var lbl = item as Label;
                lbl.FontSize = lbl.FontSize + 4;
            }

        }

        private void btnZoomOut_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in stack.Children)
            {
                var lbl = item as Label;
                lbl.FontSize = lbl.FontSize - 4;
            }

        }



        private void BtnGreenFont_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in stack.Children)
            {
                var lbl = item as Label;
                if (lbl.Name != "sep")
                    lbl.Foreground = new SolidColorBrush(Color.FromRgb(113, 232, 35));
            }
            
        }
        private void BtnWhiteFont_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in stack.Children)
            {
                var lbl = item as Label;
                if (lbl.Name != "sep")
                    lbl.Foreground = new SolidColorBrush(Colors.White);
            }
         
        }

        private void TxtWidth_LostFocus(object sender, RoutedEventArgs e)
        {
            this.Width = Convert.ToDouble(txtWidth.Text);
            this.rect.Width = Convert.ToDouble(txtWidth.Text);
        }
    }
}