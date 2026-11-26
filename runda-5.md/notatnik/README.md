# Aplikacja Notatnik (Runda 5)

Aplikacja desktopowa przygotowana w technologii **C# / .NET 10** oraz frameworku **Avalonia UI** na podstawie uściślonej specyfikacji wymagań.

## 1. Interfejs użytkownika
- **Lewy panel**: lista zapisanych notatek (`NotesListBox`) z tytułem, datą i podglądem. Kliknięcie na pozycję wczytuje notatkę do formularza po prawej stronie.
- **Prawy panel**:
  - Jednolinijkowe pole na tytuł (`TitleTextBox`),
  - Wielolinijkowe pole na treść (`ContentTextBox` z obsługą Entera, zawijania wierszy i licznikiem znaków/słów),
  - Przycisk **„💾 Zapisz”** – dodaje nową notatkę lub aktualizuje istniejącą,
  - Przycisk **„➕ Nowa notatka”** – czyści formularz pod nowy wpis,
  - Przycisk **„🗑️ Usuń”** – usuwa wybraną notatkę.

## 2. Plik notatek (`notatki.md`)
Wszystkie notatki są utrwalane w jednym pliku **`notatki.md`** w folderze projektu:
- Kodowanie: **UTF-8 bez BOM**,
- Kolejność: najnowsza notatka zawsze na samej górze pliku,
- Automatyczne wczytanie zawartości przy starcie aplikacji.

### Format zapisu wpisu:
```markdown
## YYYY-MM-DD HH:mm — Tytuł notatki

treść notatki
```
- Dokładnie 1 pusta linia po nagłówku przed treścią,
- Dokładnie 2 puste linie między osobnymi wpisami.

### Przykładowa zawartość pliku:
```markdown
## 2026-09-09 18:45 — Plan działania – zażółć gęślą jaźń

Krótka notatka weryfikująca obsługę polskich znaków oraz myślnika w tytule wpisu.


## 2026-09-08 14:32 — Zakupy na weekend

mleko

chleb


## 2026-09-07 09:15 — Pomysł na projekt

minutnik z historią sesji
```

## 3. Uruchomienie aplikacji
```powershell
dotnet run --project .\notatnik\notatnik.csproj
```
lub bezpośrednio z pliku wykonywalnego:
```powershell
.\bin\Debug\net10.0\notatnik.exe
```
