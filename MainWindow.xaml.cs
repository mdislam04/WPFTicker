using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;


namespace Ticker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        dynamic koinexPrice;
        dynamic binancePrice;
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();

            GetKoinexPrice();
            GetBinancePrice();
            
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, int.Parse(txtTimer.Text));
            dispatcherTimer.Start();
        }

        private void DispatcherTimer_Tick(object sender, System.EventArgs e)
        {
            GetKoinexPrice();
            GetBinancePrice();
        }

        private async void GetKoinexPrice()
        {
            using (var client = new HttpClient())
            {
                var url = "https://koinex.in/api/ticker";
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", "C# App");
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
                lblKoinex.Content = koinexPrice.prices.inr[txtCoin.Text.ToUpper()];
                lblBinance.Content = fetchBinancePrice();
            }
        }

        private string fetchBinancePrice()
        {
            string price = string.Empty;
            if (binancePrice != null)
            {
                foreach (var item in binancePrice)
                {
                    if (item.symbol == txtCoin.Text.ToUpper() + "USDT")
                        price = item.price;// string.Format("{0:N5}", decimal.Parse(item.price));
                    else if (item.symbol == txtCoin.Text.ToUpper() + "BTC")
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
    }
}
