using System;

namespace GiftPlanner;
//Handles application startup and launches the consoleUI
public class Program
{
    public static void Main(string[] args)
    {
        var ui = new ConsoleUI();
        ui.Show();
    }
}