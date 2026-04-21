using System;
using System.IO;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Media.Imaging;

namespace DesktopControl
{
    public partial class PreviewWindow : Window
    {
        public PreviewWindow(string ip)
        {
            InitializeComponent();
            Connect(ip);
        }

        private async void Connect(string ip)
        {
            try
            {
                MessageBox.Show("Próba połączenia z " + ip);

                TcpClient client = new TcpClient();

                MessageBox.Show("Łączenie...");
                await client.ConnectAsync(ip, 5000);

                var stream = client.GetStream();

                while (true)
                {
                    byte[] lenBytes = new byte[4];
                    int readLen = await stream.ReadAsync(lenBytes, 0, 4);

                    int len = BitConverter.ToInt32(lenBytes, 0);

                    byte[] data = new byte[len];
                    int read = 0;

                    while (read < len)
                    {
                        int r = await stream.ReadAsync(data, read, len - read);
                        if (r == 0) throw new Exception("Rozłączono");
                        read += r;
                    }

                    using (MemoryStream ms = new MemoryStream(data))
                    {
                        BitmapImage img = new BitmapImage();
                        img.BeginInit();
                        img.StreamSource = ms;
                        img.CacheOption = BitmapCacheOption.OnLoad;
                        img.EndInit();

                        Dispatcher.Invoke(() =>
                        {
                            ScreenView.Source = img;
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("BŁĄD: " + ex.ToString());
            }
        }
    }
}