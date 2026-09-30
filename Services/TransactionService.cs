using ExpenseTrackerApi.Data;
using ExpenseTrackerApi.Middleware;
using ExpenseTrackerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerApi.Services;

public class TransactionService : ITransactionService
{
    private readonly TransactionDbContext _context;

    public TransactionService(TransactionDbContext context)
    {
        _context = context;
    }

    public async Task<List<Transaction>> GetAllAsync()
    {
        return await _context.Transactions.ToListAsync();
    }

    public async Task<Transaction> AddAsync(Transaction transaction)
    {

        if (transaction.Amount > 1_000_000)
        {
            throw new BadRequestException("Transaction amount cannot exceed 1,000,000");
        }

        transaction.CreatedAt = DateTime.UtcNow;

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return transaction;
    }

    public async Task<Transaction?> GetByIdAsync(int id)
    {
        return await _context.Transactions.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var transaction = await GetByIdAsync(id);

        if (transaction == null)
        {
            throw new NotFoundException("Transaction not found");
        }

        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<Transaction?> UpdateAsync(int id, Transaction transaction)
    {
        var existingTransaction = await GetByIdAsync(id);

        if (existingTransaction == null)
        {
            throw new NotFoundException("Transaction not found");
        }

        if (transaction.Amount > 1_000_000)
        {
            throw new BadRequestException("Transaction amount cannot exceed 1,000,000");
        }

        existingTransaction.Amount = transaction.Amount;
        existingTransaction.Type = transaction.Type;
        existingTransaction.Category = transaction.Category;
        existingTransaction.Description = transaction.Description;

        await _context.SaveChangesAsync();

        return existingTransaction;
    }

}