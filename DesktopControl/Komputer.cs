using System.ComponentModel;

/*********************
nazwa klasy: Komputer
opis: Reprezentuje pojedyncze urządzenie w sieci (IP, MAC, hostname, status).
parametry: przekazywane w konstruktorze (ip, mac, hostname, online)
zwracany typ i opis: brak (klasa modelowa)
*********************/
public class Komputer : INotifyPropertyChanged
{
    public string IP { get; set; } = string.Empty;
    public string MAC { get; set; } = string.Empty;
    public string Hostname { get; set; } = string.Empty;
    public bool Onl { get; set; }

    private bool _isSelected;
    private bool _isLocked;


    /*********************
    nazwa właściwości: IsSelected
    opis: Określa czy urządzenie jest zaznaczone w UI
    parametry: wartość bool
    zwracany typ i opis: bool – stan zaznaczenia
    *********************/
    public bool IsSelected
    {
        get => _isSelected;
        set { _isSelected = value; OnPropertyChanged(nameof(IsSelected)); }
    }

    private string _error = "Gotowy";

    /*********************
    nazwa właściwości: Error
    opis: Przechowuje aktualny status operacji (np. błąd, wysłano komendę)
    parametry: string
    zwracany typ i opis: string – komunikat statusu
    *********************/
    public string Error
    {
        get => _error;
        set { _error = value; OnPropertyChanged(nameof(Error)); }
    }


    public bool IsLocked
    {
        get => _isLocked;
        set
        {
            _isLocked = value;
            OnPropertyChanged(nameof(IsLocked));
        }
    }

    /*********************
    nazwa konstruktora: Komputer
    opis: Inicjalizuje obiekt urządzenia
    parametry: ip (string), mac (string), hostname (string), online (bool)
    zwracany typ i opis: brak
    *********************/
    public Komputer(string ip, string mac, string hostname, bool online)
    {
        IP = ip;
        MAC = mac;
        Hostname = hostname;
        Onl = online;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    /*********************
    nazwa metody: OnPropertyChanged
    opis: Wywołuje zdarzenie zmiany właściwości (binding UI)
    parametry: name (string) – nazwa właściwości
    zwracany typ i opis: void
    *********************/
    protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}