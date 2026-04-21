using System;

namespace DesktopControl

{

	/*********************
    nazwa klasy: Komputer
    opis: Model reprezentujący komputer w sieci. Przechowuje podstawowe dane takie jak adres IP, MAC, nazwa hosta, status online oraz datę ostatniego logowania.
    parametry: ip, mac, hostname, onl, ostatnieLogowanie (przekazywane w konstruktorze)
    zwracany typ i opis: brak (klasa modelowa)
    *********************/

	public class Komputer

	{

		public string IP { get; set; }
		public string MAC { get; set; }
		public string Hostname { get; set; }
		public bool Onl { get; set; }
		public DateTime OstatnieLogowanie { get; set; }

		/*********************
        nazwa konstruktora: Komputer
        opis: Inicjalizuje obiekt komputera wszystkimi wymaganymi danymi
        parametry: ip, mac, hostname, onl, ostatnieLogowanie
        zwracany typ i opis: brak
        *********************/

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
