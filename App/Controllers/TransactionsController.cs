using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App.Data;
using Model;
using System.Text.Json;
using System.Net;

namespace App.Controllers;

public class TransactionsController : Controller
{
    private readonly DbCatalogContext _dbContext;

    private List<Transaction>? TransactionsCache { get; set; }

    private Dictionary<string, (string User, decimal Earnings, decimal Expenses, decimal Balance)> OverviewCache { get; set; }

    public TransactionsController(DbCatalogContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<JsonResult> Overview()
    {
        await InitializeCacheAsync();
        var items = OverviewCache.Values.Select(x => new 
        {
            User = x.User,
            Earnings = x.Earnings,
            Expenses = x.Expenses,
            Balance = x.Balance
        }).ToArray();
        return Json(items, JsonSerializerOptions.Default);
    }

    [HttpGet]
    public async Task<JsonResult> Earnings()
    {
        await InitializeCacheAsync();
        var items = TransactionsCache!.Where(x => x.Amount > 0).ToList();
        return Json(items, JsonSerializerOptions.Default);
    }

    [HttpGet]
    public async Task<JsonResult> Expenses()
    {
        await InitializeCacheAsync();
        var items = TransactionsCache!.Where(x => x.Amount <= 0).ToList();
        return Json(items, JsonSerializerOptions.Default);
    }

    [HttpGet]
    public async Task<JsonResult> Details(Guid? id)
    {
        await InitializeCacheAsync();

        if (id is null)
            return Error("Cannot process null transactions.");

        var item = TransactionsCache?.FirstOrDefault(m => m.Id == id);

        if (item is null)
            return Error("Unable to find transaction.");

        return Error("Unable to find transaction.");

        return Json(item, JsonSerializerOptions.Default);
    }

    [HttpPost]
    public async Task<JsonResult> Create(string transactionDate, string user, Currency currency, TransactionType type, string amount)
    {
        if (!ModelState.IsValid)
            return await CommitLogAsync("Error: model validations fail.", user);

        if (!DateTime.TryParse(transactionDate, out var date))
            return await CommitLogAsync("Failed to parse value " + nameof(Transaction.TransactionDate), user);

        if (!decimal.TryParse(amount, out var decimalAmount))
            return await CommitLogAsync("Failed to parse value: " + nameof(Transaction.Amount), user);

        if (date > DateTime.UtcNow)
            return await CommitLogAsync("Error: Submission of future transactions is forbidden.", user);

        if (decimalAmount <= 0 && type is TransactionType.Income ||
            decimalAmount > 0 && type is not TransactionType.Income)
            return await CommitLogAsync($"Error: {nameof(TransactionType)} selected is not compatible with {nameof(Transaction.Amount)}", user);

        await InitializeCacheAsync();

        if (decimalAmount <= 0 && (!OverviewCache.TryGetValue(user, out var overview) || overview.Balance < -decimalAmount))
            return await CommitLogAsync($"Error: user {user} does not have enough money to pay this transaction", user);

        var transaction = new Transaction
        {
            TransactionDate = date,
            User = user,
            Currency = currency,
            Type = type,
            Amount = decimalAmount
        };

        await CommitTransactionAsync(transaction);
        return await CommitLogAsync("Transaction submitted successfully!", user);        
    }

    private async Task InitializeCacheAsync()
    {
        if (TransactionsCache is null || OverviewCache is null)
        {
            TransactionsCache = await _dbContext.TransactionalData.OrderByDescending(x => x.TransactionDate).ToListAsync();

            OverviewCache = TransactionsCache!.GroupBy(x => x.User).Select(x =>
            {
                var plus = x.Where(t => t.Amount > 0).Sum(t => t.Amount);
                var minus = x.Where(t => t.Amount <= 0).Sum(t => t.Amount);
                return (User: x.Key, Earnings: plus, Expenses: minus, Balance: plus + minus);
            })
            .ToDictionary(x => x.User);
        }
    }

    private async Task<JsonResult> CommitLogAsync(string msg, string? user)
    {
        var log = new ActivityLog() { User = user?? "", Activity = msg };
        _dbContext.Add(log);
        await _dbContext.SaveChangesAsync();
        return Json(new { message = log.Activity });
    }

    private async Task CommitTransactionAsync(Transaction transaction)
    {
        transaction.Id = Guid.NewGuid();
        _dbContext.Add(transaction);
        await _dbContext.SaveChangesAsync();
        await InitializeCacheAsync();
        TransactionsCache?.Add(transaction);
    }

    private JsonResult Error(string message)
    {
        Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        return Json(new { msg = message }, JsonSerializerOptions.Default);
    }
}
