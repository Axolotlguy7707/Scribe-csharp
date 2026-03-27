using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Scribe.Systems;
using System.Collections.ObjectModel;

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
        var chapter1 = new Chapter { Title = "Chapter 1" };
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
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        Editor.Focus();
        Editor.TextArea.Focus();
    }
}