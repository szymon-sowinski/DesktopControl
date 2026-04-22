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

	/*********************
	nazwa klasy: MainWindow
	opis: Główne okno aplikacji zarządzającej urządzeniami w sieci
	parametry: brak
	zwracany typ i opis: brak
	*********************/
	public partial class MainWindow : Window
	{
		[DllImport("iphlpapi.dll", ExactSpelling = true)]
		public static extern int SendARP(int destIp, int srcIp, byte[] macAddr, ref int physicalAddrLen);

		public ObservableCollection<Komputer> Komputery { get; set; } = new ObservableCollection<Komputer>();

		/*********************
		nazwa konstruktora: MainWindow
		opis: Inicjalizuje UI oraz ustawia źródło danych
		parametry: brak
		zwracany typ i opis: brak
		*********************/
		public MainWindow()
		{
			InitializeComponent();
			this.DataContext = this;
			ComputersGrid.ItemsSource = Komputery;
		}

		/*********************
		nazwa metody: IsValidIp
		opis: Sprawdza czy adres IP jest poprawny i nie jest broadcastem/multicastem
		parametry: ip (string)
		zwracany typ i opis: bool – true jeśli poprawny
		*********************/
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

		/*********************
		nazwa metody: GetDevicesFromArp
		opis: Pobiera listę urządzeń z tablicy ARP systemu
		parametry: brak
		zwracany typ i opis: lista (IP, MAC)
		*********************/
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

		/*********************
		nazwa metody: BtnSelectAll_Click
		opis: Zaznacza wszystkie urządzenia na liście
		parametry: sender, RoutedEventArgs
		zwracany typ i opis: void
		*********************/
		private void BtnSelectAll_Click(object sender, RoutedEventArgs e)
		{
			foreach (var komputery in Komputery)
			{
				komputery.IsSelected = true;
			}
		}

		/*********************
		nazwa metody: BtnPowerOff_Click
		opis: Wysyła komendę wyłączenia do urządzenia przez TCP
		parametry: sender, RoutedEventArgs
		zwracany typ i opis: void
		*********************/
		private async void BtnPowerOff_Click(object sender, RoutedEventArgs e)
		{
			var button = sender as Button;
			var device = button?.DataContext as Komputer;

			if (device == null) return;

			device.Error = "Wyłączanie...";

			try
			{
				using (TcpClient client = new TcpClient())
				{
					await client.ConnectAsync(device.IP, 6000);

					byte[] data = System.Text.Encoding.UTF8.GetBytes("shutdown");
					await client.GetStream().WriteAsync(data, 0, data.Length);
				}

				device.Error = "Wysłano komendę";
			}
			catch (Exception ex)
			{
				device.Error = "Błąd: " + ex.Message;
			}
		}

		/*********************
		nazwa metody: GetLocalIPAddress
		opis: Pobiera lokalny adres IP komputera
		parametry: brak
		zwracany typ i opis: string – adres IP
		*********************/
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

		/*********************
		nazwa metody: GetHostName
		opis: Pobiera nazwę hosta na podstawie IP
		parametry: ip (string)
		zwracany typ i opis: string – hostname
		*********************/
		private string GetHostName(string ip)
		{
			try { return Dns.GetHostEntry(ip).HostName; }
			catch { return "Unknown"; }
		}

		/*********************
		nazwa metody: GetMacAddress
		opis: Pobiera adres MAC na podstawie IP przy użyciu ARP
		parametry: ip (string)
		zwracany typ i opis: string – adres MAC
		*********************/
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

		/*********************
		nazwa metody: WykryjKomputery
		opis: Skanuje sieć i dodaje urządzenia do listy
		parametry: sender, RoutedEventArgs
		zwracany typ i opis: void
		*********************/
		private async void WykryjKomputery(object sender, RoutedEventArgs e)
		{
			Komputery.Clear();

			await Task.Run(() =>
			{
				var devices = GetDevicesFromArp();

				foreach (var d in devices)
				{
					Dispatcher.Invoke(() =>
					{
						Komputery.Add(new Komputer(
							d.IP,
							d.MAC,
							"Unknown",
							true
						));
					});
				}
			});
		}

		/*********************
		nazwa metody: SendWakeOnLan
		opis: Wysyła pakiet Wake-on-LAN do urządzenia
		parametry: macAddress (string)
		zwracany typ i opis: void
		*********************/
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

		/*********************
		nazwa metody: BtnPowerOn_Click
		opis: Włącza urządzenie przez Wake-on-LAN
		parametry: sender, RoutedEventArgs
		zwracany typ i opis: void
		*********************/
		private async void BtnPowerOn_Click(object sender, RoutedEventArgs e)
		{
			var przycisk = sender as Button;
			var urz = przycisk?.DataContext as Komputer;

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

		/*********************
		nazwa metody: Preview_Click
		opis: Otwiera okno podglądu urządzenia
		parametry: sender, RoutedEventArgs
		zwracany typ i opis: void
		*********************/
		private void Preview_Click(object sender, RoutedEventArgs e)
		{
			var button = sender as FrameworkElement;
			var device = button?.DataContext as Komputer;

			if (device == null) return;

			PreviewWindow window = new PreviewWindow(device.IP);
			window.Owner = this;
			window.Show();
		}

		/*********************
		nazwa metody: Btn_Lock
		opis: Obsługuje kliknięcie przycisku blokady. Wysyła do wybranego komputera komendę "lock" lub "unlock" przez TCP (port 6000), a następnie aktualizuje jego stan oraz komunikat statusu.
		parametry: 
		    sender – źródło zdarzenia (przycisk)
		    e – dane zdarzenia RoutedEventArgs
		zwracany typ i opis: void (metoda asynchroniczna obsługująca zdarzenie UI)
		*********************/
        private async void Btn_Lock(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var device = button?.DataContext as Komputer;

            if (device == null) return;

            string command = device.IsLocked ? "unlock" : "lock";

            device.Error = device.IsLocked ? "Odblokowywanie..." : "Blokowanie...";

            try
            {
                using (TcpClient client = new TcpClient())
                {
                    await client.ConnectAsync(device.IP, 6000);

                    byte[] data = System.Text.Encoding.UTF8.GetBytes(command);
                    await client.GetStream().WriteAsync(data, 0, data.Length);
                }

                device.IsLocked = !device.IsLocked;

                device.Error = device.IsLocked ? "Zablokowany" : "Odblokowany";
            }
            catch (Exception ex)
            {
                device.Error = "Błąd: " + ex.Message;
            }

        }
    }


    /*********************
	nazwa klasy: StatusToColorConverter
	opis: Konwerter statusu urządzenia na kolor w UI
	parametry: brak
	zwracany typ i opis: brak
	*********************/
    public class StatusToColorConverter : IValueConverter
	{
		/*********************
		nazwa metody: Convert
		opis: Zamienia status tekstowy na kolor
		parametry: value, targetType, parameter, culture
		zwracany typ i opis: Brush – kolor dla UI
		*********************/
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

		/*********************
		nazwa metody: ConvertBack
		opis: Niezaimplementowana konwersja wsteczna
		parametry: value, targetType, parameter, culture
		zwracany typ i opis: brak (rzuca wyjątek)
		*********************/
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
    }
}
