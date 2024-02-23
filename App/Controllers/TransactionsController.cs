using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App.Data;
using Model;
using System.Text.Json;
using System.Net;
using System.Collections.Concurrent;
using App.Extensions;
using Model.Enums;
using Microsoft.AspNetCore.SignalR.Client;

namespace App.Controllers;

public class TransactionsController : Controller
{
    private readonly DbCatalogContext _dbContext;

    public HubConnection Connection { get; }

    private ConcurrentDictionary<Guid, Transaction>? TransactionsCache { get; set; }

    private ConcurrentDictionary<string, Balance> BalanceByUser { get; set; }

    public TransactionsController(DbCatalogContext dbContext)
    {
        _dbContext = dbContext;
        Connection = new HubConnectionBuilder().WithUrl(Consts.MessageHubAddress).WithAutomaticReconnect().Build();
    }

    [HttpGet]
    public async Task<JsonResult> Overview()
    {
        await UpdateCacheAsync();
        var items = BalanceByUser.Values.Select(x => new 
        {
            User = x.User,
            Earnings = x.Earnings,
            Expenses = x.Expenses,
            Balance = x.Value
        }).ToArray();
        return Json(items, JsonSerializerOptions.Default);
    }

    [HttpGet]
    public async Task<JsonResult> Earnings()
    {
        await UpdateCacheAsync();
        var items = TransactionsCache!.Values.Where(x => x.Amount > 0).OrderByDescending(x => x.TransactionDate).ToList();
        return Json(items, JsonSerializerOptions.Default);
    }

    [HttpGet]
    public async Task<JsonResult> Expenses()
    {
        await UpdateCacheAsync();
        var items = TransactionsCache!.Values.Where(x => x.Amount <= 0).OrderByDescending(x => x.TransactionDate).ToList();
        return Json(items, JsonSerializerOptions.Default);
    }

    [HttpGet]
    public async Task<JsonResult> Details(Guid id)
    {
        await UpdateCacheAsync();
           
        if (!TransactionsCache.TryGetValue(id, out var item))
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

        await UpdateCacheAsync();

        if (decimalAmount <= 0 && (!BalanceByUser.TryGetValue(user, out var balance) || balance.Value < -decimalAmount))
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

    private async Task UpdateCacheAsync(Transaction? transaction = null)
    {
        if (TransactionsCache is null || BalanceByUser is null)
        {
            TransactionsCache = (await _dbContext.TransactionalData.ToArrayAsync()).ToConcurrentDictionary(x => x.Id);

            BalanceByUser = TransactionsCache!.Values.GroupBy(x => x.User).Select(x =>
            {
                var plus = x.Where(t => t.Amount > 0).Sum(t => t.Amount);
                var minus = x.Where(t => t.Amount <= 0).Sum(t => t.Amount);
                return new Balance(User: x.Key, Earnings: plus, Expenses: minus, Value: plus + minus);
            })
            .ToConcurrentDictionary(x => x.User);
        }

        if(transaction is not null)
        {
            TransactionsCache.TryAdd(transaction.Id, transaction);

            var balance = BalanceByUser[transaction.User];
            balance = balance with { Value = balance.Value + transaction.Amount };
            if (transaction.Amount > 0)
                BalanceByUser[transaction.User] = balance with { Earnings = balance.Earnings + transaction.Amount };
            else
                BalanceByUser[transaction.User] = balance with { Expenses = balance.Expenses + transaction.Amount };
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
        await UpdateCacheAsync(transaction);
        await BroadcastDataChangedAsync();
    }

    private async Task BroadcastDataChangedAsync()
    {
        await Connection.StartAsync();
        await Connection.SendAsync(Consts.SendMessage, GetType().Name, Consts.DataChanged);
        await Connection.StopAsync();
    }

    private JsonResult Error(string message)
    {
        Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        return Json(new { msg = message }, JsonSerializerOptions.Default);
    }
}
