using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Scribe.Systems;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;

namespace Scribe.Views;

public partial class EditorWindow : Window
{
    private ObservableCollection<Chapter> Chapters { get; } = new();
    private Chapter CurrentChapter { get; set; }
    // Editor is the Editor AvaloniaEdit control, defined in EditorWindow.axaml
    public EditorWindow()
    {
        InitializeComponent();

        this.Opened += OnOpened;
        this.Closed += OnClosed;
        // Add initial chapter
        var chapter1 = new Chapter { Title = "Chapter 1" , Content = "Chapter 1 Content"};
        Chapters.Add(chapter1);
        LoadChapter(chapter1);
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        var mainWindow = new MainWindow();
        mainWindow.Show();
    }
    private void LoadChapter(Chapter chapter)
    {
        CurrentChapter = chapter;
        Editor.Text = "";
        CurrentChapter = chapter;
        Editor.Text = chapter.Content;
    }
    private void SaveCurrentChapter()
    {
        if (CurrentChapter != null)
            CurrentChapter.Content = Editor.Text;
    }
    public void NewChapter_Click(object? sender, RoutedEventArgs e)
    {
        SaveCurrentChapter();

        var newChapter = new Chapter { Title = $"Chapter {Chapters.Count + 1}" };
        Chapters.Add(newChapter);
        LoadChapter(newChapter);
        
        PopulateChaptersMenu();
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        Editor.Focus();
        Editor.TextArea.Focus();

        PopulateChaptersMenu();
    }
    
    private void SaveToScribe(string filePath)
    {
        SaveCurrentChapter();

        var tempDir = Path.Combine(Path.GetTempPath(), "scribe_temp");

        if (Directory.Exists(tempDir))
            Directory.Delete(tempDir, true);

        Directory.CreateDirectory(tempDir);
        Directory.CreateDirectory(Path.Combine(tempDir, "chapters"));

        // Create book metadata
        var book = new Book
        {
            Chapters = Chapters.ToList()
        };

        // Write chapter files
        for (int i = 0; i < book.Chapters.Count; i++)
        {
            var chapter = book.Chapters[i];
            chapter.FileName = $"chapter{i + 1}.rtf";

            var chapterPath = Path.Combine(tempDir, "chapters", chapter.FileName);
            File.WriteAllText(chapterPath, chapter.Content);
        }

        // Write book.json
        var json = JsonSerializer.Serialize(book, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(Path.Combine(tempDir, "book.json"), json);

        // Zip it
        if (File.Exists(filePath))
            File.Delete(filePath);

        ZipFile.CreateFromDirectory(tempDir, filePath);

        // Cleanup
        Directory.Delete(tempDir, true);
    }
    
    private void LoadFromScribe(string filePath)
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "scribe_temp_load");

        if (Directory.Exists(tempDir))
            Directory.Delete(tempDir, true);

        ZipFile.ExtractToDirectory(filePath, tempDir);

        var jsonPath = Path.Combine(tempDir, "book.json");
        var json = File.ReadAllText(jsonPath);

        var book = JsonSerializer.Deserialize<Book>(json);

        Chapters.Clear();

        foreach (var chapter in book.Chapters)
        {
            var chapterPath = Path.Combine(tempDir, "chapters", chapter.FileName);
            chapter.Content = File.ReadAllText(chapterPath);
            Chapters.Add(chapter);
        }

        if (Chapters.Count > 0)
            LoadChapter(Chapters[0]);

        Directory.Delete(tempDir, true);
        
        
    }
    
    private async void Save_Click(object? sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog
        {
            Filters = new List<FileDialogFilter>
            {
                new FileDialogFilter { Name = "Scribe Files", Extensions = { "scribe" } }
            }
        };

        var path = await dialog.ShowAsync(this);

        if (path != null)
            SaveToScribe(path);
    }

    private async void Open_Click(object? sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            AllowMultiple = false,
            Filters = new List<FileDialogFilter>
            {
                new FileDialogFilter { Name = "Scribe Files", Extensions = { "scribe" } }
            }
        };

        var result = await dialog.ShowAsync(this);

        if (result != null && result.Length > 0)
            LoadFromScribe(result[0]);
    }
    
    private void PopulateChaptersMenu()
    {
        var items = ChaptersMenu.Items; // This is an AvaloniaList<object>
        items.Clear();

        foreach (var chapter in Chapters)
        {
            var menuItem = new MenuItem
            {
                Header = chapter.Title,
                Tag = chapter
            };

            menuItem.Click += ChapterMenuItem_Click;

            items.Add(menuItem);
        }
    }

    private void ChapterMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is MenuItem mi && mi.Tag is Chapter chapter)
        {
            SaveCurrentChapter();
            LoadChapter(chapter);
        }
    }




}