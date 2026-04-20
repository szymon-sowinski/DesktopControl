using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace DesktopControl
{
    public class DeviceItem : INotifyPropertyChanged
    {
        public string IP { get; set; } = string.Empty;
        public string MAC { get; set; } = string.Empty;
        public string Hostname { get; set; } = string.Empty;
        public bool Onl { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(nameof(IsSelected)); }
        }

        private string _error = "Gotowy";
        public string Error
        {
            get => _error;
            set { _error = value; OnPropertyChanged(nameof(Error)); }
        }

        public DeviceItem(string ip, string mac, string hostname, bool online)
        {
            IP = ip;
            MAC = mac;
            Hostname = hostname;
            Onl = online;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public partial class MainWindow : Window
    {
        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        public static extern int SendARP(int destIp, int srcIp, byte[] macAddr, ref int physicalAddrLen);

        public ObservableCollection<DeviceItem> Komputery { get; set; } = new ObservableCollection<DeviceItem>();

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            ComputersGrid.ItemsSource = Komputery;
        }
        private bool IsValidIp(string ip)
        {
            if (!IPAddress.TryParse(ip, out var address))
                return false;

            byte[] bytes = address.GetAddressBytes();

            if (bytes[0] >= 224 && bytes[0] <= 239)
                return false;

            if (ip == "255.255.255.255")
                return false;

            if (bytes[3] == 255)
                return false;

            return true;
        }
        private List<(string IP, string MAC)> GetDevicesFromArp()
        {
            List<(string, string)> result = new List<(string, string)>();

            try
            {
                Process p = new Process();
                p.StartInfo.FileName = "arp";
                p.StartInfo.Arguments = "-a";
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;

                p.Start();
                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();

                var lines = output.Split('\n');

                foreach (var line in lines)
                {
                    var match = System.Text.RegularExpressions.Regex.Match(
                        line,
                        @"(\d+\.\d+\.\d+\.\d+)\s+([a-fA-F0-9\-]+)"
                    );

                    if (match.Success)
                    {
                        string ip = match.Groups[1].Value;
                        string mac = match.Groups[2].Value;

                        if (!string.IsNullOrWhiteSpace(ip) &&
                            !string.IsNullOrWhiteSpace(mac) &&
                            IsValidIp(ip))
                        {
                            result.Add((ip, mac));
                        }
                    }
                }
            }
            catch { }

            return result;
        }

        // --- NOWA METODA: ZAZNACZ WSZYSTKIE ---
        private void BtnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var komputery in Komputery)
            {
                komputery.IsSelected = true;
            }
        }

        private async void BtnPowerOff_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var device = button?.DataContext as DeviceItem;

            if (device == null) return;

            var confirm = MessageBox.Show($"Czy na pewno wyłączyć {device.IP}?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm != MessageBoxResult.Yes) return;

            device.Error = "Zamykanie...";

            try
            {
                await Task.Run(() =>
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "shutdown",
                        Arguments = $"/m \\\\{device.IP} /s /f /t 0",
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardError = true
                    };

                    using (Process? proc = Process.Start(psi))
                    {
                        if (proc != null)
                        {
                            string stdError = proc.StandardError.ReadToEnd();
                            proc.WaitForExit();

                            Dispatcher.Invoke(() =>
                            {
                                if (proc.ExitCode == 0)
                                    device.Error = "Wysłano sygnał.";
                                else
                                    device.Error = $"Błąd: {stdError.Trim()}";
                            });
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                device.Error = $"Wyjątek: {ex.Message}";
            }
        }

        private string GetLocalIPAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                    if (ip.AddressFamily == AddressFamily.InterNetwork) return ip.ToString();
            }
            catch { }
            return "127.0.0.1";
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
            catch { return "Unknown"; }
        }

        private async void WykryjKomputery(object sender, RoutedEventArgs e)
        {
            Komputery.Clear();

            string localIP = GetLocalIPAddress();
            string subnet = localIP.Substring(0, localIP.LastIndexOf('.') + 1);

            List<Task> tasks = new List<Task>();
            SemaphoreSlim semaphore = new SemaphoreSlim(40);

            // 🔹 1. Ping sweep (uzupełnia ARP)
            for (int i = 1; i < 255; i++)
            {
                string ip = subnet + i;

                tasks.Add(Task.Run(async () =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        using (Ping ping = new Ping())
                        {
                            try
                            {
                                await ping.SendPingAsync(ip, 400);
                            }
                            catch { }
                        }
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));
            }

            await Task.WhenAll(tasks);

            var devices = GetDevicesFromArp();

            foreach (var d in devices)
            {
                Dispatcher.Invoke(() =>
                {
                    Komputery.Add(new DeviceItem(
                        d.IP,
                        d.MAC,
                        GetHostName(d.IP),
                        true
                    ));
                });
            }
        }

        private void SendWakeOnLan(string macAddress)
        {
            try
            {
                byte[] macBytes = new byte[6];
                string[] hex = macAddress.Split('-');

                for (int i = 0; i < 6; i++)
                    macBytes[i] = Convert.ToByte(hex[i], 16);

                byte[] packet = new byte[102];
                for (int i = 0; i < 6; i++) packet[i] = 0xFF;
                for (int i = 1; i <= 16; i++) Buffer.BlockCopy(macBytes, 0, packet, i * 6, 6);

                using (UdpClient client = new UdpClient())
                {
                    client.EnableBroadcast = true;
                    client.Send(packet, packet.Length, new IPEndPoint(IPAddress.Broadcast, 9));
                }
            }
            catch { }
        }

        private async void BtnPowerOn_Click(object sender, RoutedEventArgs e)
        {
            var przycisk = sender as Button;
            var urz = przycisk?.DataContext as DeviceItem;

            if (urz == null) return;

            urz.Error = "Włączanie...";

            try
            {
                await Task.Run(() => { SendWakeOnLan(urz.MAC); });
                urz.Error = "Wysłano WoL.";
            }
            catch (Exception ex)
            {
                urz.Error = $"Błąd: {ex.Message}";
            }
        }
    }

    // --- KONWERTER KOLORÓW STATUSU ---
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string status = value as string ?? "";
            if (status == "Gotowy" || status == "Wysłano sygnał." || status == "Wysłano WoL.")
                return Brushes.LightGreen;

            if (status.Contains("Błąd") || status.Contains("Wyjątek") || status.Contains("Odmowa"))
                return Brushes.Tomato;

            if (status.Contains("..."))
                return Brushes.Orange;

            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}