using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace notatnik;

public static class NotesStorage
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

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
                    return Path.Combine(dir.FullName, "notatki.json");
                }
                dir = dir.Parent;
            }
        }
        catch
        {
            // fallback if directory traversal fails
        }

        return Path.Combine(AppContext.BaseDirectory, "notatki.json");
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
                    Title = "Przykładowa notatka",
                    Content = "Witaj w Notatniku!\n\nTo jest Twoja pierwsza notatka. Możesz:\n1. Wybrać notatkę z listy po lewej stronie, aby ją przeczytać lub edytować.\n2. Wpisać tytuł i treść, a następnie kliknąć 'Zapisz'.\n3. Utworzyć nową notatkę przyciskiem '+ Nowa notatka'.\n\nWszystkie notatki są automatycznie zapisywane do pliku notatki.json.",
                    UpdatedAt = DateTime.Now
                }
            };
            SaveNotes(sampleNotes);
            return sampleNotes;
        }

        try
        {
            var json = File.ReadAllText(filePath, Encoding.UTF8);
            if (string.IsNullOrWhiteSpace(json))
                return new List<Note>();

            var notes = JsonSerializer.Deserialize<List<Note>>(json, JsonOptions);
            return notes ?? new List<Note>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas odczytu pliku: {ex.Message}");
            return new List<Note>();
        }
    }

    public static void SaveNotes(IEnumerable<Note> notes)
    {
        var filePath = GetStorageFilePath();
        var dir = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        var json = JsonSerializer.Serialize(notes, JsonOptions);
        File.WriteAllText(filePath, json, Encoding.UTF8);

        // Also save readable plain-text copy (notatki.txt)
        try
        {
            var txtPath = Path.ChangeExtension(filePath, ".txt");
            var sb = new StringBuilder();
            sb.AppendLine("=== ZAPISANE NOTATKI ===");
            sb.AppendLine($"Data aktualizacji: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine();

            foreach (var note in notes)
            {
                sb.AppendLine("----------------------------------------");
                sb.AppendLine($"TYTUŁ: {note.Title}");
                sb.AppendLine($"DATA:  {note.FormattedDate}");
                sb.AppendLine("TREŚĆ:");
                sb.AppendLine(note.Content);
                sb.AppendLine("----------------------------------------");
                sb.AppendLine();
            }

            File.WriteAllText(txtPath, sb.ToString(), Encoding.UTF8);
        }
        catch
        {
            // Silently ignore text backup failures if any
        }
    }
}

