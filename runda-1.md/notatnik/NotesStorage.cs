using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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
        var projectDir = Path.GetDirectoryName(filePath) ?? AppContext.BaseDirectory;

        if (!File.Exists(filePath))
        {
            // Migracja z dotychczasowego notatki.json, jeśli istnieje
            var jsonPath = Path.Combine(projectDir, "notatki.json");
            if (File.Exists(jsonPath))
            {
                try
                {
                    var json = File.ReadAllText(jsonPath, Encoding.UTF8);
                    var oldNotes = JsonSerializer.Deserialize<List<Note>>(json);
                    if (oldNotes != null && oldNotes.Count > 0)
                    {
                        SaveNotes(oldNotes);
                        return oldNotes.OrderByDescending(n => n.UpdatedAt).ToList();
                    }
                }
                catch
                {
                    // W razie błędu parsowania JSON przejdź do wartości domyślnej
                }
            }

            var sampleNotes = new List<Note>
            {
                new()
                {
                    Title = "Przykładowa notatka",
                    Content = "Witaj w Notatniku!\nNotatki są zapisywane w jednym pliku notatki.md w folderze projektu.\nNajnowsze notatki znajdują się na górze pliku.",
                    UpdatedAt = DateTime.Now
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
                    // Obsługa myślnika em-dash (—), en-dash (–) oraz dywizu (-)
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
            sb.AppendLine($"## {dateStr} — {note.Title}");
            if (!string.IsNullOrEmpty(note.Content))
            {
                sb.AppendLine(note.Content);
            }
            if (i < sorted.Count - 1)
            {
                sb.AppendLine(); // Pusta linia między wpisami
            }
        }

        // Polskie znaki mają zostać polskimi znakami (kodowanie UTF-8 bez BOM)
        File.WriteAllText(filePath, sb.ToString(), new UTF8Encoding(false));
    }
}
