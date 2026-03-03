namespace GiftPlanner;

// Represents a purchase recorded for a person
public class Purchase
{
    // Unique identifier for the purchase
    public int PurchaseId { get; set; }

    // Amount spent for this purchase
    public decimal Amount { get; set; }

    // Constructor initializes the purchase with an ID and amount
    public Purchase(int purchaseId, decimal amount)
    {
        PurchaseId = purchaseId;
        Amount = amount;
    }

    // Returns a formatted string representation of the purchase
    public override string ToString()
    {
        return $"{PurchaseId}: ${Amount}";
    }
}