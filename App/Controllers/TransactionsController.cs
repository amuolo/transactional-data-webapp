using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App.Data;
using Model;

namespace App.Controllers;

public class TransactionsController : Controller
{
    private readonly DbCatalogContext _dbContext;

    public TransactionsController(DbCatalogContext dbContext)
    {
        _dbContext = dbContext;
    }

    // GET: TransactionalData
    public async Task<IActionResult> Index()
    {
        return View(await _dbContext.TransactionalData.ToListAsync());
    }

    // GET: TransactionalData/Details/5
    public async Task<IActionResult> Details(Guid? id)
    {
        if (id is null)
            return NotFound();

        var transactionalData = await _dbContext.TransactionalData.FirstOrDefaultAsync(m => m.Id == id);

        if (transactionalData is null)
            return NotFound();

        return View(transactionalData);
    }

    // GET: TransactionalData/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: TransactionalData/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind(nameof(Transaction.Id)+","+nameof(Transaction.TransactionDate)+","+nameof(Transaction.SettlementDate)
         +","+nameof(Transaction.User)+","+nameof(Transaction.Currency)+","+nameof(Transaction.Type)+","+nameof(Transaction.Amount))] 
        Transaction transaction)
    {
        if (ModelState.IsValid)
        {
            await CommitAsync(transaction);
            return RedirectToAction(nameof(Index));
        }
        return View(transaction);
    }

    [HttpPost]
    public async Task Create(string transactionDate, string user, Currency currency, TransactionType type, decimal amount)
    {
        var transaction = new Transaction
        {
            TransactionDate = DateTime.Parse(transactionDate),
            Currency = currency,
            Type = type,
            Amount = amount
        };

        await CommitAsync(transaction);
    }

    private async Task CommitAsync(Transaction transaction)
    {
        transaction.Id = Guid.NewGuid();
        _dbContext.Add(transaction);
        await _dbContext.SaveChangesAsync();
    }

    // GET: TransactionalData/Edit/5
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id is null)
            return NotFound();

        var transactionalData = await _dbContext.TransactionalData.FindAsync(id);
        
        if (transactionalData is null)
            return NotFound();

        return View(transactionalData);
    }

    // POST: TransactionalData/Edit/5
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

    // GET: TransactionalData/Delete/5
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id is null)
            return NotFound();

        var transactionalData = await _dbContext.TransactionalData.FirstOrDefaultAsync(m => m.Id == id);
        
        if (transactionalData is null)
            return NotFound();

        return View(transactionalData);
    }

    // POST: TransactionalData/Delete/5
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
