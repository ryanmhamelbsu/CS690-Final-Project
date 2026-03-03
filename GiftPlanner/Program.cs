using System;

namespace GiftPlanner;

public class Program
{
    public static void Main(string[] args)
    {
        var dataManager = new DataManager();
        var ui = new ConsoleUI(dataManager);
        ui.Show();
    }
}