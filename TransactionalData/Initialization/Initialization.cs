namespace Model.Initialization;

public static class Initialization
{
    public static IEnumerable<Transaction> TransactionalData()
    {
        return new Transaction[]
        {
            new() { User = "John", TransactionDate = DateTime.Parse("2024/02/18"), Currency = Currency.CHF, Type = TransactionType.Leisure, Amount = -10 },
            new() { User = "Bob", TransactionDate = DateTime.Parse("2024/02/19"), Currency = Currency.CHF, Type = TransactionType.Leisure, Amount = -10 },
            
            new() { User = "John", TransactionDate = DateTime.Parse("2024/02/01"), Currency = Currency.CHF, Type = TransactionType.Income, Amount = +10 },
            new() { User = "Bob", TransactionDate = DateTime.Parse("2024/02/01"), Currency = Currency.CHF, Type = TransactionType.Income, Amount = +10 },
        };
    }

    public static IEnumerable<ActivityLog> ActivityLog()
    {
        return new ActivityLog[]
        {
            new() { User = "John", Date = DateTime.Parse("2024/02/01"), Activity = Constants.NewEarningProcessed + ": " + TransactionType.Income },
            new() { User = "Bob", Date = DateTime.Parse("2024/02/01"), Activity = Constants.NewEarningProcessed + ": " + TransactionType.Income },

            new() { User = "John", Date = DateTime.Parse("2024/02/18"), Activity = Constants.NewExpenseProcessed + ": " + TransactionType.Leisure },
            new() { User = "Bob", Date = DateTime.Parse("2024/02/19"), Activity = Constants.NewExpenseProcessed + ": " + TransactionType.Leisure }
        };
    }
}
