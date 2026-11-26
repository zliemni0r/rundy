# Aplikacja Notatnik (Runda 4)

Aplikacja desktopowa przygotowana w technologii **C# / .NET 10** oraz frameworku **Avalonia UI**.

## Trzy zróżnicowane przykłady w `notatki.md`

W pliku **`notatki.md`** znajdują się trzy celowo różniące się od siebie wpisy testowe:

```markdown
## 2026-09-09 18:45 — Plan działania – zażółć gęślą jaźń

Krótka notatka weryfikująca obsługę polskich znaków (ą, ć, ę, ł, ń, ó, ś, ź, ż) oraz myślnika w tytule wpisu.


## 2026-09-08 14:32 — Zakupy na weekend

mleko

chleb

masło i jajka


## 2026-09-07 09:15 — Pomysł na projekt

minutnik z historią sesji
```

### Charakterystyka wpisów
1. **Wpis z polskimi znakami i myślnikiem w tytule** (`2026-09-09 18:45`):
   - Tytuł: `Plan działania – zażółć gęślą jaźń` zawiera myślnik en-dash `–` oraz litery diakrytyczne `ł, ń, ż, ó, ł, ć, g, ę, ś, l, ą, j, a, ź, ń`.
   - Parser poprawnie oddziela pierwszy separator daty (` — `) od myślnika w tytule.
2. **Wpis wielolinijkowy** (`2026-09-08 14:32`):
   - Treść zawiera kilka linii rozdzielonych pustymi wierszami (pozycje listy zakupów).
3. **Wpis jednolinijkowy** (`2026-09-07 09:15`):
   - Zwięzła, jednolinijkowa treść notatki (`minutnik z historią sesji`).

### Zasady formatowania
- Nagłówek: `## YYYY-MM-DD HH:mm — Tytuł notatki`,
- Pusta linia po nagłówku przed treścią,
- Dwie puste linie odstępu między wpisami,
- Najnowsza notatka na samej górze pliku,
- Kodowanie UTF-8 bez BOM.

## Uruchomienie aplikacji
```powershell
dotnet run --project .\notatnik\notatnik.csproj
```
lub bezpośrednio:
```powershell
.\bin\Debug\net10.0\notatnik.exe
```
