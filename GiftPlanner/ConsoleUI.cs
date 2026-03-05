using System;
using System.Linq;//https://learn.microsoft.com/en-us/dotnet/csharp/linq/


namespace GiftPlanner;

// Handles all console menus and user interaction for the application
public class ConsoleUI
{
    // Holds access to application data and file storage logic
    private DataManager dataManager;

    // A function to create the DataManager and load saved data
    public ConsoleUI()
    {
        dataManager = new DataManager();
    }

    // A function to display the main menu and route the user to feature menus
    public void Show()
    {
        string command;

        do
        {
            Console.WriteLine("\n--- GIFT PLANNER ---");
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

    // A function to display the Manage People menu
    private void ManagePeopleMenu()
    {
        string choice;

        do
        {
            Console.WriteLine("\n--- MANAGE PEOPLE ---");
            Console.WriteLine("1) Add Person");
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

    // A function to add a person based on user input (supports typing Cancel to go back)
    private void AddPersonFlow()
    {
        Console.Write("\nEnter person name (or type Cancel to go back): ");
        string name = Console.ReadLine() ?? "";

        // Let the user escape if they entered this screen by mistake
        if (name.Trim().Equals("cancel", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("\nCancelled. Returning to previous menu.\n");
            return;
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("\nError: Name cannot be blank.\n");
            return;
        }

        // Reject names containing numbers
        if (name.Any(char.IsDigit))
        {
            Console.WriteLine("\nError: Name cannot contain numbers.\n");
            return;
        }

        int newId = dataManager.People.Count + 1;

        var person = new Person(newId, name.Trim());
        dataManager.AddPerson(person);

        Console.WriteLine("\nPerson added successfully!\n");
    }

    // A function to list all saved people
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

    // A function to display the Manage Gift Ideas menu
    private void ManageGiftIdeasMenu()
    {
        string choice;

        do
        {
            Console.WriteLine("\n--- MANAGE GIFT IDEAS ---");
            Console.WriteLine("1) Add Gift Idea");
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

    // A function to add a gift idea linked to a selected person (supports typing Cancel to go back)
    private void AddGiftIdeaFlow()
    {
        if (dataManager.People.Count == 0)
        {
            Console.WriteLine("\nError: No people available. Add a person first.\n");
            return;
        }

        var selectedPerson = SelectPersonWithCancel();
        if (selectedPerson == null)
        {
            return; // Cancel
        }

        Console.Write("Enter gift idea description (or type Cancel to go back): ");
        string description = Console.ReadLine() ?? "";

        // Let the user escape if they entered this screen by mistake
        if (description.Trim().Equals("cancel", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("\nCancelled. Returning to previous menu.\n");
            return;
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            Console.WriteLine("\nError: Description cannot be blank.\n");
            return;
        }

        var giftIdea = dataManager.AddGiftIdeaToPerson(selectedPerson.PersonId, description.Trim());

        if (giftIdea != null)
        {
            Console.WriteLine($"\nGift idea added for {selectedPerson.Name}!\n");
        }
        else
        {
            Console.WriteLine("\nError: Unable to add gift idea.\n");
        }
    }

    // A function to view all gift ideas for a selected person
    private void ViewGiftIdeasFlow()
    {
        if (dataManager.People.Count == 0)
        {
            Console.WriteLine("\nError: No people available. Add a person first.\n");
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

    // A function to display the Track Purchases menu
    private void TrackPurchasesMenu()
    {
        string choice;

        do
        {
            Console.WriteLine("\n--- TRACK PURCHASES ---");
            Console.WriteLine("1) Record Purchase");
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

    // A function to record a purchase amount for a selected person (supports typing Cancel to go back)
    private void RecordPurchaseFlow()
    {
        if (dataManager.People.Count == 0)
        {
            Console.WriteLine("\nError: No people available. Add a person first.\n");
            return;
        }

        var selectedPerson = SelectPersonWithCancel();
        if (selectedPerson == null)
        {
            return; // Cancel
        }

        Console.Write("Enter purchase amount (example: 25.99) or type Cancel to go back: ");
        string input = Console.ReadLine() ?? "";

        // Let the user escape if they entered this screen by mistake
        if (input.Trim().Equals("cancel", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("\nCancelled. Returning to previous menu.\n");
            return;
        }

        if (!decimal.TryParse(input, out decimal amount))
        {
            Console.WriteLine("\nError: Please enter a valid number.\n");
            return;
        }

        if (amount <= 0)
        {
            Console.WriteLine("\nError: Amount must be greater than 0.\n");
            return;
        }

        var purchase = dataManager.AddPurchaseToPerson(selectedPerson.PersonId, amount);

        if (purchase != null)
        {
            Console.WriteLine($"\nPurchase recorded for {selectedPerson.Name}: ${amount}\n");
        }
        else
        {
            Console.WriteLine("\nError: Unable to record purchase.\n");
        }
    }

    // A function to view purchases for a selected person and display the total spent
    private void ViewPurchasesFlow()
    {
        if (dataManager.People.Count == 0)
        {
            Console.WriteLine("\nError: No people available. Add a person first.\n");
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

    // A function to let the user select a person or cancel and return to the previous menu
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
            Console.WriteLine("\nError: Invalid selection.\n");
            return null;
        }

        if (choice == dataManager.People.Count + 1)
        {
            return null; // Cancel
        }

        if (choice < 1 || choice > dataManager.People.Count)
        {
            Console.WriteLine("\nError: Invalid selection.\n");
            return null;
        }

        return dataManager.People[choice - 1];
    }
}