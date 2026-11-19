using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace notatnik;

public class Note : INotifyPropertyChanged
{
    private Guid _id = Guid.NewGuid();
    private string _title = string.Empty;
    private string _content = string.Empty;
    private DateTime _updatedAt = DateTime.Now;

    public Guid Id
    {
        get => _id;
        set => SetField(ref _id, value);
    }

    public string Title
    {
        get => _title;
        set
        {
            if (SetField(ref _title, value))
            {
                OnPropertyChanged(nameof(DisplayTitle));
            }
        }
    }

    public string Content
    {
        get => _content;
        set
        {
            if (SetField(ref _content, value))
            {
                OnPropertyChanged(nameof(Preview));
            }
        }
    }

    public DateTime UpdatedAt
    {
        get => _updatedAt;
        set
        {
            if (SetField(ref _updatedAt, value))
            {
                OnPropertyChanged(nameof(FormattedDate));
            }
        }
    }

    [JsonIgnore]
    public string DisplayTitle => string.IsNullOrWhiteSpace(Title) ? "(Bez tytułu)" : Title;

    [JsonIgnore]
    public string FormattedDate => UpdatedAt.ToString("dd.MM.yyyy HH:mm");

    [JsonIgnore]
    public string Preview
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Content))
                return "(brak treści)";

            var singleLine = Content.Replace("\r", " ").Replace("\n", " ").Trim();
            return singleLine.Length > 75 ? singleLine.Substring(0, 72) + "..." : singleLine;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

