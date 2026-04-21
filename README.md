# Wprowadzenie

Aplikacja DesktopControl jest aplikacją desktopową, której głównym celem jest umożliwienie zdalnej kontroli jednego komputera z poziomu drugiego urządzenia. Projekt został stworzony z myślą o użytkownikach, którzy potrzebują prostego i efektywnego sposobu zarządzania komputerem na odległość.  

System pozwala na nawiązanie połączenia między dwoma komputerami w tej samej sieci, dzięki czemu użytkownik może wykonywać operacje takie jak podgląd pulpitu, czy włączanie i wyłączanie komputera. Rozwiązanie to zwiększa wygodę pracy, umożliwia szybką pomoc techniczną oraz usprawnia zarządzanie wieloma stanowiskami z poziomu jednego urządzenia.  

Projekt DesktopControl został opracowany jako aplikacja o przejrzystym interfejsie i intuicyjnej obsłudze, tak aby nawet mniej zaawansowani użytkownicy mogli w łatwy sposób korzystać z jego funkcjonalności.  

---

# Wymagania systemowe

Aby aplikacja DesktopControl działała poprawnie i zapewniała stabilne połączenie zdalne między komputerami, system użytkownika powinien spełniać poniższe wymagania:

- **System operacyjny:** Windows 11  
- **Sieć:** aktywne połączenie z Internetem lub siecią lokalną  

---

# Proces konfiguracji

Aby aplikacja DesktopControl działała poprawnie i umożliwiała zdalne zarządzanie urządzeniami, wymagane jest skonfigurowanie systemu w dwóch miejscach: na komputerze użytkownika (sterującym) oraz na komputerze zdalnym (klienckim).

Na urządzeniu zdalnym musi być uruchomiona specjalna aplikacja agenta. Jest ona odpowiedzialna za udostępnienie funkcji takich jak podgląd ekranu oraz zdalne włączanie i wyłączanie komputera. Bez aktywnego agenta urządzenie nie będzie widoczne jako w pełni obsługiwalne w systemie DesktopControl.

Proces konfiguracji na urządzeniu zdalnym przebiega następująco: najpierw należy uruchomić aplikację agenta na komputerze docelowym. Następnie aplikacja automatycznie inicjuje usługę nasłuchującą w sieci lokalnej i udostępnia swój status systemowi DesktopControl. Od tego momentu komputer staje się dostępny na liście urządzeń w aplikacji głównej.

Dopiero po poprawnym uruchomieniu agenta możliwe jest korzystanie z funkcji takich jak podgląd pulpitu oraz zdalne włączanie i wyłączanie urządzenia z poziomu innego komputera w tej samej sieci lokalnej.

---

# Interfejs użytkownika

Interfejs aplikacji DesktopControl został zaprojektowany w sposób przejrzysty i nowoczesny, z naciskiem na szybki dostęp do najważniejszych funkcji zarządzania urządzeniami w sieci.

## Górna część aplikacji

Na samej górze znajduje się pasek nagłówka zawierający:
- ikonę aplikacji  
- nazwę **Desktop Control**  

Nagłówek pełni funkcję informacyjną i identyfikuje aplikację.

---

## Główna sekcja – lista urządzeń

Centralnym elementem interfejsu jest tabela prezentująca wykryte urządzenia w sieci lokalnej. Urządzenia zostaną wykryte po kliknięciu przycisku **„Wykryj komputery”**.  

Tabela zawiera następujące kolumny:
- **MAC Address** – adres fizyczny urządzenia  
- **IP Address** – adres IP w sieci  
- **Hostname** – nazwa komputera (jeśli dostępna)  
- **Online** – status dostępności (np. True)  
- **Akcje** – przyciski operacji  
- **Status / Błąd** – aktualny stan operacji (np. Gotowy)  

Każdy wiersz reprezentuje jedno urządzenie.

---

## Przyciski akcji (w tabeli)

W kolumnie **Akcje** dostępne są trzy przyciski dla każdego urządzenia:
- **Włącz** (zielony) – uruchamia komputer (Wake on LAN)  
- **Wyłącz** (czerwony) – wyłącza komputer  
- **Podgląd** (niebieski) – uruchamia podgląd zdalnego pulpitu  

Kolorystyka przycisków ułatwia szybkie rozróżnienie funkcji:
- zielony → akcja pozytywna (uruchomienie)  
- czerwony → akcja zatrzymania  
- niebieski → podgląd / interakcja  

---

## Dolny panel sterowania

Na dole aplikacji znajduje się zestaw przycisków do operacji globalnych:
- **Wykryj komputery** – skanuje sieć i odświeża listę urządzeń  
- **Zaznacz wszystko** – zaznacza wszystkie urządzenia na liście  
- **Włącz wszystkie** – uruchamia wszystkie zaznaczone komputery  
- **Wyłącz wszystkie** – wyłącza wszystkie zaznaczone komputery  

Przyciski te umożliwiają szybkie zarządzanie wieloma urządzeniami jednocześnie.

---

## Styl wizualny

Interfejs wykorzystuje:
- ciemny motyw (dark mode)  
- kontrastowe kolory przycisków  
- czytelną tabelę z wyraźnym podziałem danych  

Dzięki temu aplikacja jest wygodna w użytkowaniu i nie męczy wzroku podczas dłuższej pracy.

---

# Funkcjonalności

## Podgląd zdalnego pulpitu

Aplikacja umożliwia zdalny podgląd ekranu wybranego komputera znajdującego się w tej samej sieci. Funkcja ta pozwala użytkownikowi na monitorowanie pracy drugiego urządzenia w czasie rzeczywistym bez konieczności fizycznego dostępu do niego.

Proces korzystania z funkcji wygląda następująco:
- aplikacja automatycznie wykrywa dostępne urządzenia w sieci lokalnej i wyświetla je w formie listy  
- użytkownik wybiera interesujące go urządzenie z listy  
- po kliknięciu przycisku **„Podgląd”** nawiązywane jest połączenie  
- na ekranie użytkownika wyświetlany jest aktualny obraz pulpitu zdalnego komputera  

Funkcjonalność ta jest szczególnie przydatna w przypadku zdalnej pomocy technicznej, nadzoru nad pracą użytkownika lub zarządzania wieloma stanowiskami jednocześnie.

---

## Zdalne włączanie i wyłączanie urządzeń

Aplikacja umożliwia zarządzanie stanem zasilania komputerów dostępnych w sieci poprzez ich zdalne włączanie oraz wyłączanie.

### Włączanie urządzeń

Funkcja wykorzystuje kabel sieciowy Ethernet, który pozwala na uruchomienie komputera poprzez wysłanie specjalnego sygnału sieciowego do wybranego urządzenia.

Działanie funkcji:
- użytkownik wybiera urządzenie z listy  
- klika przycisk **„Włącz”**  
- aplikacja wysyła sygnał sieciowy do wskazanego komputera  
- komputer zostaje uruchomiony, nawet jeśli był wcześniej wyłączony lub w trybie uśpienia  

Rozwiązanie to eliminuje konieczność fizycznego włączania urządzenia i umożliwia szybki dostęp do niego w dowolnym momencie.

---

### Wyłączanie urządzeń

Aplikacja pozwala również na zdalne wyłączenie wybranego komputera:
- użytkownik wybiera urządzenie z listy  
- klika przycisk **„Wyłącz”**  
- do urządzenia wysyłana jest komenda zamknięcia systemu  
- komputer zostaje bezpiecznie wyłączony  

Funkcja ta jest szczególnie przydatna w zarządzaniu energią oraz administracji wieloma komputerami w sieci.

---

# Błędy i rozwiązywanie problemów

Poniżej przedstawiono najczęstsze problemy, które mogą wystąpić podczas korzystania z aplikacji DesktopControl, wraz z ich możliwymi przyczynami oraz sposobami rozwiązania.

---

## Problem: Brak wykrytych urządzeń

**Możliwe przyczyny:**
- komputer nie jest podłączony do tej samej sieci  
- brak połączenia sieciowego  

**Rozwiązanie:**
- upewnij się, że oba komputery znajdują się w tej samej sieci lokalnej (LAN)  
- sprawdź połączenie z Internetem / routerem  
- kliknij ponownie przycisk **„Wykryj komputery”**  

---

## Problem: Nie działa podgląd pulpitu

**Możliwe przyczyny:**
- blokada połączenia przez zaporę systemową  
- brak komunikacji między urządzeniami  

**Rozwiązanie:**
- upewnij się, że oba komputery są w tej samej sieci lokalnej  
- wyłącz zaporę systemową (firewall) lub dodaj wyjątek dla aplikacji  
- sprawdź, czy urządzenie docelowe jest włączone i dostępne  

---

## Problem: Nie można włączyć komputera (Wake on LAN)

**Możliwe przyczyny:**
- urządzenie nie obsługuje Wake on LAN  
- funkcja jest wyłączona w BIOS/UEFI  
- błędna konfiguracja sieci  

**Rozwiązanie:**
- sprawdź, czy Wake on LAN jest włączony w BIOS/UEFI  
- upewnij się, że karta sieciowa obsługuje tę funkcję  
- sprawdź poprawność adresu MAC urządzenia  

---

## Problem: Nie można wyłączyć komputera

**Możliwe przyczyny:**
- brak uprawnień  
- brak połączenia z urządzeniem  

**Rozwiązanie:**
- upewnij się, że komputer docelowy jest dostępny w sieci  
- sprawdź połączenie sieciowe  
- spróbuj ponownie wykonać operację  
