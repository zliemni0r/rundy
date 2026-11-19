using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace notatnik;

public partial class MainWindow : Window
{
    private readonly ObservableCollection<Note> _allNotes = new();
    private readonly ObservableCollection<Note> _filteredNotes = new();
    private Note? _currentSelectedNote;
    private bool _isUpdatingSelection;

    public MainWindow()
    {
        InitializeComponent();

        NotesListBox.ItemsSource = _filteredNotes;

        var storagePath = NotesStorage.GetStorageFilePath();
        FilePathTextBlock.Text = Path.GetFileName(storagePath);
        ToolTip.SetTip(FilePathTextBlock, storagePath);

        LoadNotesFromDisk();
    }

    private void LoadNotesFromDisk()
    {
        _allNotes.Clear();
        var loaded = NotesStorage.LoadNotes();
        foreach (var note in loaded.OrderByDescending(n => n.UpdatedAt))
        {
            _allNotes.Add(note);
        }

        ApplyFilter();

        if (_filteredNotes.Count > 0)
        {
            NotesListBox.SelectedItem = _filteredNotes[0];
            StatusTextBlock.Text = $"Wczytano {_allNotes.Count} notatek z pliku";
        }
        else
        {
            SetEditorToNewNoteMode();
            StatusTextBlock.Text = "Brak zapisanych notatek. Utwórz pierwszą!";
        }

        UpdateNotesCountBadge();
    }

    private void ApplyFilter()
    {
        var query = SearchTextBox?.Text?.Trim() ?? string.Empty;
        var selectedBefore = NotesListBox?.SelectedItem as Note;

        _filteredNotes.Clear();

        var matching = string.IsNullOrEmpty(query)
            ? _allNotes
            : _allNotes.Where(n =>
                (n.Title?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (n.Content?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false));

        foreach (var note in matching)
        {
            _filteredNotes.Add(note);
        }

        if (selectedBefore != null && _filteredNotes.Contains(selectedBefore))
        {
            NotesListBox!.SelectedItem = selectedBefore;
        }

        UpdateNotesCountBadge();
    }

    private void UpdateNotesCountBadge()
    {
        if (NotesCountBadge != null)
        {
            NotesCountBadge.Text = $"({_filteredNotes.Count})";
        }

        if (ListFooterTextBlock != null)
        {
            ListFooterTextBlock.Text = _allNotes.Count == 0
                ? "Brak notatek w pliku"
                : $"Łącznie: {_allNotes.Count} | Widoczne: {_filteredNotes.Count}";
        }
    }

    private void OnNotesSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_isUpdatingSelection) return;

        if (NotesListBox.SelectedItem is Note selectedNote)
        {
            _currentSelectedNote = selectedNote;
            _isUpdatingSelection = true;

            TitleTextBox.Text = selectedNote.Title;
            ContentTextBox.Text = selectedNote.Content;

            EditorModeTextBlock.Text = "Edycja notatki";
            LastModifiedTextBlock.Text = $"Ostatnia modyfikacja: {selectedNote.FormattedDate}";
            DeleteButton.IsVisible = true;

            _isUpdatingSelection = false;

            UpdateCounters();
            StatusTextBlock.Text = $"Wybrano notatkę: \"{selectedNote.DisplayTitle}\"";
        }
        else
        {
            if (_filteredNotes.Count == 0)
            {
                SetEditorToNewNoteMode();
            }
        }
    }

    private void OnSaveClick(object? sender, RoutedEventArgs e)
    {
        var title = TitleTextBox.Text?.Trim() ?? string.Empty;
        var content = ContentTextBox.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(content))
        {
            StatusTextBlock.Text = "⚠️ Wprowadź tytuł lub treść przed zapisaniem.";
            TitleTextBox.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            var firstLine = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            title = !string.IsNullOrEmpty(firstLine)
                ? (firstLine.Length > 30 ? firstLine.Substring(0, 27) + "..." : firstLine)
                : "Bez tytułu";
            TitleTextBox.Text = title;
        }

        bool isNew = false;
        Note targetNote;

        if (_currentSelectedNote != null && _allNotes.Contains(_currentSelectedNote))
        {
            targetNote = _currentSelectedNote;
            targetNote.Title = title;
            targetNote.Content = ContentTextBox.Text ?? string.Empty;
            targetNote.UpdatedAt = DateTime.Now;

            // Re-sort order to bring updated note to top
            _allNotes.Remove(targetNote);
            _allNotes.Insert(0, targetNote);
        }
        else
        {
            isNew = true;
            targetNote = new Note
            {
                Title = title,
                Content = ContentTextBox.Text ?? string.Empty,
                UpdatedAt = DateTime.Now
            };
            _allNotes.Insert(0, targetNote);
            _currentSelectedNote = targetNote;
        }

        // Zapis do pliku
        try
        {
            NotesStorage.SaveNotes(_allNotes);

            ApplyFilter();

            _isUpdatingSelection = true;
            NotesListBox.SelectedItem = targetNote;
            _isUpdatingSelection = false;

            EditorModeTextBlock.Text = "Edycja notatki";
            LastModifiedTextBlock.Text = $"Zapisano: {targetNote.FormattedDate}";
            DeleteButton.IsVisible = true;

            StatusTextBlock.Text = isNew
                ? $"✅ Dodano nową notatkę i zapisano do pliku ({DateTime.Now:HH:mm:ss})"
                : $"✅ Zaktualizowano notatkę i zapisano do pliku ({DateTime.Now:HH:mm:ss})";
        }
        catch (Exception ex)
        {
            StatusTextBlock.Text = $"❌ Błąd podczas zapisu do pliku: {ex.Message}";
        }
    }

    private void OnDeleteClick(object? sender, RoutedEventArgs e)
    {
        if (_currentSelectedNote == null) return;

        var noteToRemove = _currentSelectedNote;
        _allNotes.Remove(noteToRemove);

        try
        {
            NotesStorage.SaveNotes(_allNotes);
            ApplyFilter();
            SetEditorToNewNoteMode();
            StatusTextBlock.Text = $"🗑️ Usunięto notatkę \"{noteToRemove.DisplayTitle}\" z pliku";
        }
        catch (Exception ex)
        {
            StatusTextBlock.Text = $"❌ Błąd podczas usuwania: {ex.Message}";
        }
    }

    private void OnNewNoteClick(object? sender, RoutedEventArgs e)
    {
        SetEditorToNewNoteMode();
        StatusTextBlock.Text = "Tworzenie nowej notatki";
    }

    private void OnClearClick(object? sender, RoutedEventArgs e)
    {
        TitleTextBox.Text = string.Empty;
        ContentTextBox.Text = string.Empty;
        UpdateCounters();
        StatusTextBlock.Text = "Wyczyszczono formularz";
    }

    private void SetEditorToNewNoteMode()
    {
        _isUpdatingSelection = true;
        NotesListBox.SelectedItem = null;
        _currentSelectedNote = null;

        TitleTextBox.Text = string.Empty;
        ContentTextBox.Text = string.Empty;

        EditorModeTextBlock.Text = "Nowa notatka";
        LastModifiedTextBlock.Text = "Wprowadź tytuł i treść, a następnie kliknij Zapisz";
        DeleteButton.IsVisible = false;

        _isUpdatingSelection = false;

        UpdateCounters();
        TitleTextBox.Focus();
    }

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        ApplyFilter();
    }

    private void OnEditorFieldsChanged(object? sender, TextChangedEventArgs e)
    {
        // Reserved for auto-dirty checks if needed
    }

    private void OnContentTextChanged(object? sender, TextChangedEventArgs e)
    {
        UpdateCounters();
    }

    private void UpdateCounters()
    {
        if (EditorCountersTextBlock == null) return;

        var text = ContentTextBox?.Text ?? string.Empty;
        var charCount = text.Length;
        var wordCount = text.Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;

        EditorCountersTextBlock.Text = $"Znaków: {charCount} | Słów: {wordCount}";
    }
}
