using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;

namespace DesktopControl
{
    public partial class MainWindow : Window
    {
        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        public static extern int SendARP(int destIp, int srcIp, byte[] macAddr, ref int physicalAddrLen);

        public ObservableCollection<Komputer> Komputery { get; set; } = new ObservableCollection<Komputer>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
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
            try { return Dns.GetHostEntry(ip).HostName; }
            catch { return "Unknown"; }
        }

        private string GetMacAddress(string ip)
        {
            try
            {
                byte[] macAddr = new byte[6];
                int len = macAddr.Length;

                int dest = BitConverter.ToInt32(IPAddress.Parse(ip).GetAddressBytes(), 0);

                SendARP(dest, 0, macAddr, ref len);

                return BitConverter.ToString(macAddr);
            }
            catch
            {
                return "Unknown";
            }
        }

        private async void WykryjKomputery(object sender, RoutedEventArgs e)
        {
            Komputery.Clear();

            string localIP = GetLocalIPAddress();
            string subnet = localIP.Substring(0, localIP.LastIndexOf('.') + 1);

            List<Task> tasks = new List<Task>();
            object lockObj = new object();
            HashSet<string> found = new HashSet<string>();

            for (int i = 1; i < 255; i++)
            {
                string ip = subnet + i;

                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        using (Ping ping = new Ping())
                        {
                            var reply = await ping.SendPingAsync(ip, 200);

                            if (reply.Status == IPStatus.Success)
                            {
                                lock (lockObj)
                                {
                                    if (!found.Add(ip))
                                        return;
                                }

                                var komputer = new Komputer(
                                    ip,
                                    GetMacAddress(ip),
                                    GetHostName(ip),
                                    true,
                                    DateTime.Now
                                );

                                Dispatcher.Invoke(() =>
                                {
                                    Komputery.Add(komputer);
                                });
                            }
                        }
                    }
                    catch { }
                }));
            }

            await Task.WhenAll(tasks);
        }
    }
}