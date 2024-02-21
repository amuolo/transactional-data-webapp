using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App.Data;
using Model;

namespace App.Controllers;

public class TransactionsController : Controller
{
    private readonly DbCatalogContext _dbContext;

    private List<Transaction>? TransactionsCache { get; set; }

    public TransactionsController(DbCatalogContext dbContext)
    {
        _dbContext = dbContext;
    }

    // GET: Transactions
    public async Task<IActionResult> Index()
    {
        await InitializeCacheAsync();
        return View(TransactionsCache);
    }

    private async Task InitializeCacheAsync()
    {
        if (TransactionsCache is null)
            TransactionsCache = await _dbContext.TransactionalData.ToListAsync();
    }

    // GET: Transactions/Details/5
    public async Task<IActionResult> Details(Guid? id)
    {
        if (id is null)
            return NotFound();

        var transactionalData = await _dbContext.TransactionalData.FirstOrDefaultAsync(m => m.Id == id);

        if (transactionalData is null)
            return NotFound();

        return View(transactionalData);
    }

    [HttpPost]
    public async Task<string> Create(string transactionDate, string user, Currency currency, TransactionType type, string amount)
    {
        if (!DateTime.TryParse(transactionDate, out var date))
            return await CommitLogAsync("Failed to parse value " + nameof(Transaction.TransactionDate));

        if (!decimal.TryParse(amount, out var decimalAmount))
            return await CommitLogAsync("Failed to parse value: " + nameof(Transaction.Amount));

        if (date > DateTime.UtcNow)
            return await CommitLogAsync("Error: Submission of future transactions is forbidden.");

        if (decimalAmount < 0 && type is TransactionType.Income ||
            decimalAmount >= 0 && type is not TransactionType.Income)
            return await CommitLogAsync("Error: " + nameof(TransactionType) + " selected is not compatible with " + nameof(Transaction.Amount));

        var transaction = new Transaction
        {
            TransactionDate = date,
            User = user,
            Currency = currency,
            Type = type,
            Amount = decimalAmount
        };

        await CommitTransactionAsync(transaction);
        return "Transaction submission completed successfully!";        
    }

    private async Task<string> CommitLogAsync(string msg, string? user = default)
    {
        var log = new ActivityLog() { User = user?? "", Activity = msg };
        _dbContext.Add(log);
        await _dbContext.SaveChangesAsync();
        return msg;
    }

    private async Task CommitTransactionAsync(Transaction transaction)
    {
        transaction.Id = Guid.NewGuid();
        _dbContext.Add(transaction);
        await _dbContext.SaveChangesAsync();
        await InitializeCacheAsync();
        TransactionsCache?.Add(transaction);
    }

    // GET: Transactions/Edit/5
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id is null)
            return NotFound();

        var transactionalData = await _dbContext.TransactionalData.FindAsync(id);
        
        if (transactionalData is null)
            return NotFound();

        return View(transactionalData);
    }

    // POST: Transactions/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id,
        [Bind(nameof(Transaction.Id)+","+nameof(Transaction.TransactionDate)+","+nameof(Transaction.SettlementDate)
         +","+nameof(Transaction.User)+","+nameof(Transaction.Currency)+","+nameof(Transaction.Type)+","+nameof(Transaction.Amount))] 
        Transaction transactionalData)
    {
        if (id != transactionalData.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _dbContext.Update(transactionalData);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionalDataExists(transactionalData.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(transactionalData);
    }

    // GET: Transactions/Delete/5
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id is null)
            return NotFound();

        var transactionalData = await _dbContext.TransactionalData.FirstOrDefaultAsync(m => m.Id == id);
        
        if (transactionalData is null)
            return NotFound();

        return View(transactionalData);
    }

    // POST: Transactions/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var transactionalData = await _dbContext.TransactionalData.FindAsync(id);
        if (transactionalData != null)
            _dbContext.TransactionalData.Remove(transactionalData);

        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool TransactionalDataExists(Guid id)
    {
        return _dbContext.TransactionalData.Any(e => e.Id == id);
    }
}
