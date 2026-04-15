using System.Collections.ObjectModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Windows;

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

            // 🔥 test czy binding działa
            Komputery.Add(new Komputer("TEST", "AA-BB-CC", "LOCAL", true, DateTime.Now, ""));
        }

        private async void WykryjKomputery(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Skanowanie sieci...");

            await ScanNetwork();

            MessageBox.Show($"Znaleziono: {Komputery.Count}");
        }

        private async Task ScanNetwork()
        {
            Komputery.Clear();

            string localIP = GetLocalIPAddress();
            string subnet = localIP.Substring(0, localIP.LastIndexOf('.') + 1);

            SemaphoreSlim semaphore = new SemaphoreSlim(50); // 🔥 limit równoległości
            List<Task> tasks = new List<Task>();

            for (int i = 1; i < 255; i++)
            {
                string ip = subnet + i;

                await semaphore.WaitAsync();

                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        using Ping ping = new Ping();

                        var reply = await ping.SendPingAsync(ip, 800);

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
                    finally
                    {
                        semaphore.Release();
                    }
                }));
            }

            await Task.WhenAll(tasks);
        }

        private string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();

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
                byte[] mac = new byte[6];
                int len = mac.Length;

                int dest = BitConverter.ToInt32(
                    IPAddress.Parse(ip).GetAddressBytes(), 0);

                SendARP(dest, 0, mac, ref len);

                return BitConverter.ToString(mac);
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}