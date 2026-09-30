using ExpenseTrackerApi.DTOs;
using ExpenseTrackerApi.Middleware;
using ExpenseTrackerApi.Models;
using ExpenseTrackerApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ExpenseTrackerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet]
    public async Task<IActionResult> Get(string? category, string? type, DateTime? from, DateTime? to)
    {
        var transactions = await _transactionService.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(category))
        {
            transactions = transactions
                .Where(x => x.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(type))
        {
            transactions = transactions
                .Where(x => x.Type.Equals(type, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (from.HasValue)
        {
            transactions = transactions
                .Where(x => x.CreatedAt >= from.Value)
                .ToList();
        }

        if (to.HasValue)
        {
            var endDate = to.Value.Date.AddDays(1);

            transactions = transactions
                .Where(x => x.CreatedAt < endDate)
                .ToList();
        }

        var response = transactions.Select(transaction => new TransactionResponseDto
        {
            Id = transaction.Id,
            Amount = transaction.Amount,
            Type = transaction.Type,
            Category = transaction.Category,
            Description = transaction.Description,
            CreatedAt = transaction.CreatedAt
        }).ToList();

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Add(TransactionCreateDto dto)
    {
        var transaction = new Transaction
        {
            Amount = dto.Amount,
            Type = dto.Type,
            Category = dto.Category,
            Description = dto.Description
        };

        var addedTransaction = await _transactionService.AddAsync(transaction);

        return CreatedAtAction(
            nameof(GetById),
            new { id = addedTransaction.Id },
            addedTransaction);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var transaction = await _transactionService.GetByIdAsync(id);

        if (transaction == null)
        {
            throw new NotFoundException("Transaction not found");
        }

        var response = new TransactionResponseDto
        {
            Id = transaction.Id,
            Amount = transaction.Amount,
            Type = transaction.Type,
            Category = transaction.Category,
            Description = transaction.Description,
            CreatedAt = transaction.CreatedAt
        };

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _transactionService.DeleteAsync(id);

        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, TransactionUpdateDto dto)
    {
        var transaction = new Transaction
        {
            Amount = dto.Amount,
            Type = dto.Type,
            Category = dto.Category,
            Description = dto.Description
        };

        var updatedTransaction = await _transactionService.UpdateAsync(id, transaction);

        return Ok(updatedTransaction);
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var transactions = await _transactionService.GetAllAsync();

        var totalIncome = transactions
                .Where(x => x.Type == "Income")
                .Sum(x => x.Amount);

        var totalExpense = transactions
                .Where(x => x.Type == "Expense")
                .Sum(x => x.Amount);

        var balance = totalIncome - totalExpense;

        return Ok(new
        {
            totalIncome,
            totalExpense,
            balance
        });
    }
}