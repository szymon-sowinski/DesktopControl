/*********************
nazwa pliku: Agent (Program)
opis: Aplikacja działająca jako agent na komputerze. Umożliwia zdalny podgląd ekranu (port 5000) oraz wykonywanie komend (port 6000, np. shutdown).
*********************/
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

/*********************
nazwa klasy: Program
opis: Główna klasa aplikacji. Uruchamia serwer podglądu ekranu oraz serwer komend.
*********************/
class Program
{
	/*********************
    nazwa metody: Main
    opis: Punkt startowy aplikacji. Uruchamia dwa wątki: ScreenServer oraz CommandServer.
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
    opis: Serwer TCP wysyłający obraz ekranu do klienta (port 5000). Wysyła kolejne klatki jako JPEG.
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
				Console.WriteLine("Klient podglądu rozłączony");
				client.Close();
			}
		}
	}

	/*********************
    nazwa metody: CommandServer
    opis: Serwer TCP odbierający komendy (port 6000). Obsługuje m.in. komendę "shutdown".
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

			client.Close();
		}
	}

	/*********************
    nazwa metody: Capture
    opis: Wykonuje zrzut ekranu i zwraca go jako obiekt Bitmap.
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
}