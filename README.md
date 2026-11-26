# Notatnik i cztery rundy próśb

Kompleksowy projekt aplikacji desktopowej **Notatnik** zrealizowany w języku **C# (.NET 10)** z użyciem frameworka **Avalonia UI**, rozwijany iteracyjnie poprzez kolejne rundy inżynierii promptów.

---

## 1. Uruchomienie aplikacji

Aplikację główną można uruchomić poleceniem:
```powershell
dotnet run --project .\notatnik\notatnik.csproj
```
lub bezpośrednio z pliku wykonywalnego:
```powershell
.\notatnik\bin\Debug\net10.0\notatnik.exe
```

---

## 2. Realizacja wymagań i warunków zadania

### Warunek 1: Przegląd zmian przed akceptacją
Każda zmiana w kodzie generowana przez agenta podlegała dokładnej analizie pod kątem poprawności składniowej XAML/C#, obsługi wątków UI, kodowania znaków oraz bezpieczeństwa operacji wejścia-wyjścia (I/O).

### Warunek 2: Historia commitów i kopie rund w repozytorium
Wszystkie etapy zadania posiadają commity w historii gita, a ich pełne kopie robocze znajdują się w podfolderach:
- `runda-1.md/` (commit `c5ec4a1: runda 1`) — pierwsza wersja notatnika na bazie ogólnej prośby.
- `runda-2.md/` (commit `fc0e012: runda 2` oraz `17bf29c: try i catch`) — przejście na plik `notatki.md`, format nagłówków oraz obsługa błędu zapisu.
- `runda-3.md/` (commit `d5054b9: runda 3`) — standaryzacja formatu wpisów na podstawie pojedynczego szablonu (few-shot).
- `runda-4.md/` (commit `34ebef2: runda 4`) — uodpornienie parsera na trzy zróżnicowane przypadki brzegowe.
- `runda-5.md/` (commit `5e6fbf2: runda 5`) — implementacja jednoznacznej prośby sformułowanej po analizie braków promptu.
- `notatnik/` — finalna, dopracowana wersja aplikacji.

---

### Warunek 3: Analiza powtórek dla wszystkich rund (`runda-X-powtorka.md`)

Każda runda została wykonana powtórnie z identyczną prośbą, a wyniki zachowano w odpowiadających folderach:

| Runda | Oryginał | Powtórka | Kluczowe różnice między wykonaniami |
| :--- | :--- | :--- | :--- |
| **Runda 1** | `runda-1.md/` | `runda-1-powtorka.md/` | **Bardzo duże rozbieżności**: oryginał użył formatu `notatki.json` z GUID i czasem modyfikacji, wyszukiwarką i przyciskami na górze; powtórka użyła płaskiego pliku `notatki.txt` z separatorem `---`, prostego modelu DTO i przycisku zapisu na dole. |
| **Runda 2** | `runda-2.md/` | `runda-2-powtorka.md/` | **Średnie rozbieżności**: oba rozwiązania zapisały notatki do `notatki.md` z nagłówkiem `##`, ale różniły się implementacją parsera (zaawansowany Regex z obsługą myślników vs prosty podział tekstu) oraz sposobem lokalizacji katalogu projektu. |
| **Runda 3** | `runda-3.md/` | `runda-3-powtorka.md/` | **Małe rozbieżności**: dzięki dołączeniu wzorcowego pliku (few-shot) oba wykonania wygenerowały identyczny układ odstępów (1 linia pod nagłówkiem, 2 linie między wpisami); różnice ograniczyły się do szczegółów trimowania linii. |
| **Runda 4** | `runda-4.md/` | `runda-4-powtorka.md/` | **Minimalne rozbieżności**: oba wykonania poprawnie poradziły sobie ze wszystkimi 3 przypadkami brzegowymi (wpis 1-linijkowy, wielolinijkowy, polskie znaki z myślnikiem w tytule); różniła się jedynie treść przykładowych notatek demonstracyjnych. |
| **Runda 5** | `runda-5.md/` | `runda-5-powtorka.md/` | **Brak istotnych rozbieżności**: ścisła specyfikacja sprawiła, że oba warianty mają identyczną architekturę, układ GUI, model danych, blok `try-catch` i schemat pliku `notatki.md`. |

#### Wpływ zjawiska stochastyczności:
> Porównanie powtórek wszystkich pięciu rund dowodzi, że im bardziej ogólny był prompt (jak w rundzie 1), tym większa część różnic w architekturze i formatach wynikała z czystej losowości modelu, natomiast stopniowe doprecyzowywanie wymagań i dodawanie przykładów (rundy 2–5) zredukowało tę wariancję niemal do zera, czyniąc wyniki w pełni powtarzalnymi.

---

### Warunek 4: Zalecenie producenta modelu (Google / Gemini)

- **Rada producenta**: Stosowanie znaczników strukturalnych i separatorów (np. tagów XML `<zasady>`, `<format_wyjsciowy>`, `<przyklady>`) w celu jednoznacznego rozdzielenia instrukcji systemowych, danych wejściowych i przykładów demonstracyjnych (*Use delimiters and XML tags to structure the prompt*).
- **Link do dokumentacji**: [https://ai.google.dev/gemini-api/docs/prompting-strategies#use-delimiters](https://ai.google.dev/gemini-api/docs/prompting-strategies#use-delimiters)
- **Data odczytania**: 2026-11-26
- **Wpływ na notatnik**:
  > Zastosowanie tagów XML oddzielających specyfikację formatu od przykładowych notatek zapobiegło myleniu przez model znaków myślnika i pustych linii występujących wewnątrz przykładowych treści z separatorem nagłówka, gwarantując bezbłędne wygenerowanie parsera i serializatora.

---

## 3. Cechy ukończonej aplikacji (`notatnik/`)

1. **Format zapisu Markdown (zgodny z Rundą 4)**:
   - Nagłówek: `## YYYY-MM-DD HH:mm — Tytuł notatki` (separator: spacja, em-dash `—`, spacja),
   - Dokładnie 1 pusta linia bezpośrednio pod nagłówkiem,
   - Dokładnie 2 puste linie odstępu między osobnymi wpisami.
2. **Polskie znaki**: kodowanie `UTF-8 bez BOM` w zapisie i odczycie zapobiega uszkodzeniom znaków `ą, ć, ę, ł, ń, ó, ś, ź, ż`.
3. **Sortowanie**: najnowsza notatka znajduje się zawsze na samej górze pliku (`OrderByDescending(n => n.UpdatedAt)`).
4. **Stabilność danych**: brak limitu wpisów (żadne notatki nie znikają po 5., 10. ani kolejnych zapisach).
5. **Obsługa błędów („Własną ręką: obsługa błędu”)**:
   - Kontrolka `<TextBlock Name="StatusText" />` znajduje się pod przyciskiem zapisu,
   - Blok `try-catch` przechwytuje wyjątki wejścia-wyjścia (np. plik tylko do odczytu / brak uprawnień) i informuje użytkownika komunikatem `Nie udało się zapisać: {ex.Message}`, zapobiegając nagłemu wyłączeniu programu i utracie wpisywanego tekstu.
