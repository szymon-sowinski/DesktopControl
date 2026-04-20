using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

class Program
{
    static void Main()
    {
        TcpListener server = new TcpListener(IPAddress.Any, 5000);
        server.Start();

        var client = server.AcceptTcpClient();
        var stream = client.GetStream();

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