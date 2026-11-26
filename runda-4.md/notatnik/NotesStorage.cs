using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace notatnik;

public static class NotesStorage
{
    public const string StorageFileName = "notatki.md";

    public static string GetStorageFilePath()
    {
        try
        {
            var baseDir = AppContext.BaseDirectory;
            var dir = new DirectoryInfo(baseDir);
            while (dir != null)
            {
                if (File.Exists(Path.Combine(dir.FullName, "notatnik.csproj")))
                {
                    return Path.Combine(dir.FullName, StorageFileName);
                }
                dir = dir.Parent;
            }
        }
        catch
        {
            // fallback if directory traversal fails
        }

        return Path.Combine(AppContext.BaseDirectory, StorageFileName);
    }

    public static List<Note> LoadNotes()
    {
        var filePath = GetStorageFilePath();

        if (!File.Exists(filePath))
        {
            var sampleNotes = new List<Note>
            {
                new()
                {
                    Title = "Plan działania – zażółć gęślą jaźń",
                    Content = "Krótka notatka weryfikująca obsługę polskich znaków (ą, ć, ę, ł, ń, ó, ś, ź, ż) oraz myślnika w tytule wpisu.",
                    UpdatedAt = new DateTime(2026, 9, 9, 18, 45, 0)
                },
                new()
                {
                    Title = "Zakupy na weekend",
                    Content = "mleko\n\nchleb\n\nmasło i jajka",
                    UpdatedAt = new DateTime(2026, 9, 8, 14, 32, 0)
                },
                new()
                {
                    Title = "Pomysł na projekt",
                    Content = "minutnik z historią sesji",
                    UpdatedAt = new DateTime(2026, 9, 7, 9, 15, 0)
                }
            };
            SaveNotes(sampleNotes);
            return sampleNotes;
        }

        var notes = new List<Note>();
        try
        {
            var lines = File.ReadAllLines(filePath, Encoding.UTF8);
            Note? currentNote = null;
            var contentLines = new List<string>();

            void FlushCurrent()
            {
                if (currentNote != null)
                {
                    // Usuń puste linie z początku (np. pojedynczą pustą linię pod nagłówkiem)
                    while (contentLines.Count > 0 && string.IsNullOrWhiteSpace(contentLines[0]))
                    {
                        contentLines.RemoveAt(0);
                    }
                    // Usuń puste linie z końca (odstępy między wpisami)
                    while (contentLines.Count > 0 && string.IsNullOrWhiteSpace(contentLines[^1]))
                    {
                        contentLines.RemoveAt(contentLines.Count - 1);
                    }
                    currentNote.Content = string.Join(Environment.NewLine, contentLines);
                    notes.Add(currentNote);
                    contentLines.Clear();
                    currentNote = null;
                }
            }

            foreach (var line in lines)
            {
                if (line.StartsWith("## "))
                {
                    FlushCurrent();

                    var header = line.Substring(3).Trim();
                    // Oczekiwany format: ## YYYY-MM-DD HH:mm — Tytuł notatki
                    // Obsługa separatora (em-dash, en-dash, dywiz) przy jednoczesnym zachowaniu myślników w samym tytule
                    var match = Regex.Match(header, @"^(\d{4}-\d{2}-\d{2}\s+\d{2}:\d{2})\s*[—–-]\s*(.*)$");
                    if (match.Success)
                    {
                        var dateStr = match.Groups[1].Value.Trim();
                        var titleStr = match.Groups[2].Value.Trim();

                        var date = DateTime.TryParseExact(dateStr, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt)
                            ? dt
                            : DateTime.Now;

                        currentNote = new Note
                        {
                            Title = titleStr,
                            UpdatedAt = date
                        };
                    }
                    else
                    {
                        currentNote = new Note
                        {
                            Title = header,
                            UpdatedAt = DateTime.Now
                        };
                    }
                }
                else
                {
                    if (currentNote != null)
                    {
                        contentLines.Add(line);
                    }
                }
            }

            FlushCurrent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas odczytu pliku {StorageFileName}: {ex.Message}");
        }

        return notes.OrderByDescending(n => n.UpdatedAt).ToList();
    }

    public static void SaveNotes(IEnumerable<Note> notes)
    {
        var filePath = GetStorageFilePath();
        var dir = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        // Najnowsza notatka na górze pliku
        var sorted = notes.OrderByDescending(n => n.UpdatedAt).ToList();
        var sb = new StringBuilder();

        for (int i = 0; i < sorted.Count; i++)
        {
            var note = sorted[i];
            var dateStr = note.UpdatedAt.ToString("yyyy-MM-dd HH:mm");

            // Nagłówek wpisu
            sb.AppendLine($"## {dateStr} — {note.Title}");

            // Pusta linia pod nagłówkiem
            sb.AppendLine();

            // Treść notatki
            if (!string.IsNullOrEmpty(note.Content))
            {
                sb.AppendLine(note.Content.Trim('\r', '\n'));
            }

            // Dwie puste linie między wpisami
            if (i < sorted.Count - 1)
            {
                sb.AppendLine();
                sb.AppendLine();
            }
        }

        // Zapis w UTF-8 bez BOM (poprawne polskie znaki diakrytyczne)
        File.WriteAllText(filePath, sb.ToString(), new UTF8Encoding(false));
    }
}
