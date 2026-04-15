using System.Collections.ObjectModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DesktopControl
{
    public partial class MainWindow : Window
    {
        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        public static extern int SendARP(int destIp, int srcIp, byte[] macAddr, ref int physicalAddrLen);

        public ObservableCollection<Komputer> Komputery { get; set; }
            = new ObservableCollection<Komputer>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private async void WykryjKomputery(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Wykrywanie komputerów...");

            Komputery.Clear();

            string localIP = GetLocalIPAddress();
            string subnet = localIP.Substring(0, localIP.LastIndexOf('.') + 1);

            List<Task> tasks = new List<Task>();

            for (int i = 1; i < 255; i++)
            {
                string ip = subnet + i;

                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        using Ping ping = new Ping();
                        var reply = await ping.SendPingAsync(ip, 200);

                        if (reply.Status == IPStatus.Success)
                        {
                            string hostname = GetHostName(ip);
                            string mac = GetMacAddress(ip);

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                Komputery.Add(new Komputer(
                                    ip,
                                    mac,
                                    hostname,
                                    true,
                                    DateTime.Now,
                                    ""
                                ));
                            });
                        }
                    }
                    catch
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Komputery.Add(new Komputer(
                                ip,
                                "Unknown",
                                "Unknown",
                                false,
                                DateTime.Now,
                                "Error"
                            ));
                        });
                    }
                }));
            }

            await Task.WhenAll(tasks);
            Komputery.Add(new Komputer(
                "111.111.111.1.111",
                "idk",
                "idk",
                false,
                DateTime.Now,
                "idk"
                ));
            MessageBox.Show($"Znaleziono: {Komputery.Count}");
        }

        private string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
            }

            throw new Exception("Brak IP");
        }

        private string GetHostName(string ip)
        {
            try
            {
                return Dns.GetHostEntry(ip).HostName;
            }
            catch
            {
                return "Unknown";
            }
        }

        private string GetMacAddress(string ip)
        {
            try
            {
                byte[] macAddr = new byte[6];
                int len = macAddr.Length;

                int dest = BitConverter.ToInt32(
                    IPAddress.Parse(ip).GetAddressBytes(), 0);

                SendARP(dest, 0, macAddr, ref len);

                return BitConverter.ToString(macAddr);
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}