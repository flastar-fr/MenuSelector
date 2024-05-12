using Avalonia.Controls;

namespace MenuSelector;

public partial class MenuItem : UserControl
{
    public MenuItem()
    {
        InitializeComponent();
    }
    
    public MenuItem(string menuName, string menuWeek, string menuTime)
    {
        InitializeComponent();
        MenuName.Text = menuName;
        MenuWeek.Text = menuWeek;
        MenuTime.Text = menuTime;
    }
}