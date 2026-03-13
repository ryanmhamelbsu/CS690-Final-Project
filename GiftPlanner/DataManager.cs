using System;
using System.Collections.Generic; //https://learn.microsoft.com/en-us/dotnet/standard/generics/collections
using System.Globalization; //https://learn.microsoft.com/en-us/dotnet/core/extensions/globalization
using System.IO; //https://learn.microsoft.com/en-us/dotnet/standard/io/
using System.Linq; //https://learn.microsoft.com/en-us/dotnet/csharp/linq/

namespace GiftPlanner;

public class DataManager
{
    // Code to store file paths in one consistent location
    private readonly string dataFolder;
    private readonly string peopleFile;
    private readonly string giftIdeasFile;
    private readonly string purchasesFile;
    private readonly string importantDatesFile;

    // Code to store all people in memory while the program is running
    public List<Person> People { get; }

    // A function to initialize file paths and load saved data
    public DataManager()
    {
        // Store data in a dedicated folder next to the published application
        dataFolder = Path.Combine(AppContext.BaseDirectory, "data");

        // Code to build full file paths inside the data folder
        peopleFile = Path.Combine(dataFolder, "people.txt");
        giftIdeasFile = Path.Combine(dataFolder, "giftideas.txt");
        purchasesFile = Path.Combine(dataFolder, "purchases.txt");
        importantDatesFile = Path.Combine(dataFolder, "importantdates.txt");

        // Code to ensure the data folder exists before loading/saving
        Directory.CreateDirectory(dataFolder);

        People = new List<Person>();

        LoadPeople();
        LoadGiftIdeas();
        LoadPurchases();
        LoadImportantDates();
    }

    // A function to add a person and save them to file
    public void AddPerson(Person person)
    {
        People.Add(person);

        // Save person to file as: PersonId|Name
        File.AppendAllText(peopleFile, $"{person.PersonId}|{person.Name}{Environment.NewLine}");
    }

    // A function to find a person by their ID
    public Person? FindPersonById(int personId)
    {
        return People.FirstOrDefault(p => p.PersonId == personId);
    }

    // A function to delete a person and all associated data
    public void DeletePerson(int personId)
    {
        var person = FindPersonById(personId);
        if (person == null)
        {
            return;
        }

        // Remove person from memory
        People.Remove(person);

        // Rewrite people file
        File.WriteAllLines(
            peopleFile,
            People.Select(p => $"{p.PersonId}|{p.Name}")
        );

        // Rewrite gift ideas file
        SaveGiftIdeas();

        // Rewrite purchases file
        File.WriteAllLines(
            purchasesFile,
            People.SelectMany(p => p.Purchases.Select(pr => $"{p.PersonId}|{pr.Item}|{pr.Amount.ToString(CultureInfo.InvariantCulture)}"))
        );

        // Rewrite important dates file
        File.WriteAllLines(
            importantDatesFile,
            People.SelectMany(p => p.ImportantDates.Select(d => $"{p.PersonId}|{d.Type}|{d.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}"))
        );
    }

    // A function to load people from file when the application starts
    private void LoadPeople()
    {
        if (!File.Exists(peopleFile))
        {
            return;
        }

        var lines = File.ReadAllLines(peopleFile);

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split('|');
            if (parts.Length != 2) continue;

            if (!int.TryParse(parts[0], out int id)) continue;

            var name = parts[1].Trim();
            if (name == "") continue;

            People.Add(new Person(id, name));
        }
    }

    // A function to add a gift idea to a person and save it to file
    public GiftIdea? AddGiftIdeaToPerson(int personId, string description)
    {
        var person = FindPersonById(personId);
        if (person == null)
        {
            return null;
        }

        int newGiftIdeaId = person.GiftIdeas.Count + 1;

        var giftIdea = new GiftIdea(newGiftIdeaId, description, false);
        person.GiftIdeas.Add(giftIdea);

        // Save gift idea as: PersonId|GiftIdeaId|Description|Bought
        File.AppendAllText(giftIdeasFile, $"{personId}|{giftIdea.GiftIdeaId}|{giftIdea.Description}|{giftIdea.Bought}{Environment.NewLine}");

        return giftIdea;
    }

    // A function to remove a gift idea from a person
    public void RemoveGiftIdea(int personId, int giftIdeaId)
    {
        var person = FindPersonById(personId);
        if (person == null)
        {
            return;
        }

        var giftIdea = person.GiftIdeas.FirstOrDefault(g => g.GiftIdeaId == giftIdeaId);
        if (giftIdea == null)
        {
            return;
        }

        person.GiftIdeas.Remove(giftIdea);

        // Rewrite gift ideas file
        SaveGiftIdeas();
    }

    // A function to mark a matching gift idea as bought for a person
    private void MarkGiftIdeaAsBoughtIfMatch(int personId, string item)
    {
        var person = FindPersonById(personId);
        if (person == null)
        {
            return;
        }

        var matchingGiftIdea = person.GiftIdeas.FirstOrDefault(g =>
            g.Description.Equals(item.Trim(), StringComparison.OrdinalIgnoreCase));

        if (matchingGiftIdea != null)
        {
            matchingGiftIdea.Bought = true;
            SaveGiftIdeas();
        }
    }

    // A function to rewrite all gift ideas to file
    private void SaveGiftIdeas()
    {
        File.WriteAllLines(
            giftIdeasFile,
            People.SelectMany(p => p.GiftIdeas.Select(g => $"{p.PersonId}|{g.GiftIdeaId}|{g.Description}|{g.Bought}"))
        );
    }

    // A function to load gift ideas from file when the application starts
    private void LoadGiftIdeas()
    {
        if (!File.Exists(giftIdeasFile))
        {
            return;
        }

        var lines = File.ReadAllLines(giftIdeasFile);

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split('|');

            // Support old format: PersonId|GiftIdeaId|Description
            // Support new format: PersonId|GiftIdeaId|Description|Bought
            if (parts.Length != 3 && parts.Length != 4) continue;

            if (!int.TryParse(parts[0], out int personId)) continue;
            if (!int.TryParse(parts[1], out int giftIdeaId)) continue;

            var description = parts[2].Trim();
            if (description == "") continue;

            bool bought = false;
            if (parts.Length == 4)
            {
                bool.TryParse(parts[3], out bought);
            }

            var person = FindPersonById(personId);
            if (person == null) continue;

            person.GiftIdeas.Add(new GiftIdea(giftIdeaId, description, bought));
        }
    }

    // A function to add a purchase to a person and save it to file
    public Purchase? AddPurchaseToPerson(int personId, string item, decimal amount)
    {
        var person = FindPersonById(personId);
        if (person == null)
        {
            return null;
        }

        var purchase = new Purchase(item, amount);
        person.Purchases.Add(purchase);

        // Save purchase as: PersonId|Item|Amount
        File.AppendAllText(
            purchasesFile,
            $"{personId}|{purchase.Item}|{purchase.Amount.ToString(CultureInfo.InvariantCulture)}{Environment.NewLine}"
        );

        // Mark matching gift idea as bought if purchase item matches gift idea description
        MarkGiftIdeaAsBoughtIfMatch(personId, item);

        return purchase;
    }

    // A function to load purchases from file when the application starts
    private void LoadPurchases()
    {
        if (!File.Exists(purchasesFile))
        {
            return;
        }

        var lines = File.ReadAllLines(purchasesFile);

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split('|');
            if (parts.Length != 3) continue;

            if (!int.TryParse(parts[0], out int personId)) continue;

            var item = parts[1].Trim();
            if (item == "") continue;

            if (!decimal.TryParse(parts[2], NumberStyles.Number, CultureInfo.InvariantCulture, out decimal amount)) continue;

            var person = FindPersonById(personId);
            if (person == null) continue;

            person.Purchases.Add(new Purchase(item, amount));
        }
    }

    // A function to add an important date to a person and save it to file
    public ImportantDate? AddImportantDateToPerson(int personId, string type, DateTime date)
    {
        var person = FindPersonById(personId);
        if (person == null)
        {
            return null;
        }

        var importantDate = new ImportantDate(type, date);
        person.ImportantDates.Add(importantDate);

        // Save important date as: PersonId|Type|Date
        File.AppendAllText(
            importantDatesFile,
            $"{personId}|{importantDate.Type}|{importantDate.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}{Environment.NewLine}"
        );

        return importantDate;
    }

    // A function to load important dates from file when the application starts
    private void LoadImportantDates()
    {
        if (!File.Exists(importantDatesFile))
        {
            return;
        }

        var lines = File.ReadAllLines(importantDatesFile);

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split('|');
            if (parts.Length != 3) continue;

            if (!int.TryParse(parts[0], out int personId)) continue;

            var type = parts[1].Trim();
            if (type == "") continue;

            if (!DateTime.TryParseExact(parts[2], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date)) continue;

            var person = FindPersonById(personId);
            if (person == null) continue;

            person.ImportantDates.Add(new ImportantDate(type, date));
        }
    }

    // A function to get all upcoming important dates sorted by the next occurrence
    public List<(Person Person, ImportantDate ImportantDate, int DaysUntil)> GetUpcomingImportantDates()
    {
        var today = DateTime.Today;
        var upcomingDates = new List<(Person Person, ImportantDate ImportantDate, int DaysUntil)>();

        foreach (var person in People)
        {
            foreach (var importantDate in person.ImportantDates)
            {
                var nextOccurrence = new DateTime(today.Year, importantDate.Date.Month, importantDate.Date.Day);

                if (nextOccurrence < today)
                {
                    nextOccurrence = nextOccurrence.AddYears(1);
                }

                int daysUntil = (nextOccurrence - today).Days;

                upcomingDates.Add((person, importantDate, daysUntil));
            }
        }

        return upcomingDates
            .OrderBy(x => x.DaysUntil)
            .ToList();
    }

    // A function to get important dates happening within the next 7 days
    public List<(Person Person, ImportantDate ImportantDate, int DaysUntil)> GetImportantDatesWithin7Days()
    {
        return GetUpcomingImportantDates()
            .Where(x => x.DaysUntil <= 7)
            .ToList();
    }
}