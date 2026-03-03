using System;

namespace GiftPlanner;

public class ConsoleUI
{
    private readonly DataManager dataManager;
    private int nextPersonId = 1;

    public ConsoleUI(DataManager dataManager)
    {
        this.dataManager = dataManager;
    }

    public void Show()
    {
        string choice;

        do
        {
            Console.WriteLine();
            Console.WriteLine("=== Gift Planner ===");
            Console.WriteLine("1. Manage People");
            Console.WriteLine("2. Exit");
            Console.Write("Select an option: ");
            choice = Console.ReadLine() ?? "";

            if (choice == "1")
            {
                ShowManagePeopleMenu();
            }

        } while (choice != "2");
    }

    private void ShowManagePeopleMenu()
    {
        string choice;

        do
        {
            Console.WriteLine();
            Console.WriteLine("=== Manage People ===");
            Console.WriteLine("1. Add Person (FR1)");
            Console.WriteLine("2. List People");
            Console.WriteLine("3. Back");
            Console.Write("Select an option: ");
            choice = Console.ReadLine() ?? "";

            if (choice == "1")
            {
                AddPersonFlow();
            }
            else if (choice == "2")
            {
                ListPeople();
            }

        } while (choice != "3");
    }

    private void AddPersonFlow()
    {
        Console.Write("Enter person's name: ");
        var name = Console.ReadLine() ?? "";

        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Error: Name cannot be blank.");
            return;
        }

        var person = new Person(nextPersonId, name.Trim());
        nextPersonId++;

        dataManager.AddPerson(person);

        Console.WriteLine($"Saved: {person}");
    }

    private void ListPeople()
    {
        Console.WriteLine();
        Console.WriteLine("People:");

        if (dataManager.People.Count == 0)
        {
            Console.WriteLine("(none yet)");
            return;
        }

        foreach (var p in dataManager.People)
        {
            Console.WriteLine(p.ToString());
        }
    }
}