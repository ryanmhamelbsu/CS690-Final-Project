using System;

namespace GiftPlanner;

// Handles all console menus and user interaction for the application
public class ConsoleUI
{
    // Holds access to application data and file storage logic
    private DataManager dataManager;

    // Constructor creates the DataManager and loads saved data
    public ConsoleUI()
    {
        dataManager = new DataManager();
    }

    // Code to display the main menu and route the user to feature menus
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
                ManageGiftIdeasMenu();
            }
            else if (command == "3")
            {
                TrackPurchasesMenu();
            }

        } while (command != "4");
    }

    // Code to display the Manage People menu (FR1)
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

    // Code to add a person based on user input (FR1)
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

    // Code to list all saved people
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

    // Code to display the Manage Gift Ideas menu (FR3)
    private void ManageGiftIdeasMenu()
    {
        string choice;

        do
        {
            Console.WriteLine("\n--- Manage Gift Ideas ---");
            Console.WriteLine("1) Add Gift Idea (FR3)");
            Console.WriteLine("2) View Gift Ideas");
            Console.WriteLine("3) Cancel");
            Console.Write("Select an option: ");

            choice = Console.ReadLine() ?? "";

            if (choice == "1")
            {
                AddGiftIdeaFlow();
            }
            else if (choice == "2")
            {
                ViewGiftIdeasFlow();
            }

        } while (choice != "3");
    }

    // Code to add a gift idea linked to a selected person (FR3)
    private void AddGiftIdeaFlow()
    {
        if (dataManager.People.Count == 0)
        {
            Console.WriteLine("\nNo people available. Add a person first.");
            return;
        }

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

    // Code to view all gift ideas for a selected person
    private void ViewGiftIdeasFlow()
    {
        if (dataManager.People.Count == 0)
        {
            Console.WriteLine("\nNo people available. Add a person first.");
            return;
        }

        var selectedPerson = SelectPersonWithCancel();
        if (selectedPerson == null)
        {
            return; // Cancel
        }

        Console.WriteLine($"\nGift Ideas for {selectedPerson.Name}:");

        if (selectedPerson.GiftIdeas.Count == 0)
        {
            Console.WriteLine("(none yet)");
            return;
        }

        foreach (var idea in selectedPerson.GiftIdeas)
        {
            Console.WriteLine(idea.ToString());
        }
    }

    // Code to display the Track Purchases menu (FR5)
    private void TrackPurchasesMenu()
    {
        string choice;

        do
        {
            Console.WriteLine("\n--- Track Purchases ---");
            Console.WriteLine("1) Record Purchase (FR5)");
            Console.WriteLine("2) View Purchases");
            Console.WriteLine("3) Cancel");
            Console.Write("Select an option: ");

            choice = Console.ReadLine() ?? "";

            if (choice == "1")
            {
                RecordPurchaseFlow();
            }
            else if (choice == "2")
            {
                ViewPurchasesFlow();
            }

        } while (choice != "3");
    }

    // Code to record a purchase amount for a selected person (FR5)
    private void RecordPurchaseFlow()
    {
        if (dataManager.People.Count == 0)
        {
            Console.WriteLine("\nNo people available. Add a person first.");
            return;
        }

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

    // Code to view purchases for a selected person and display the total spent
    private void ViewPurchasesFlow()
    {
        if (dataManager.People.Count == 0)
        {
            Console.WriteLine("\nNo people available. Add a person first.");
            return;
        }

        var selectedPerson = SelectPersonWithCancel();
        if (selectedPerson == null)
        {
            return; // Cancel
        }

        Console.WriteLine($"\nPurchases for {selectedPerson.Name}:");

        if (selectedPerson.Purchases.Count == 0)
        {
            Console.WriteLine("(none yet)");
            return;
        }

        decimal total = 0;

        foreach (var purchase in selectedPerson.Purchases)
        {
            Console.WriteLine(purchase.ToString());
            total += purchase.Amount;
        }

        Console.WriteLine("-----------------------");
        Console.WriteLine($"Total Spent: ${total}");
    }

    // Code to let the user select a person or cancel and return to the previous menu
    private Person? SelectPersonWithCancel()
    {
        Console.WriteLine("\nSelect a person:");

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