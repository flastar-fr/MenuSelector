using System;
using Avalonia.Controls;

namespace MenuSelector;

public partial class MenuItemSelection : UserControl
{
    private readonly Func<MenuItemSelection, bool>? _functionToDelete;
    public MenuItemSelection()
    {
        InitializeComponent();
    }
    
    public MenuItemSelection(string menuName, string menuSeason, string menuWeek, string menuTime, string menuFrequence,
        Func<MenuItemSelection, bool> deleteBinding)
    {
        InitializeComponent();
        MenuName.Text = menuName;
        MenuSeason.Text = menuSeason;
        MenuWeek.Text = menuWeek;
        MenuTime.Text = menuTime;
        MenuFrequence.Text = menuFrequence;

        DeleteButton.Click += OnClickHandler;

        _functionToDelete = deleteBinding;
    }

    public void OnClickHandler(object? sender, EventArgs e)
    {
        _functionToDelete?.Invoke(this);
    }
}