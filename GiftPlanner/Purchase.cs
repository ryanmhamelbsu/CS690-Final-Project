namespace GiftPlanner;

public class Purchase
{
    public int PurchaseId { get; set; }
    public decimal Amount { get; set; }

    public Purchase(int purchaseId, decimal amount)
    {
        PurchaseId = purchaseId;
        Amount = amount;
    }

    public override string ToString()
    {
        return $"{PurchaseId}: ${Amount}";
    }
}