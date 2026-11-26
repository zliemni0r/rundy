# Aplikacja Notatnik (Runda 2)

Aplikacja desktopowa przygotowana w technologii **C# / .NET 10** oraz frameworku **Avalonia UI**.

## Zapis notatek (Format z Rundy 2)
Wszystkie notatki są zapisywane do jednego pliku **`notatki.md`** w folderze projektu w formacie:

```markdown
## YYYY-MM-DD HH:mm — Tytuł notatki
treść notatki, może mieć kilka linii
```

- Między wpisami znajduje się pusta linia,
- Najnowsza notatka znajduje się zawsze na górze pliku,
- Pełne wsparcie dla polskich znaków (kodowanie UTF-8).

## Funkcjonalności
1. **Pole na tytuł** (`TitleTextBox`) – wprowadzenie tytułu notatki.
2. **Pole na treść** (`ContentTextBox`) – wielowierszowe pole tekstowe z obsługą Entera i licznikiem znaków/słów.
3. **Przycisk zapisu** (`SaveButton`) – przycisk „💾 Zapisz”, który zapisuje nową notatkę lub aktualizuje zaznaczoną w pliku `notatki.md`.
4. **Lista zapisanych notatek** (`NotesListBox`) – lista po lewej stronie sortowana od najnowszej do najstarszej. Kliknięcie na pozycję wczytuje ją do edycji.
5. **Przycisk „➕ Nowa notatka”** i **„🗑️ Usuń”** – tworzenie nowej notatki i usuwanie wybranej z pliku `notatki.md`.
6. **Wyszukiwarka** – filtrowanie notatek po tytule i treści.

## Uruchomienie aplikacji
```powershell
dotnet run --project .\notatnik\notatnik.csproj
```
lub plikiem wykonywalnym:
```powershell
.\bin\Debug\net10.0\notatnik.exe
```
