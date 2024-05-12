using Avalonia.Controls;

namespace MenuSelector;

public partial class MenuItemSelection : UserControl
{
    public MenuItemSelection()
    {
        InitializeComponent();
    }
    
    public MenuItemSelection(string menuName, string menuSeason, string menuWeek, string menuTime, string menuFrequence)
    {
        InitializeComponent();
        MenuName.Text = menuName;
        MenuSeason.Text = menuSeason;
        MenuWeek.Text = menuWeek;
        MenuTime.Text = menuTime;
        MenuFrequence.Text = menuFrequence;
    }
}