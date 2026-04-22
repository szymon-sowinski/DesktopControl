# Wprowadzenie

Aplikacja DesktopControl jest aplikacją desktopową, której głównym celem jest umożliwienie zdalnej kontroli jednego komputera z poziomu drugiego urządzenia. Projekt został stworzony z myślą o użytkownikach, którzy potrzebują prostego i efektywnego sposobu zarządzania komputerem na odległość.  

System pozwala na nawiązanie połączenia między dwoma komputerami w tej samej sieci, dzięki czemu użytkownik może wykonywać operacje takie jak podgląd pulpitu, czy włączanie i wyłączanie komputera. Rozwiązanie to zwiększa wygodę pracy, umożliwia szybką pomoc techniczną oraz usprawnia zarządzanie wieloma stanowiskami z poziomu jednego urządzenia.  

Projekt DesktopControl został opracowany jako aplikacja o przejrzystym interfejsie i intuicyjnej obsłudze, tak aby nawet mniej zaawansowani użytkownicy mogli w łatwy sposób korzystać z jego funkcjonalności.  

---

# Wymagania systemowe

Aby aplikacja DesktopControl działała poprawnie i zapewniała stabilne połączenie zdalne między komputerami, system użytkownika powinien spełniać poniższe wymagania:

- **System operacyjny:** Windows 11  
- **Sieć:** aktywne połączenie z siecią lokalną (LAN)  

---

# Proces konfiguracji

Aby aplikacja DesktopControl działała poprawnie i umożliwiała zdalne zarządzanie urządzeniami, wymagane jest skonfigurowanie systemu w dwóch miejscach: na komputerze użytkownika (sterującym) oraz na komputerze zdalnym (klienckim).

Na urządzeniu zdalnym musi być uruchomiona specjalna aplikacja **agenta**. Jest ona odpowiedzialna za udostępnienie funkcji takich jak podgląd ekranu oraz zdalne zarządzanie urządzeniem. Bez aktywnego agenta urządzenie nie będzie widoczne jako w pełni obsługiwalne w systemie DesktopControl.

Aplikacja agenta działa w tle systemu jako proces nasłuchujący komunikacji sieciowej. Oznacza to, że po jej uruchomieniu nie jest widoczna jako standardowe okno aplikacji i nie wymaga dodatkowych działań ze strony użytkownika.

### Konfiguracja urządzenia zdalnego:
1. Uruchom aplikację agenta na komputerze docelowym  
2. Agent automatycznie uruchomi usługę nasłuchującą w sieci lokalnej  
3. Urządzenie pojawi się na liście w aplikacji DesktopControl  

Dopiero po poprawnym uruchomieniu agenta możliwe jest korzystanie z funkcji takich jak:
- podgląd pulpitu  
- zdalne włączanie i wyłączanie  
- blokowanie i odblokowywanie urządzenia  

---

# Interfejs użytkownika

Interfejs aplikacji DesktopControl został zaprojektowany w sposób przejrzysty i nowoczesny, z naciskiem na szybki dostęp do najważniejszych funkcji zarządzania urządzeniami w sieci.

---

## Górna część aplikacji

Na samej górze znajduje się pasek nagłówka zawierający:
- nazwę **Desktop Control**

Nagłówek pełni funkcję informacyjną i identyfikuje aplikację.

---

## Główna sekcja – lista urządzeń

Centralnym elementem interfejsu jest tabela prezentująca wykryte urządzenia w sieci lokalnej. Lista ta stanowi główny obszar roboczy aplikacji.

Urządzenia pojawiają się po kliknięciu przycisku **„Wykryj komputery”**.

Tabela zawiera następujące kolumny:
- **MAC Address** – adres fizyczny urządzenia  
- **IP Address** – adres IP w sieci  
- **Hostname** – nazwa komputera (jeśli dostępna)  
- **Online** – status dostępności urządzenia (True / False)  
- **Akcje** – zestaw przycisków operacji  
- **Status / Błąd** – informacje o stanie lub błędach  

Każdy wiersz reprezentuje jedno urządzenie.

---

## Przyciski akcji (w tabeli)

W kolumnie **Akcje** dostępne są przyciski umożliwiające bezpośrednią interakcję:

- **Włącz** (zielony) – uruchamia komputer (Wake on LAN)  
- **Wyłącz** (czerwony) – wyłącza komputer  
- **Podgląd** (niebieski) – uruchamia podgląd pulpitu  
- **Zablokuj / Odblokuj** (dynamiczny) – zarządza blokadą urządzenia  

### Dynamiczna blokada:
- gdy urządzenie jest aktywne → przycisk **„Zablokuj”**  
- po zablokowaniu → przycisk zmienia się na **„Odblokuj”**

### Kolorystyka:
- zielony → akcja pozytywna (start)  
- czerwony → zatrzymanie / wyłączenie  
- niebieski → podgląd / interakcja  
- blokada → zmienna (w zależności od stanu)

---

## Dolny panel sterowania

Na dole aplikacji znajduje się przycisk:
- **Wykryj komputery** – wyszukuje urządzenia w sieci  

---

## Styl wizualny

Interfejs aplikacji wykorzystuje:
- ciemny, gradientowy motyw (dark mode)  
- kontrastowe kolory przycisków  
- czytelną tabelę  

Zachowana jest spójność wizualna wszystkich elementów, co wpływa na profesjonalny wygląd aplikacji.

---

# Funkcjonalności

## Podgląd zdalnego pulpitu

Aplikacja umożliwia zdalny podgląd ekranu wybranego komputera w czasie rzeczywistym.

Proces działania:
1. Wykrycie urządzeń w sieci  
2. Wybór urządzenia  
3. Kliknięcie **„Podgląd”**  
4. Wyświetlenie pulpitu zdalnego  

Zastosowania:
- pomoc techniczna  
- monitoring pracy  
- zarządzanie wieloma stanowiskami  

---

## Zdalne włączanie i wyłączanie urządzeń

### Włączanie (Wake on LAN)

- wybór urządzenia  
- kliknięcie **„Włącz”**  
- wysłanie sygnału sieciowego  
- uruchomienie komputera  

---

### Wyłączanie

- wybór urządzenia  
- kliknięcie **„Wyłącz”**  
- wysłanie komendy zamknięcia  
- bezpieczne wyłączenie systemu  

---

## Zdalna blokada urządzenia

Aplikacja umożliwia zdalne zablokowanie komputera.

Proces:
- wybór urządzenia  
- kliknięcie **„Zablokuj”**  
- wysłanie komendy blokady  
- ekran zostaje zablokowany  

Zastosowania:
- zabezpieczenie stanowiska  
- kontrola dostępu  
- środowiska firmowe i szkolne  

---

# Błędy i rozwiązywanie problemów

---

## Problem: Brak wykrytych urządzeń

**Możliwe przyczyny:**
- brak tej samej sieci  
- brak połączenia  

**Rozwiązanie:**
- sprawdź sieć LAN  
- sprawdź połączenie z routerem  
- kliknij ponownie **„Wykryj komputery”**  

---

## Problem: Nie działa podgląd pulpitu

**Możliwe przyczyny:**
- firewall blokuje połączenie  
- brak komunikacji  

**Rozwiązanie:**
- sprawdź sieć lokalną  
- wyłącz firewall lub dodaj wyjątek  
- sprawdź dostępność urządzenia  

---

## Problem: Nie można włączyć komputera (Wake on LAN)

**Możliwe przyczyny:**
- brak wsparcia WOL  
- wyłączone w BIOS/UEFI  
- błędna konfiguracja  

**Rozwiązanie:**
- włącz WOL w BIOS/UEFI  
- sprawdź kartę sieciową  
- zweryfikuj adres MAC  

---

## Problem: Nie można wyłączyć komputera

**Możliwe przyczyny:**
- brak uprawnień  
- brak połączenia  

**Rozwiązanie:**
- sprawdź dostępność urządzenia  
- sprawdź sieć  
- spróbuj ponownie  

---
