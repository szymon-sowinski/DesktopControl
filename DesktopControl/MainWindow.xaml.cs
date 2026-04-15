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

namespace DesktopControl
{
	public class Computer
	{
		public bool IsSelected { get; set; }
		public string Mac { get; set; }
		public string Ip { get; set; }
		public string Error { get; set; }
	}
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			var computers = new List<Computer>
			{
				new Computer
				{
					IsSelected = false,
					Mac = "00:1A:2B:3C:4D:5E",
					Ip = "192.168.0.255",
					Error = "OK"
				},
				new Computer
				{
					IsSelected = true,
					Mac = "AA:BB:CC:DD:EE:FF",
					Ip = "192.168.0.255",
					Error = "Offline"
				}
			};

			ComputersGrid.ItemsSource = computers;
		}
	}
}