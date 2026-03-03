using System;

namespace GiftPlanner;

public class ConsoleUI
{
    private DataManager dataManager;

    public ConsoleUI()
    {
        dataManager = new DataManager();
    }

    public void Show()
    {
        string command;

        do
        {
            Console.WriteLine("\n--- Gift Planner ---");
            Console.WriteLine("1) Manage People");
            Console.WriteLine("2) Manage Gift Ideas");
            Console.WriteLine("3) Track Purchases");
            Console.WriteLine("4) Exit");
            Console.Write("Select an option: ");

            command = Console.ReadLine() ?? "";

            if (command == "1")
            {
                ManagePeopleMenu();
            }
            else if (command == "2")
            {
                ManageGiftIdeasFlow();
            }
            else if (command == "3")
            {
                TrackPurchasesFlow();
            }

        } while (command != "4");
    }

    // --------------------
    // Manage People (FR1)
    // --------------------
    private void ManagePeopleMenu()
    {
        string choice;

        do
        {
            Console.WriteLine("\n--- Manage People ---");
            Console.WriteLine("1) Add Person (FR1)");
            Console.WriteLine("2) List People");
            Console.WriteLine("3) Cancel");
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
        Console.Write("\nEnter person name: ");
        string name = Console.ReadLine() ?? "";

        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Error: Name cannot be blank.");
            return;
        }

        int newId = dataManager.People.Count + 1;

        var person = new Person(newId, name.Trim());
        dataManager.AddPerson(person);

        Console.WriteLine("Person added successfully!");
    }

    private void ListPeople()
    {
        Console.WriteLine("\nPeople:");

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

    // --------------------
    // Manage Gift Ideas (FR3)
    // --------------------
    private void ManageGiftIdeasFlow()
    {
        if (dataManager.People.Count == 0)
        {
            Console.WriteLine("\nNo people available. Add a person first.");
            return;
        }

        Console.WriteLine("\n--- Manage Gift Ideas ---");
        var selectedPerson = SelectPersonWithCancel();
        if (selectedPerson == null)
        {
            return; // Cancel
        }

        Console.Write("Enter gift idea description: ");
        string description = Console.ReadLine() ?? "";

        if (string.IsNullOrWhiteSpace(description))
        {
            Console.WriteLine("Error: Description cannot be blank.");
            return;
        }

        var giftIdea = dataManager.AddGiftIdeaToPerson(selectedPerson.PersonId, description.Trim());

        if (giftIdea != null)
        {
            Console.WriteLine($"Gift idea added for {selectedPerson.Name}!");
        }
        else
        {
            Console.WriteLine("Error adding gift idea.");
        }
    }

    // --------------------
    // Track Purchases (FR5)
    // --------------------
    private void TrackPurchasesFlow()
    {
        if (dataManager.People.Count == 0)
        {
            Console.WriteLine("\nNo people available. Add a person first.");
            return;
        }

        Console.WriteLine("\n--- Track Purchases ---");
        var selectedPerson = SelectPersonWithCancel();
        if (selectedPerson == null)
        {
            return; // Cancel
        }

        Console.Write("Enter purchase amount (example: 25.99): ");
        string input = Console.ReadLine() ?? "";

        if (!decimal.TryParse(input, out decimal amount))
        {
            Console.WriteLine("Error: Please enter a valid number.");
            return;
        }

        if (amount <= 0)
        {
            Console.WriteLine("Error: Amount must be greater than 0.");
            return;
        }

        var purchase = dataManager.AddPurchaseToPerson(selectedPerson.PersonId, amount);

        if (purchase != null)
        {
            Console.WriteLine($"Purchase recorded for {selectedPerson.Name}: ${amount}");
        }
        else
        {
            Console.WriteLine("Error recording purchase.");
        }
    }

    // --------------------
    // Shared Helper
    // --------------------
    private Person? SelectPersonWithCancel()
    {
        Console.WriteLine("Select a person:");

        for (int i = 0; i < dataManager.People.Count; i++)
        {
            Console.WriteLine($"{i + 1}) {dataManager.People[i].Name}");
        }

        Console.WriteLine($"{dataManager.People.Count + 1}) Cancel");
        Console.Write("Enter selection: ");

        if (!int.TryParse(Console.ReadLine(), out int choice))
        {
            Console.WriteLine("Invalid selection.");
            return null;
        }

        if (choice == dataManager.People.Count + 1)
        {
            return null; // Cancel
        }

        if (choice < 1 || choice > dataManager.People.Count)
        {
            Console.WriteLine("Invalid selection.");
            return null;
        }

        return dataManager.People[choice - 1];
    }
}   