# Aplikacja Notatnik (Runda 3)

Aplikacja desktopowa przygotowana w technologii **C# / .NET 10** oraz frameworku **Avalonia UI**.

## Format pliku notatek (`notatki.md`)
Wszystkie notatki są zapisywane do pliku **`notatki.md`** w folderze projektu dokładnie według poniższego schematu:

```markdown
## 2026-09-08 14:32 — Zakupy

mleko

chleb


## 2026-09-07 09:15 — Pomysł na projekt

minutnik z historią sesji
```

### Zasady formatowania
1. Każdy wpis rozpoczyna się nagłówkiem poziomu 2: `## YYYY-MM-DD HH:mm — Tytuł notatki`.
2. Pod nagłówkiem znajduje się jedna pusta linia przed treścią notatki.
3. Między kolejnymi wpisami znajdują się dwie puste linie.
4. Najnowsza notatka znajduje się zawsze na samej górze pliku.
5. Wszystkie polskie znaki diakrytyczne są zachowane (kodowanie UTF-8 bez BOM).

## Uruchomienie aplikacji
```powershell
dotnet run --project .\notatnik\notatnik.csproj
```
lub bezpośrednio:
```powershell
.\bin\Debug\net10.0\notatnik.exe
```
