using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace GiftPlanner;

public class DataManager
{
    // File paths used to store application data
    private readonly string peopleFile = "people.txt";
    private readonly string giftIdeasFile = "giftideas.txt";
    private readonly string purchasesFile = "purchases.txt";

    // In-memory list of all people in the system
    public List<Person> People { get; }

    // Constructor initializes lists and loads saved data from files
    public DataManager()
    {
        People = new List<Person>();

        LoadPeople();
        LoadGiftIdeas();
        LoadPurchases();
    }

    // Code to add people and save them to file
    public void AddPerson(Person person)
    {
        People.Add(person);

        // Save person to file as: PersonId|Name
        File.AppendAllText(peopleFile, $"{person.PersonId}|{person.Name}{Environment.NewLine}");
    }

    // Code to find a person by their ID
    public Person? FindPersonById(int personId)
    {
        return People.FirstOrDefault(p => p.PersonId == personId);
    }

    // Code to load people from file when the application starts
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

    // Code to add gift ideas linked to a person and save them to file
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

    // Code to load gift ideas from file when the application starts
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

    // Code to record purchases for a person and save them to file
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

    // Code to load purchases from file when the application starts
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