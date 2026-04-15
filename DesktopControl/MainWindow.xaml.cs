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

namespace DesktopControl
{
	// Zmieniłem nazwę na DeviceItem, żeby uniknąć błędu niejednoznaczności (Ambiguity)
	public class DeviceItem : INotifyPropertyChanged
	{
		public string IP { get; set; } = string.Empty;
		public string MAC { get; set; } = string.Empty;
		public string Hostname { get; set; } = string.Empty;
		public bool IsSelected { get; set; }
		public bool Onl { get; set; }

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

		// Używamy nowej nazwy klasy DeviceItem
		public ObservableCollection<DeviceItem> Komputery { get; set; } = new ObservableCollection<DeviceItem>();

		public MainWindow()
		{
			InitializeComponent();
			this.DataContext = this;
			ComputersGrid.ItemsSource = Komputery;
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

			for (int i = 1; i < 255; i++)
			{
				string ip = subnet + i;
				tasks.Add(Task.Run(async () =>
				{
					using (Ping ping = new Ping())
					{
						try
						{
							var reply = await ping.SendPingAsync(ip, 150);
							if (reply.Status == IPStatus.Success)
							{
								var k = new DeviceItem(ip, GetMacAddress(ip), GetHostName(ip), true);
								Dispatcher.Invoke(() => Komputery.Add(k));
							}
						}
						catch { }
					}
				}));
			}
			await Task.WhenAll(tasks);
		}
	}
}