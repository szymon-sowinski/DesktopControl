using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

/*********************
nazwa klasy: Program
opis: Główna klasa aplikacji. Uruchamia serwery sieciowe oraz zarządza blokadą ekranu.
*********************/
class Program
{
    static Form? lockForm;

    /*********************
    nazwa metody: Main
    opis: Punkt startowy aplikacji. Uruchamia w osobnych wątkach ScreenServer oraz CommandServer.
    *********************/
    static void Main()
    {
        Console.WriteLine("Start agenta...");

        new Thread(ScreenServer).Start();
        new Thread(CommandServer).Start();

        Console.WriteLine("Działa. ENTER = exit");
        Console.ReadLine();
    }

    /*********************
    nazwa metody: ScreenServer
    opis: Serwer TCP wysyłający obraz ekranu do klienta (port 5000). Wysyła cyklicznie zrzuty ekranu w formacie JPEG.
    *********************/
    static void ScreenServer()
    {
        TcpListener server = new TcpListener(IPAddress.Any, 5000);
        server.Start();

        Console.WriteLine("Screen server (5000) działa...");

        while (true)
        {
            var client = server.AcceptTcpClient();
            Console.WriteLine("Klient podglądu połączony");

            var stream = client.GetStream();

            try
            {
                while (true)
                {
                    using (Bitmap bmp = Capture())
                    using (MemoryStream ms = new MemoryStream())
                    {
                        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

                        byte[] data = ms.ToArray();
                        byte[] len = BitConverter.GetBytes(data.Length);

                        stream.Write(len, 0, 4);
                        stream.Write(data, 0, data.Length);
                    }

                    Thread.Sleep(100);
                }
            }
            catch
            {
                Console.WriteLine("Klient rozłączony");
                client.Close();
            }
        }
    }

    /*********************
    nazwa metody: CommandServer
    opis: Serwer TCP odbierający komendy (port 6000). Obsługuje komendy: shutdown, lock, unlock.
    *********************/
    static void CommandServer()
    {
        TcpListener server = new TcpListener(IPAddress.Any, 6000);
        server.Start();

        Console.WriteLine("Command server (6000) działa...");

        while (true)
        {
            var client = server.AcceptTcpClient();
            var stream = client.GetStream();

            byte[] buffer = new byte[1024];
            int len = stream.Read(buffer, 0, buffer.Length);

            string cmd = Encoding.UTF8.GetString(buffer, 0, len);

            Console.WriteLine("Komenda: " + cmd);

            if (cmd == "shutdown")
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "shutdown",
                    Arguments = "/s /f /t 0",
                    CreateNoWindow = true,
                    UseShellExecute = false
                });
            }
            else if (cmd == "lock")
            {
                new Thread(ShowLockScreen)
                {
                    IsBackground = true
                }.Start();
            }
            else if (cmd == "unlock")
            {
                HideLockScreen();
            }

            client.Close();
        }
    }

    /*********************
    nazwa metody: Capture
    opis: Wykonuje zrzut ekranu i zwraca go jako Bitmap.
    *********************/
    static Bitmap Capture()
    {
        int w = 1920;
        int h = 1080;

        Bitmap bmp = new Bitmap(w, h);

        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.CopyFromScreen(0, 0, 0, 0, bmp.Size);
        }

        return bmp;
    }

    /*********************
    nazwa metody: ShowLockScreen
    opis: Wyświetla pełnoekranową blokadę komputera z komunikatem dla użytkownika. Ukrywa kursor.
    *********************/
    static void ShowLockScreen()
    {
        if (lockForm != null)
            return;

        Thread t = new Thread(() =>
        {
            lockForm = new Form();

            lockForm.FormBorderStyle = FormBorderStyle.None;
            lockForm.WindowState = FormWindowState.Maximized;
            lockForm.TopMost = true;
            lockForm.BackColor = Color.Black;
            lockForm.ShowInTaskbar = false;

            Label label = new Label();
            label.Text = "KOMPUTER ZABLOKOWANY\nSkontaktuj się z administratorem";
            label.ForeColor = Color.White;
            label.Font = new Font("Arial", 40, FontStyle.Bold);
            label.Dock = DockStyle.Fill;
            label.TextAlign = ContentAlignment.MiddleCenter;

            lockForm.Controls.Add(label);

            Cursor.Hide();

            Application.Run(lockForm);
        });

        t.SetApartmentState(ApartmentState.STA);
        t.Start();
    }

    /*********************
    nazwa metody: HideLockScreen
    opis: Zamyka ekran blokady i przywraca widoczność kursora.
    *********************/
    static void HideLockScreen()
    {
        if (lockForm != null)
        {
            lockForm.Invoke(new Action(() =>
            {
                Cursor.Show();
                lockForm.Close();
            }));

            lockForm = null;
        }
    }
}
