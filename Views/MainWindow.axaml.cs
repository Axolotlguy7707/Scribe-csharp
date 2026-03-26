using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Scribe.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OpenEditor(object sender, RoutedEventArgs e)
    {
        // Open new editor window and close this window
        var editor = new EditorWindow();
        editor.Show();
        this.Hide();
    }
}