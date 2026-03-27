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
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        Editor.Focus();
        Editor.TextArea.Focus();
    }
}