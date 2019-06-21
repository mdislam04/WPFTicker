using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;
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
                getPrices();
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
                GetKoinexPrice();
                GetBinancePrice();
            }
            catch(Exception ex)
            {
                lblBinance.Content = ex.Message;
            }
        }

        

        private void DispatcherTimer_Tick(object sender, System.EventArgs e)
        {
            getPrices();
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
            if (koinexPrice != null && binancePrice != null)
            {
                //lblKoinex.Content = koinexPrice.prices.inr[txtCoin.Text.ToUpper()];
                lblKoinex.Content = fetchBinancePrice();
                lblBinance.Content = fetchBinancePricebyPara("BTC"); 
            }
        }

        private string fetchBinancePrice(string coin = null)
        {
            string price = string.Empty;
                      if (binancePrice != null)
            {
                foreach (var item in binancePrice)
                {
                    if (item.symbol == txtCoin.Text.ToUpper() + "USDT")
                    {
                        string s = item.price;
                        price = string.Format("{0:N5}", decimal.Parse(s));
                    }
                    else if (item.symbol == txtCoin.Text.ToUpper() + "BTC")
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
                        price = string.Format("{0:N2}", decimal.Parse(s));
                    }
                    else if (item.symbol == coin.ToUpper() + "BTC")
                        price = item.price;
                }
            }
            return price;
        }

        private void TxtTimer_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            int sec = 10;
            if (txtTimer.Text != null && int.TryParse(txtTimer.Text, out sec))
            {              
                dispatcherTimer.Stop();
                dispatcherTimer.Interval = new TimeSpan(0, 0, int.Parse(txtTimer.Text));
                dispatcherTimer.Start();
            }
        }

        private void btnZoomIn_Click(object sender, RoutedEventArgs e)
        {
            lblKoinex.FontSize = lblKoinex.FontSize + 4;
            lblBinance.FontSize = lblKoinex.FontSize + 4;
            lblSep.FontSize = lblKoinex.FontSize + 4;
        }

        private void btnZoomOut_Click(object sender, RoutedEventArgs e)
        {
            lblKoinex.FontSize = lblKoinex.FontSize - 4;
            lblBinance.FontSize = lblKoinex.FontSize - 4;
            lblSep.FontSize = lblKoinex.FontSize - 4;
        }

        public static string DoFormat(double myNumber)
        {
            var s = string.Format("{0:0.00}", myNumber);

            if (s.EndsWith("00"))
            {
                return ((int)myNumber).ToString();
            }
            else
            {
                return s;
            }
        }

        private void BtnGreenFont_Click(object sender, RoutedEventArgs e)
        {
            lblBinance.Foreground = new SolidColorBrush(Color.FromRgb(113, 232, 35));
            lblKoinex.Foreground = new SolidColorBrush(Color.FromRgb(113, 232, 35));
        }
        private void BtnWhiteFont_Click(object sender, RoutedEventArgs e)
        {
            lblBinance.Foreground = new SolidColorBrush(Colors.White);
            lblKoinex.Foreground = new SolidColorBrush(Colors.White);
        }
    }
}
