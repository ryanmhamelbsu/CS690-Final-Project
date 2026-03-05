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

    // Code to store all people in memory while the program is running
    public List<Person> People { get; }

    // A function to initialize file paths and load saved data
    public DataManager()
    {
        // Code to detect where the program is being run from
        string currentFolder = Directory.GetCurrentDirectory();

        // Code to always save files inside the GiftPlanner folder
        if (Path.GetFileName(currentFolder) == "GiftPlanner")
        {
            dataFolder = currentFolder; // running inside GiftPlanner folder
        }
        else
        {
            dataFolder = Path.Combine(currentFolder, "GiftPlanner"); // running from repo root
        }

        // Code to build full file paths inside the data folder
        peopleFile = Path.Combine(dataFolder, "people.txt");
        giftIdeasFile = Path.Combine(dataFolder, "giftideas.txt");
        purchasesFile = Path.Combine(dataFolder, "purchases.txt");

        // Code to ensure the data folder exists before loading/saving
        Directory.CreateDirectory(dataFolder);

        People = new List<Person>();

        LoadPeople();
        LoadGiftIdeas();
        LoadPurchases();
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

        var giftIdea = new GiftIdea(newGiftIdeaId, description);
        person.GiftIdeas.Add(giftIdea);

        // Save gift idea as: PersonId|GiftIdeaId|Description
        File.AppendAllText(giftIdeasFile, $"{personId}|{giftIdea.GiftIdeaId}|{giftIdea.Description}{Environment.NewLine}");

        return giftIdea;
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
            if (parts.Length != 3) continue;

            if (!int.TryParse(parts[0], out int personId)) continue;
            if (!int.TryParse(parts[1], out int giftIdeaId)) continue;

            var description = parts[2].Trim();
            if (description == "") continue;

            var person = FindPersonById(personId);
            if (person == null) continue;

            person.GiftIdeas.Add(new GiftIdea(giftIdeaId, description));
        }
    }

    // A function to add a purchase to a person and save it to file
    public Purchase? AddPurchaseToPerson(int personId, decimal amount)
    {
        var person = FindPersonById(personId);
        if (person == null)
        {
            return null;
        }

        int newPurchaseId = person.Purchases.Count + 1;

        var purchase = new Purchase(newPurchaseId, amount);
        person.Purchases.Add(purchase);

        // Save purchase as: PersonId|PurchaseId|Amount
        File.AppendAllText(
            purchasesFile,
            $"{personId}|{purchase.PurchaseId}|{purchase.Amount.ToString(CultureInfo.InvariantCulture)}{Environment.NewLine}"
        );

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
            if (!int.TryParse(parts[1], out int purchaseId)) continue;

            if (!decimal.TryParse(parts[2], NumberStyles.Number, CultureInfo.InvariantCulture, out decimal amount)) continue;

            var person = FindPersonById(personId);
            if (person == null) continue;

            person.Purchases.Add(new Purchase(purchaseId, amount));
        }
    }
}