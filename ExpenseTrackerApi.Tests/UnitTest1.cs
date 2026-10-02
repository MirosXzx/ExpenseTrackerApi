using ExpenseTrackerApi.Data;
using ExpenseTrackerApi.Middleware;
using ExpenseTrackerApi.Models;
using ExpenseTrackerApi.Services;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerApi.Tests;

public class TransactionServiceTests
{
    private TransactionService CreateService(string databaseName)
    {
        var options = new DbContextOptionsBuilder<TransactionDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        var context = new TransactionDbContext(options);

        return new TransactionService(context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddTransaction()
    {
        var service = CreateService("TestDatabase");

        var transaction = new Transaction
        {
            Amount = 100,
            Type = "Expense",
            Category = "Food",
            Description = "Test"
        };

        var result = await service.AddAsync(transaction);

        Assert.NotEqual(0, result.Id);
        Assert.Equal(100, result.Amount);
        Assert.Equal("Food", result.Category);
    }

    [Fact]
    public async Task AddAsync_ShouldThrow_WhenAmountExceedsLimit()
    {
        var service = CreateService("TestDatabase2");

        var transaction = new Transaction
        {
            Amount = 1_000_001,
            Type = "Expense",
            Category = "Food",
            Description = "Test"
        };

        await Assert.ThrowsAsync<BadRequestException>(
            () => service.AddAsync(transaction));
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenAmountExceedsLimit()
    {
        var service = CreateService("TestDatabase3");

        var transaction = new Transaction
        {
            Amount = 100,
            Type = "Expense",
            Category = "Food",
            Description = "Test"
        };

        await service.AddAsync(transaction);

        var update = new Transaction
        {
            Amount = 1_000_001,
            Type = "Expense",
            Category = "Food",
            Description = "Updated"
        };

        await Assert.ThrowsAsync<BadRequestException>(
            () => service.UpdateAsync(transaction.Id, update));
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteTransaction()
    {
        var service = CreateService("TestDatabase4");

        var transaction = new Transaction
        {
            Amount = 100,
            Type = "Expense",
            Category = "Food",
            Description = "Test"
        };

        var addedTransaction = await service.AddAsync(transaction);

        await service.DeleteAsync(addedTransaction.Id);

        var result = await service.GetByIdAsync(addedTransaction.Id);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrow_WhenTransactionNotFound()
    {
        var service = CreateService("TestDatabase5");

        await Assert.ThrowsAsync<NotFoundException>(
            () => service.DeleteAsync(999));
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTransaction()
    {
        var service = CreateService("TestDatabase6");

        var transaction = new Transaction
        {
            Amount = 250,
            Type = "Expense",
            Category = "Food",
            Description = "Dinner"
        };

        var addedTransaction = await service.AddAsync(transaction);

        var result = await service.GetByIdAsync(addedTransaction.Id);

        Assert.NotNull(result);
        Assert.Equal(250, result.Amount);
        Assert.Equal("Dinner", result.Description);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenTransactionNotFound()
    {
        var service = CreateService("TestDatabase7");

        var result = await service.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTransactions()
    {
        var service = CreateService("TestDatabase8");

        await service.AddAsync(new Transaction
        {
            Amount = 100,
            Type = "Expense",
            Category = "Food",
            Description = "Dinner"
        });

        await service.AddAsync(new Transaction
        {
            Amount = 500,
            Type = "Income",
            Category = "Salary",
            Description = "Salary"
        });

        var result = await service.GetAllAsync();

        Assert.Equal(2, result.Count);
    }
    [Fact]
    public async Task UpdateAsync_ShouldUpdateTransaction()
    {
        var service = CreateService("TestDatabase9");

        var transaction = await service.AddAsync(new Transaction
        {
            Amount = 100,
            Type = "Expense",
            Category = "Food",
            Description = "Dinner"
        });

        var update = new Transaction
        {
            Amount = 200,
            Type = "Expense",
            Category = "Transport",
            Description = "Taxi"
        };

        var result = await service.UpdateAsync(transaction.Id, update);

        Assert.NotNull(result);
        Assert.Equal(200, result.Amount);
        Assert.Equal("Transport", result.Category);
        Assert.Equal("Taxi", result.Description);
    }
}