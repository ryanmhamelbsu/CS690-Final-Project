namespace GiftPlanner;

// Represents a purchase recorded for a person
public class Purchase
{
    // Description of the item purchased
    public string Item { get; set; }

    // Amount spent for this purchase
    public decimal Amount { get; set; }

    // Constructor initializes the purchase with an item description and amount
    public Purchase(string item, decimal amount)
    {
        Item = item;
        Amount = amount;
    }

    // Returns a formatted string representation of the purchase
    public override string ToString()
    {
        return $"{Item}: ${Amount:F2}";
    }
}