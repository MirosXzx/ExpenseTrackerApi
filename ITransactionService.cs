using ExpenseTrackerApi.Models;

namespace ExpenseTrackerApi.Services;

public interface ITransactionService
{
    Task<List<Transaction>> GetAllAsync();

    Task<Transaction> AddAsync(Transaction transaction);

    Task<Transaction?> GetByIdAsync(int id);

    Task<bool> DeleteAsync(int id);

    Task<Transaction?> UpdateAsync(int id, Transaction transaction);

}