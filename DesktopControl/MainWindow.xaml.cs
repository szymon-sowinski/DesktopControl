using System.Collections.ObjectModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading.Tasks;
namespace DesktopControl
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        public static extern int SendARP(int destIp, int srcIp, byte[] macAddr, ref int physicalAddrLen);
        ObservableCollection<Komputer> komputery = new ObservableCollection<Komputer>();
        private string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            throw new Exception("Brak IP");
        }
        private string GetHostName(string ip)
        {
            try
            {
                var host = Dns.GetHostEntry(ip);
                return host.HostName;
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
                int length = macAddr.Length;

                int dest = BitConverter.ToInt32(IPAddress.Parse(ip).GetAddressBytes(), 0);

                SendARP(dest, 0, macAddr, ref length);

                string mac = BitConverter.ToString(macAddr);
                return mac;
            }
            catch
            {
                return "Unknown";
            }
        }
        public MainWindow()
		{
			InitializeComponent();
            DataContext = this;
        }

        private async void WykryjKomputery(object sender, RoutedEventArgs e)
        {
            komputery.Clear();
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
                        Ping ping = new Ping();
                        PingReply reply = await ping.SendPingAsync(ip, 200);

                        if (reply.Status == IPStatus.Success)
                        {
                            string hostname = GetHostName(ip);
                            string mac = GetMacAddress(ip);

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                komputery.Add(new Komputer(
                                    ip,
                                    mac,
                                    hostname,
                                    true,
                                    DateTime.Now
                                ));
                            });
                        }
                    }
                    catch { }
                }));
            }
            foreach (var komputer in komputery)
            {
                Console.WriteLine($"IP: {komputer.IP}, MAC: {komputer.MAC}, Host: {komputer.Hostname}, Online: {komputer.Onl}");
            }
            await Task.WhenAll(tasks);


        }
    }
}