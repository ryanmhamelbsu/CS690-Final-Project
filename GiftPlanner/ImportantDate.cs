using System;

namespace GiftPlanner;

// Handles important dates and date releated logic
public class ImportantDate
{
    // The type of important date, such as Birthday or Anniversary
    public string Type { get; set; }

    // The date value
    public DateTime Date { get; set; }

    // Constructor initializes the important date
    public ImportantDate(string type, DateTime date)
    {
        Type = type;
        Date = date;
    }

    // Returns a formatted string representation of the important date
    public override string ToString()
    {
        return $"{Type}: {Date:yyyy-MM-dd}";
    }
}