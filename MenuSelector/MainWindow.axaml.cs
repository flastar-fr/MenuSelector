using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Enums;

namespace MenuSelector;

public partial class MainWindow : Window
{
    private const string DbPath = "database_to_use.db";
    private readonly SQLiteCommand _command;

    public MainWindow()
    {
        InitializeComponent();

        var connection = new SQLiteConnection("Data Source=" + DbPath);
        connection.Open();
        _command = new SQLiteCommand(connection);
    }

    public bool RemoveFromMenusBox(MenuItemSelection menuItemSelection)
    {
        int.TryParse(menuItemSelection.MenuFrequence.Text!, out int freq);
        RemoveToDatabase(menuItemSelection.MenuName.Text!,
            menuItemSelection.MenuSeason.Text!,
            menuItemSelection.MenuWeek.Text!,
            menuItemSelection.MenuTime.Text!,
            freq);
        MenusBox.Items.Remove(menuItemSelection);

        return true;
    }

    public void AddToDatabase(object? sender, RoutedEventArgs e)
    {
        string? menuToAdd = MenuToAdd.Text;

        if (string.IsNullOrEmpty(menuToAdd))
        {
            IMsBox<ButtonResult>? box =
                MessageBoxManager.GetMessageBoxStandard("Error", "Nom de menu invalide",
                    icon: MsBox.Avalonia.Enums.Icon.Error);
            box.ShowAsync();
            return;
        }

        var seasonToAdd = (SeasonToAdd.SelectedItem as ComboBoxItem)?.Content!.ToString();
        var weekToAdd = (WeekToAdd.SelectedItem as ComboBoxItem)?.Content!.ToString();
        var timeToAdd = (TimeToAdd.SelectedItem as ComboBoxItem)?.Content!.ToString();

        bool isNumeric = int.TryParse(FreqToAdd.Text, out int freqToAdd);
        if (!isNumeric)
        {
            IMsBox<ButtonResult>? box =
                MessageBoxManager.GetMessageBoxStandard("Error", "La fréquence doit être entre 1 et 5",
                    icon: MsBox.Avalonia.Enums.Icon.Error);
            box.ShowAsync();
            return;
        }

        if (freqToAdd is < 0 or > 4)
        {
            IMsBox<ButtonResult>? box =
                MessageBoxManager.GetMessageBoxStandard("Error", "La fréquence doit être entre 1 et 5",
                    icon: MsBox.Avalonia.Enums.Icon.Error);
            box.ShowAsync();
        }

        _command.CommandText = "INSERT INTO Menus(name, season, week, timing, frequence) " +
                               "VALUES (@menu, @season, @week, @time, @freq);";

        _command.Parameters.Add(new SQLiteParameter("menu", menuToAdd));
        _command.Parameters.Add(new SQLiteParameter("season", seasonToAdd));
        _command.Parameters.Add(new SQLiteParameter("week", weekToAdd));
        _command.Parameters.Add(new SQLiteParameter("time", timeToAdd));
        _command.Parameters.Add(new SQLiteParameter("freq", freqToAdd));

        _command.ExecuteNonQuery();
    }

    public void RemoveToDatabase(string menuName, string menuSeason, string menuWeek, string menuTime, int freq)
    {
        _command.CommandText = $"DELETE FROM MENUS" +
                               $" WHERE Menus.name = '{menuName}' AND" +
                               $" Menus.season = '{menuSeason}' AND" +
                               $" Menus.week = '{menuWeek}' AND" +
                               $" Menus.timing = '{menuTime}' AND" +
                               $" Menus.frequence = {freq}";
        _command.ExecuteNonQuery();
    }

    public void GetInDatabaseFromFilter(object? sender, RoutedEventArgs e)
    {
        var season = (SeasonFilter.SelectedItem as ComboBoxItem)?.Content!.ToString();
        var week = (WeekFilter.SelectedItem as ComboBoxItem)?.Content!.ToString();
        var time = (TimeFilter.SelectedItem as ComboBoxItem)?.Content!.ToString();

        var commandToExecute = "SELECT * FROM MENUS WHERE ";
        var count = 0;
        if (season != "Toutes")
        {
            commandToExecute += $"Menus.season = '{season}'";
            count++;
        }

        if (week != "Tous")
        {
            if (count > 0) commandToExecute += " AND ";
            commandToExecute += $"Menus.week = '{week}'";
            count++;
        }

        if (time != "Tous")
        {
            if (count > 0) commandToExecute += " AND ";
            commandToExecute += $"Menus.timing = '{time}'";
        }

        MenusBox.Items.Clear();

        if (season == "Toutes" && week == "Tous" && time == "Tous")
        {
            _command.CommandText = "SELECT * FROM MENUS";
        }
        else
        {
            _command.CommandText = commandToExecute + ";";
        }

        SQLiteDataReader reader = _command.ExecuteReader();
        while (reader.Read())
        {
            var item = new MenuItemSelection(
                reader["name"].ToString()!,
                reader["season"].ToString()!,
                reader["week"].ToString()!,
                reader["timing"].ToString()!,
                reader["frequence"].ToString()!,
                RemoveFromMenusBox);
            MenusBox.Items.Add(item);
        }
    }

    public void GenerateMenus(object? sender, RoutedEventArgs e)
    {
        // get season
        string season;
        if ((bool)SsHivers.IsChecked!) season = "Hivers";
        else if ((bool)SsMi.IsChecked!) season = "Mi-Saison";
        else season = "Été";
        
        MenusBox.Items.Clear();

        bool isNumeric = int.TryParse(MidiSe.Text ?? "0", out int amount);
        if (!isNumeric)
        {
            IMsBox<ButtonResult>? box = 
                MessageBoxManager.GetMessageBoxStandard("Error", "Valeur d'entrée invalide",
                    icon:MsBox.Avalonia.Enums.Icon.Error);
            box.ShowAsync();
            return;
        }
        if (amount >= 1) GenerateMenusFromCategory(season, "Semaine", "Midi", amount);
        
        isNumeric = int.TryParse(SoirSe.Text ?? "0", out amount);
        if (!isNumeric)
        {
            IMsBox<ButtonResult>? box = 
                MessageBoxManager.GetMessageBoxStandard("Error", "Valeur d'entrée invalide",
                    icon:MsBox.Avalonia.Enums.Icon.Error);
            box.ShowAsync();
            return;
        }
        if (amount >= 1) GenerateMenusFromCategory(season, "Semaine", "Soir", amount);
        
        isNumeric = int.TryParse(MidiWe.Text ?? "0", out amount);
        if (!isNumeric)
        {
            IMsBox<ButtonResult>? box = 
                MessageBoxManager.GetMessageBoxStandard("Error", "Valeur d'entrée invalide",
                    icon:MsBox.Avalonia.Enums.Icon.Error);
            box.ShowAsync();
            return;
        }
        if (amount >= 1) GenerateMenusFromCategory(season, "Week-End", "Midi", amount);
        
        isNumeric = int.TryParse(SoirWe.Text ?? "0", out amount);
        if (!isNumeric)
        {
            IMsBox<ButtonResult>? box = 
                MessageBoxManager.GetMessageBoxStandard("Error", "Valeur d'entrée invalide",
                    icon:MsBox.Avalonia.Enums.Icon.Error);
            box.ShowAsync();
            return;
        }
        if (amount >= 1) GenerateMenusFromCategory(season, "Week-End", "Soir", amount);
    }

    public void GenerateMenusFromCategory(string season, string week, string time, int amount)
    {
        var listItem = new List<MenuItem>();
        var hashTableItem = new Hashtable();
        _command.CommandText = "SELECT * FROM Menus" +
                               $" WHERE Menus.season='{season}' AND Menus.week='{week}' AND Menus.timing='{time}'";

        SQLiteDataReader reader = _command.ExecuteReader();
        while (reader.Read())
        {
            var item = new MenuItem(reader["name"].ToString()!, reader["week"].ToString()!,
                reader["timing"].ToString()!);

            int value = int.Parse(reader["frequence"].ToString()!);
            hashTableItem[item] = value;
            for (var i = 0; i < value; i++)
            {
                listItem.Add(item);
            }
        }

        reader.Close();

        if (amount > hashTableItem.Count)
        {
            IMsBox<ButtonResult>? box =
                MessageBoxManager.GetMessageBoxStandard("Error", $"Pas suffisament de menus dans cette catégorie {week}/{time}",
                    icon: MsBox.Avalonia.Enums.Icon.Error);
            box.ShowAsync();
            return;
        }
        
        var random = new Random();
        for (var i = 0; i < amount; i++)
        {
            int randomIndex = random.Next(0, listItem.Count);

            MenuItem randomItem = listItem[randomIndex];
            var newItem = new MenuItem(randomItem.MenuName.Text!, randomItem.MenuWeek.Text!, randomItem.MenuTime.Text!);
            MenusBox.Items.Add(newItem);

            int itemFreq = int.Parse(hashTableItem[randomItem]!.ToString()!);
            for (var j = 0; j < itemFreq; j++)
            {
                listItem.Remove(randomItem);
            }
        }
    }
}
