using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Documents;

namespace DesktopControl
{
    internal class Komputer
    {
        public string IP { get; set; }
        public string MAC { get; set; }
        public string Hostname { get; set; }
        public bool Onl { get; set; }
        public DateTime OstatnieLogowanie { get; set; }
        public Komputer(string ip, string mac, string hostname, bool onl, DateTime ostatnieLogowanie)
        {
            IP = ip;
            MAC = mac;
            Hostname = hostname;
            Onl = onl;
            OstatnieLogowanie = ostatnieLogowanie;
        }
    }
}
