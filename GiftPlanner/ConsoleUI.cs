using System;
using System.Globalization;
using System.Linq; //https://learn.microsoft.com/en-us/dotnet/csharp/linq/
using Spectre.Console;

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
    string choice;

    do
    {
        ShowTitle();
        ShowUpcomingDateReminders();

        choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Select an option:[/]")
                .PageSize(10)
                .HighlightStyle("cyan")
                .AddChoices(new[]
                {
                    "👥 Manage People",
                    "🎁 Manage Gift Ideas",
                    "💳 Track Purchases",
                    "🚪 Exit"
                }));

        if (choice.Contains("Manage People"))
        {
            ManagePeopleMenu();
        }
        else if (choice.Contains("Manage Gift Ideas"))
        {
            ManageGiftIdeasMenu();
        }
        else if (choice.Contains("Track Purchases"))
        {
            TrackPurchasesMenu();
        }

    } while (!choice.Contains("Exit"));
    }

    // A function to display the Manage People menu
    private void ManagePeopleMenu()
    {
        string choice;

        do
        {
            ShowSectionHeader("Manage People");

            choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Select an option:[/]")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                        "Add Person",
                        "List People",
                        "Delete Person",
                        "Add Important Date",
                        "View Upcoming Important Dates",
                        "Cancel"
                    }));

            if (choice == "Add Person")
            {
                AddPersonFlow();
            }
            else if (choice == "List People")
            {
                ListPeople();
                Pause();
            }
            else if (choice == "Delete Person")
            {
                DeletePersonFlow();
            }
            else if (choice == "Add Important Date")
            {
                AddImportantDateFlow();
            }
            else if (choice == "View Upcoming Important Dates")
            {
                ViewUpcomingImportantDatesFlow();
                Pause();
            }

        } while (choice != "Cancel");
    }

    // A function to delete a selected person and their associated gift ideas and purchases
    private void DeletePersonFlow()
    {
        if (dataManager.People.Count == 0)
        {
            ShowError("No people available to delete.");
            return;
        }

        ShowSectionHeader("Delete Person");

        var selectedPerson = SelectPersonWithCancel();
        if (selectedPerson == null)
        {
            ShowWarning("Cancelled. Returning to previous menu.");
            return;
        }

       var confirm = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"Are you sure you want to delete [red]{selectedPerson.Name}[/]?")
                .AddChoices("Yes", "No"));

        if (confirm == "No")
        {
            ShowWarning("Deletion cancelled.");
            return;
        }

        dataManager.DeletePerson(selectedPerson.PersonId);

        ShowSuccess("Person and associated data deleted successfully!");
    }

    // A function to add a person based on user input (supports typing Cancel to go back)
    private void AddPersonFlow()
    {
    ShowSectionHeader("Add Person");

    string name = AnsiConsole.Ask<string>("Enter person name ([grey]or type Cancel to go back[/]):");

    // Let the user escape if they entered this screen by mistake
    if (name.Trim().Equals("cancel", StringComparison.OrdinalIgnoreCase))
    {
        ShowWarning("Cancelled. Returning to previous menu.");
        return;
    }

    if (string.IsNullOrWhiteSpace(name))
    {
        ShowError("Name cannot be blank.");
        return;
    }

    // Reject names containing numbers
    if (name.Any(char.IsDigit))
    {
        ShowError("Name cannot contain numbers.");
        return;
    }

    // NEW: Prevent duplicate names
    if (dataManager.People.Any(p => 
        p.Name.Equals(name.Trim(), StringComparison.OrdinalIgnoreCase)))
    {
        ShowError("This person already exists in the list.");
        return;
    }

    int newId = dataManager.People.Count + 1;

    var person = new Person(newId, name.Trim());
    dataManager.AddPerson(person);

    ShowSuccess("Person added successfully!");
    }

    // A function to add an important date linked to a selected person
    private void AddImportantDateFlow()
    {
        if (dataManager.People.Count == 0)
        {
            ShowError("No people available. Add a person first.");
            return;
        }

        ShowSectionHeader("Add Important Date");

        var selectedPerson = SelectPersonWithCancel();
        if (selectedPerson == null)
        {
            ShowWarning("Cancelled. Returning to previous menu.");
            return;
        }

        string typeChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Select important date type:[/]")
                .PageSize(10)
                .AddChoices(new[]
                {
                    "Birthday",
                    "Anniversary",
                    "Graduation",
                    "Custom",
                    "Cancel"
                }));

        if (typeChoice == "Cancel")
        {
            ShowWarning("Cancelled. Returning to previous menu.");
            return;
        }

        string dateType = typeChoice;

        if (typeChoice == "Custom")
        {
            dateType = AnsiConsole.Ask<string>("Enter custom date type ([grey]or type Cancel to go back[/]):");

            if (dateType.Trim().Equals("cancel", StringComparison.OrdinalIgnoreCase))
            {
                ShowWarning("Cancelled. Returning to previous menu.");
                return;
            }

            if (string.IsNullOrWhiteSpace(dateType))
            {
                ShowError("Date type cannot be blank.");
                return;
            }
        }

        string input = AnsiConsole.Ask<string>("Enter important date ([grey]MM/DD/YYYY or type Cancel to go back[/]):");

        if (input.Trim().Equals("cancel", StringComparison.OrdinalIgnoreCase))
        {
            ShowWarning("Cancelled. Returning to previous menu.");
            return;
        }

        if (!DateTime.TryParseExact(input, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
        {
            ShowError("Please enter a valid date in MM/DD/YYYY format.");
            return;
        }

        var importantDate = dataManager.AddImportantDateToPerson(selectedPerson.PersonId, dateType.Trim(), date);

        if (importantDate != null)
        {
            ShowSuccess($"Important date added for {selectedPerson.Name}!");
        }
        else
        {
            ShowError("Unable to add important date.");
        }
    }

    // A function to list all saved people
    private void ListPeople()
    {
        ShowSectionHeader("People");

        if (dataManager.People.Count == 0)
        {
            AnsiConsole.MarkupLine("[grey](none yet)[/]");
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Cyan1)
            .AddColumn("[bold]ID[/]")
            .AddColumn("[bold]Name[/]");

        foreach (var person in dataManager.People)
        {
            table.AddRow(person.PersonId.ToString(), person.Name);
        }

        AnsiConsole.Write(table);
    }

    // A function to view upcoming important dates for all people
    private void ViewUpcomingImportantDatesFlow()
    {
        ShowSectionHeader("Upcoming Important Dates");

        var upcomingDates = dataManager.GetUpcomingImportantDates();

        if (upcomingDates.Count == 0)
        {
            AnsiConsole.MarkupLine("[grey](none yet)[/]");
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Blue)
            .AddColumn("[bold]Name[/]")
            .AddColumn("[bold]Type[/]")
            .AddColumn("[bold]Date[/]")
            .AddColumn("[bold]Days Until[/]");

        foreach (var entry in upcomingDates)
        {
            table.AddRow(
                entry.Person.Name,
                entry.ImportantDate.Type,
                entry.ImportantDate.Date.ToString("MMMM dd"),
                entry.DaysUntil.ToString());
        }

        AnsiConsole.Write(table);
    }

    // A function to display the Manage Gift Ideas menu
    private void ManageGiftIdeasMenu()
    {
        string choice;

        do
        {
            ShowSectionHeader("Manage Gift Ideas");

            choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Select an option:[/]")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                        "Add Gift Idea",
                        "View Gift Ideas",
                        "Remove Gift Idea",
                        "Cancel"
                    }));

            if (choice == "Add Gift Idea")
            {
                AddGiftIdeaFlow();
            }
            else if (choice == "View Gift Ideas")
            {
                ViewGiftIdeasFlow();
                Pause();
            }
            else if (choice == "Remove Gift Idea")
            {
                RemoveGiftIdeaFlow();
            }

        } while (choice != "Cancel");
    }

    // A function to add a gift idea linked to a selected person (supports typing Cancel to go back)
    private void AddGiftIdeaFlow()
    {
        if (dataManager.People.Count == 0)
        {
            ShowError("No people available. Add a person first.");
            return;
        }

        ShowSectionHeader("Add Gift Idea");

        var selectedPerson = SelectPersonWithCancel();
        if (selectedPerson == null)
        {
            ShowWarning("Cancelled. Returning to previous menu.");
            return;
        }

        string description = AnsiConsole.Ask<string>("Enter gift idea description ([grey]or type Cancel to go back[/]):");

        if (description.Trim().Equals("cancel", StringComparison.OrdinalIgnoreCase))
        {
            ShowWarning("Cancelled. Returning to previous menu.");
            return;
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            ShowError("Description cannot be blank.");
            return;
        }

        var giftIdea = dataManager.AddGiftIdeaToPerson(selectedPerson.PersonId, description.Trim());

        if (giftIdea != null)
        {
            ShowSuccess($"Gift idea added for {selectedPerson.Name}!");
        }
        else
        {
            ShowError("Unable to add gift idea.");
        }
    }

    // A function to view all gift ideas for a selected person
    private void ViewGiftIdeasFlow()
    {
        if (dataManager.People.Count == 0)
        {
            ShowError("No people available. Add a person first.");
            return;
        }

        ShowSectionHeader("View Gift Ideas");

        var selectedPerson = SelectPersonWithCancel();
        if (selectedPerson == null)
        {
            ShowWarning("Cancelled. Returning to previous menu.");
            return;
        }

        ShowSectionHeader($"Gift Ideas for {selectedPerson.Name}");

        if (selectedPerson.GiftIdeas.Count == 0)
        {
            AnsiConsole.MarkupLine("[grey](none yet)[/]");
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Magenta1)
            .AddColumn("[bold]Gift Idea ID[/]")
            .AddColumn("[bold]Description[/]")
            .AddColumn("[bold]Bought[/]");

        foreach (var idea in selectedPerson.GiftIdeas)
        {
            table.AddRow(
                idea.GiftIdeaId.ToString(),
                idea.Description,
                idea.Bought ? "Yes" : "No");
        }

        AnsiConsole.Write(table);
    }

    // A function to remove a gift idea from a selected person
    private void RemoveGiftIdeaFlow()
    {
        if (dataManager.People.Count == 0)
        {
            ShowError("No people available.");
            return;
        }

        ShowSectionHeader("Remove Gift Idea");

        var selectedPerson = SelectPersonWithCancel();
        if (selectedPerson == null)
        {
            ShowWarning("Cancelled. Returning to previous menu.");
            return;
        }

        if (selectedPerson.GiftIdeas.Count == 0)
        {
            ShowError("This person has no gift ideas.");
            return;
        }

        var choices = selectedPerson.GiftIdeas
            .Select(g => $"{g.GiftIdeaId} - {g.Description}")
            .ToList();

        choices.Add("Cancel");

        string selection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Select a gift idea to remove:[/]")
                .PageSize(10)
                .AddChoices(choices));

        if (selection == "Cancel")
        {
            ShowWarning("Cancelled.");
            return;
        }

        int selectedId = int.Parse(selection.Split(" - ")[0]);

        dataManager.RemoveGiftIdea(selectedPerson.PersonId, selectedId);

        ShowSuccess("Gift idea removed successfully!");
    }

    // A function to display the Track Purchases menu
    private void TrackPurchasesMenu()
    {
        string choice;

        do
        {
            ShowSectionHeader("Track Purchases");

            choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Select an option:[/]")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                        "Record Purchase",
                        "View Purchases",
                        "Cancel"
                    }));

            if (choice == "Record Purchase")
            {
                RecordPurchaseFlow();
            }
            else if (choice == "View Purchases")
            {
                ViewPurchasesFlow();
                Pause();
            }

        } while (choice != "Cancel");
    }

    // A function to record a purchase item and amount for a selected person
    private void RecordPurchaseFlow()
    {
        if (dataManager.People.Count == 0)
        {
            ShowError("No people available. Add a person first.");
            return;
        }

        ShowSectionHeader("Record Purchase");

        var selectedPerson = SelectPersonWithCancel();
        if (selectedPerson == null)
        {
            ShowWarning("Cancelled. Returning to previous menu.");
            return;
        }

        string item = AnsiConsole.Ask<string>("Enter purchase item ([grey]or type Cancel to go back[/]):");

        if (item.Trim().Equals("cancel", StringComparison.OrdinalIgnoreCase))
        {
            ShowWarning("Cancelled. Returning to previous menu.");
            return;
        }

        if (string.IsNullOrWhiteSpace(item))
        {
            ShowError("Purchase item cannot be blank.");
            return;
        }

        string input = AnsiConsole.Ask<string>("Enter purchase amount ([grey]example: 25.99 or type Cancel to go back[/]):");

        if (input.Trim().Equals("cancel", StringComparison.OrdinalIgnoreCase))
        {
            ShowWarning("Cancelled. Returning to previous menu.");
            return;
        }

        if (!decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal amount) &&
            !decimal.TryParse(input, out amount))
        {
            ShowError("Please enter a valid number.");
            return;
        }

        if (amount <= 0)
        {
            ShowError("Amount must be greater than 0.");
            return;
        }

        var purchase = dataManager.AddPurchaseToPerson(selectedPerson.PersonId, item.Trim(), amount);

        if (purchase != null)
        {
            ShowSuccess($"Purchase recorded for {selectedPerson.Name}: {item.Trim()} - ${amount:F2}");
        }
        else
        {
            ShowError("Unable to record purchase.");
        }
    }

    // A function to view purchases for a selected person and display the total spent
    private void ViewPurchasesFlow()
    {
        if (dataManager.People.Count == 0)
        {
            ShowError("No people available. Add a person first.");
            return;
        }

        ShowSectionHeader("View Purchases");

        var selectedPerson = SelectPersonWithCancel();
        if (selectedPerson == null)
        {
            ShowWarning("Cancelled. Returning to previous menu.");
            return;
        }

        ShowSectionHeader($"Purchases for {selectedPerson.Name}");

        if (selectedPerson.Purchases.Count == 0)
        {
            AnsiConsole.MarkupLine("[grey](none yet)[/]");
            return;
        }

        decimal total = 0;

        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Green)
            .AddColumn("[bold]Purchases[/]")
            .AddColumn("[bold]Amount[/]");

        foreach (var purchase in selectedPerson.Purchases)
        {
            table.AddRow(purchase.Item, $"${purchase.Amount:F2}");
            total += purchase.Amount;
        }

        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine($"\n[bold yellow]Total Spent:[/] [green]${total:F2}[/]");
    }

    // A function to let the user select a person or cancel and return to the previous menu
    private Person? SelectPersonWithCancel()
    {
        if (dataManager.People.Count == 0)
        {
            return null;
        }

        var choices = dataManager.People
            .Select(p => $"{p.PersonId} - {p.Name}")
            .ToList();

        choices.Add("Cancel");

        string selection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Select a person:[/]")
                .PageSize(10)
                .AddChoices(choices));

        if (selection == "Cancel")
        {
            return null;
        }

        int selectedId = int.Parse(selection.Split(" - ")[0]);
        return dataManager.FindPersonById(selectedId);
    }

    // A function to display reminder messages for important dates within 7 days
    private void ShowUpcomingDateReminders()
    {
        var reminders = dataManager.GetImportantDatesWithin7Days();

        if (reminders.Count == 0)
        {
            return;
        }

        foreach (var reminder in reminders)
        {
            string dayText = reminder.DaysUntil == 0 ? "today" : $"in {reminder.DaysUntil} day(s)";
            ShowWarning($"Reminder: {reminder.Person.Name} has a {reminder.ImportantDate.Type} {dayText}.");
        }

        AnsiConsole.WriteLine();
    }

    // A helper function to display the application title
    private void ShowTitle()
{
    AnsiConsole.Clear();

    AnsiConsole.Write(
        new Rule("[black on lightskyblue1 bold]  Gift Planner  [/]")
        .RuleStyle("lightskyblue1")
        .Centered());

    AnsiConsole.MarkupLine("[grey]Manage gift ideas and purchases[/]\n");
}

    // A helper function to display a section header
    private void ShowSectionHeader(string title)
    {
        AnsiConsole.Write(
            new Rule($"[yellow]{title}[/]")
                .RuleStyle("grey")
                .Centered());
    }

    // A helper function to display a success badge-style message
    private void ShowSuccess(string message)
    {
        AnsiConsole.MarkupLine($"\n[black on green3_1 bold] SUCCESS [/ ] {message}\n".Replace("[/ ]", "[/]"));
    }

    // A helper function to display an error badge-style message
    private void ShowError(string message)
    {
        AnsiConsole.MarkupLine($"\n[white on red bold] ERROR [/ ] {message}\n".Replace("[/ ]", "[/]"));
    }

    // A helper function to display a warning badge-style message
    private void ShowWarning(string message)
    {
        AnsiConsole.MarkupLine($"\n[black on yellow bold] WARNING [/ ] {message}\n".Replace("[/ ]", "[/]"));
    }

    // A helper function to pause before returning
    private void Pause()
    {
        AnsiConsole.MarkupLine("\n[grey]Press any key to continue...[/]");
        Console.ReadKey(true);
    }
}