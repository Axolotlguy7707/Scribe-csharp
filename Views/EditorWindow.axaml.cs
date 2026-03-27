using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Scribe.Views;

public partial class EditorWindow : Window
{
    // Editor is the Editor AvaloniaEdit control, defined in EditorWindow.axaml
    public EditorWindow()
    {
        InitializeComponent();

        this.Opened += OnOpened;
        this.Closed += OnClosed;
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        var mainWindow = new MainWindow();
        mainWindow.Show();
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        Editor.Focus();
        Editor.TextArea.Focus();
    }
}