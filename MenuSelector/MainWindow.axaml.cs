using Avalonia.Controls;

namespace MenuSelector;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        for (var i = 0; i < 15; i++)
        {
            var test = new MenuItem("Pomme de terre au jambon", "Week-End", "Soir");
            MenusBox.Items.Add(test);
        }
    }
}
