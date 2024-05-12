using Avalonia.Controls;

namespace MenuSelector;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        for (var i = 0; i < 15; i++)
        {
            var test = new MenuItemSelection("Pomme de terre au jambon", "Mi-Saison", "Week-End", "Soir", "4", 
                RemoveFromMenusBox);
            MenusBox.Items.Add(test);
        }
    }

    public bool RemoveFromMenusBox(MenuItemSelection menuItemSelection)
    {
        MenusBox.Items.Remove(menuItemSelection);

        return true;
    }
}
