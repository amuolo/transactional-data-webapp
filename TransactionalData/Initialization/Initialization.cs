namespace Model.Initialization;

public static class Initialization
{
    public static IEnumerable<Transaction> TransactionalData()
    {
        return new Transaction[]
        {
            new() {User = "John", TransactionDate = DateTime.Parse("2024/02/18"), Currency = "CHF", Type = TransactionType.Leisure, Amount = 0 },
            new() {User = "Bob", TransactionDate = DateTime.Parse("2024/02/19"), Currency = "CHF", Type = TransactionType.Leisure, Amount = 0 }
        };
    }
}
