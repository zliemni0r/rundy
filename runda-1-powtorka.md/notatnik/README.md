# Aplikacja Notatnik

Aplikacja desktopowa przygotowana w technologii **C# / .NET 10** oraz frameworku **Avalonia UI**.

## Zrealizowane funkcjonalności

1. **Pole na tytuł** (`TitleTextBox`) – umożliwia nadanie tytułu notatce.
2. **Pole na treść** (`ContentTextBox`) – wielowierszowe pole tekstowe z zawijaniem wierszy i obsługą klawisza Enter.
3. **Przycisk zapisu** (`SaveButton`) – przycisk „💾 Zapisz”, który zapisuje nową notatkę lub aktualizuje zaznaczoną.
4. **Lista zapisanych notatek** (`NotesListBox`) – lista boczna wyświetlająca zapisane notatki z tytułem, datą modyfikacji oraz podglądem treści. Kliknięcie na notatkę otwiera ją do podglądu i edycji.
5. **Zapis do pliku** – notatki są automatycznie wczytywane przy uruchomieniu aplikacji oraz trwale zapisywane do:
   - `notatki.json` – format strukturalny JSON,
   - `notatki.txt` – czytelny plik tekstowy.

## Dodatkowe udogodnienia
- Przycisk **„➕ Nowa notatka”** do czyszczenia formularza i tworzenia nowej notatki,
- Przycisk **„🗑️ Usuń”** do usuwania wybranej notatki (z automatycznym usunięciem z pliku),
- Wyszukiwarka notatek na bieżąco filtrująca listę,
- Licznik znaków i słów treści,
- Pasek stanu z informacją o dacie ostatniego zapisu i ścieżce do pliku.

## Uruchomienie aplikacji
Aplikację można uruchomić poleceniem:
```powershell
dotnet run --project notatnik
```
lub bezpośrednio uruchamiając plik:
```powershell
.\notatnik\bin\Debug\net10.0\notatnik.exe
```

