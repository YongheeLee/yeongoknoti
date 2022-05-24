using NSoup.Nodes;
using NSoup.Select;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;

namespace WpfApp7
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly HttpClient httpClient = new HttpClient();

        DispatcherTimer timer = new DispatcherTimer();
        ObservableCollection<string> sss = new ObservableCollection<string>();

        public MainWindow()
        {
            InitializeComponent();

            timer.Tick += Timer_Tick;
            list.ItemsSource = sss;

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            string dateTxt = date.Text;

            string ymText = dateTxt.Substring(0, 6);
            string dyText = dateTxt.Substring(6, 2);

            bool ca = cba.IsChecked.HasValue ? cba.IsChecked.Value : false;
            bool cb = cbb.IsChecked.HasValue ? cbb.IsChecked.Value : false;
            bool cc = cbc.IsChecked.HasValue ? cbc.IsChecked.Value : false;
            bool cd = cbd.IsChecked.HasValue ? cbd.IsChecked.Value : false;
            bool ce = cbe.IsChecked.HasValue ? cbe.IsChecked.Value : false;

            if (dyText[0] == '0')
                dyText = dyText.Substring(1, 1);

            string urlStr = string.Format("https://camping.gtdc.or.kr/DZ_reservation/reserCamping_v3.php?xch=reservation&xid=camping_reservation&sdate={0}", ymText);
            Document doc = NSoup.NSoupClient.Parse(new Uri(urlStr), 5000);
            Elements el = doc.Select("table.dztbl tbody tr td");

            foreach (var item in el)
            {
                Element day = item.Select("div span").First;
                if (day.Text() == dyText)
                {
                    Elements btns = item.Select("ul li button");
                    bool found = false;
                    foreach (var btn in btns)
                    {
                        string val = btn.Val();

                        if (!string.IsNullOrEmpty(val))
                        {
                            if ((val[0] == 'A' && ca) || (val[0] == 'B' && cb) || (val[0] == 'C' && cc) || (val[0] == 'D' && cd) || (val[0] == 'E' && ce))
                            {
                                found = true;
                                break;
                            }
                        }
                    }
                    if (found)
                    {
                        sss.Add("빈자리 나옴!!!!");
                        Process.Start("chrome.exe", urlStr);
                        MessageBox.Show("빨리!!!!!!!!!!!!", "긴급");
                    }
                    else
                        sss.Add("없음");

                    if (sss.Count > 200)
                        sss.Clear();
                }
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();

            if (interval.SelectedIndex == -1)
                return;

            int sec = interval.SelectedIndex == 0 ? 60 : interval.SelectedIndex == 1 ? 60 * 5 : 60 * 10;

            string dateTxt = date.Text;
            if (dateTxt.Length != 8)
                return;

            int t = 0;
            if (!int.TryParse(dateTxt, out t))
                return;

            if (int.Parse(dateTxt.Substring(4, 2)) > 12)
                return;

            if (int.Parse(dateTxt.Substring(6, 2)) > 31)
                return;

            timer.Interval = TimeSpan.FromSeconds(sec);
            timer.Start();
            sss.Add("Started");
            bta.IsEnabled = false;
            btb.IsEnabled = true;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            sss.Clear();
            sss.Add("Stopped");
            bta.IsEnabled = true;
            btb.IsEnabled = false;
        }
    }
}
