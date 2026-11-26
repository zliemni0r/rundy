using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace notatnik;

public static class NotesStorage
{
    public static string GetFilePath()
    {
        try
        {
            var baseDir = AppContext.BaseDirectory;
            var dir = new DirectoryInfo(baseDir);
            while (dir != null)
            {
                if (File.Exists(Path.Combine(dir.FullName, "notatnik.csproj")))
                {
                    return Path.Combine(dir.FullName, "notatki.txt");
                }
                dir = dir.Parent;
            }
        }
        catch { }

        return Path.Combine(AppContext.BaseDirectory, "notatki.txt");
    }

    public static List<Note> LoadNotes()
    {
        var filePath = GetFilePath();
        var list = new List<Note>();

        if (!File.Exists(filePath))
        {
            var sample = new List<Note>
            {
                new() { Title = "Moja notatka", Content = "Treść pierwszej notatki." }
            };
            SaveNotes(sample);
            return sample;
        }

        try
        {
            var text = File.ReadAllText(filePath, Encoding.UTF8);
            var blocks = text.Split(new[] { "\n---\n", "\r\n---\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var block in blocks)
            {
                var lines = block.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length > 0)
                {
                    var title = lines[0].StartsWith("Tytuł: ") ? lines[0].Substring(7) : lines[0];
                    var content = lines.Length > 1 ? string.Join(Environment.NewLine, lines, 1, lines.Length - 1) : string.Empty;
                    list.Add(new Note { Title = title.Trim(), Content = content.Trim() });
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd odczytu: {ex.Message}");
        }

        return list;
    }

    public static void SaveNotes(IEnumerable<Note> notes)
    {
        var filePath = GetFilePath();
        var sb = new StringBuilder();
        foreach (var note in notes)
        {
            sb.AppendLine($"Tytuł: {note.Title}");
            if (!string.IsNullOrEmpty(note.Content))
            {
                sb.AppendLine(note.Content);
            }
            sb.AppendLine("---");
        }
        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
    }
}
