namespace DesktopControl
{
    public class Komputer
    {
        public string IP { get; set; }
        public string MAC { get; set; }
        public string Hostname { get; set; }
        public bool Onl { get; set; }
        public DateTime OstatnieLogowanie { get; set; }
        public string Error { get; set; }

        // 🔥 ALIASY POD TWÓJ XAML
        public string Ip => IP;
        public string Mac => MAC;

        public Komputer(string ip, string mac, string hostname, bool onl, DateTime ostatnieLogowanie, string error = "")
        {
            IP = ip;
            MAC = mac;
            Hostname = hostname;
            Onl = onl;
            OstatnieLogowanie = ostatnieLogowanie;
            Error = error;
        }
    }
}