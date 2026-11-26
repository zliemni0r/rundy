using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace notatnik;

public partial class MainWindow : Window
{
    private readonly ObservableCollection<Note> _notes = new();
    private Note? _selectedNote;

    public MainWindow()
    {
        InitializeComponent();
        NotesList.ItemsSource = _notes;
        LoadData();
    }

    private void LoadData()
    {
        _notes.Clear();
        foreach (var note in NotesStorage.LoadNotes())
        {
            _notes.Add(note);
        }
    }

    private void OnNoteSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (NotesList.SelectedItem is Note note)
        {
            _selectedNote = note;
            TitleBox.Text = note.Title;
            ContentBox.Text = note.Content;
        }
    }

    private void OnSaveClick(object? sender, RoutedEventArgs e)
    {
        var title = TitleBox.Text?.Trim() ?? string.Empty;
        var content = ContentBox.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(content))
            return;

        if (_selectedNote != null && _notes.Contains(_selectedNote))
        {
            _selectedNote.Title = title;
            _selectedNote.Content = content;
        }
        else
        {
            var newNote = new Note { Title = title, Content = content };
            _notes.Add(newNote);
            _selectedNote = newNote;
            NotesList.SelectedItem = newNote;
        }

        NotesStorage.SaveNotes(_notes);

        var idx = NotesList.SelectedIndex;
        NotesList.ItemsSource = null;
        NotesList.ItemsSource = _notes;
        NotesList.SelectedIndex = idx;
    }
}
