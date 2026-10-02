using ExpenseTrackerApi.Controllers;
using ExpenseTrackerApi.DTOs;
using ExpenseTrackerApi.Middleware;
using ExpenseTrackerApi.Models;
using ExpenseTrackerApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ExpenseTrackerApi.Tests;

public class TransactionsControllerTests
{
    [Fact]
    public async Task GetById_ShouldReturnOk_WhenTransactionExists()
    {
        var transaction = new Transaction
        {
            Id = 1,
            Amount = 100,
            Type = "Expense",
            Category = "Food",
            Description = "Dinner"
        };

        var mockService = new Mock<ITransactionService>();

        mockService
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(transaction);

        var controller = new TransactionsController(mockService.Object);

        var result = await controller.GetById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetById_ShouldThrow_WhenTransactionDoesNotExist()
    {
        var mockService = new Mock<ITransactionService>();

        mockService
            .Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Transaction?)null);

        var controller = new TransactionsController(mockService.Object);

        await Assert.ThrowsAsync<ExpenseTrackerApi.Middleware.NotFoundException>(
            () => controller.GetById(999));
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenTransactionExists()
    {
        var mockService = new Mock<ITransactionService>();

        mockService
            .Setup(x => x.DeleteAsync(1))
            .ReturnsAsync(true);

        var controller = new TransactionsController(mockService.Object);

        var result = await controller.Delete(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Add_ShouldReturnCreated_WhenTransactionIsValid()
    {
        var transaction = new Transaction
        {
            Id = 1,
            Amount = 100,
            Type = "Expense",
            Category = "Food",
            Description = "Dinner"
        };

        var mockService = new Mock<ITransactionService>();

        mockService
            .Setup(x => x.AddAsync(It.IsAny<Transaction>()))
            .ReturnsAsync(transaction);

        var controller = new TransactionsController(mockService.Object);

        var dto = new ExpenseTrackerApi.DTOs.TransactionCreateDto
        {
            Amount = 100,
            Type = "Expense",
            Category = "Food",
            Description = "Dinner"
        };

        var result = await controller.Add(dto);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);

        Assert.Equal(nameof(TransactionsController.GetById), createdResult.ActionName);
        Assert.NotNull(createdResult.Value);
    }

    [Fact]
    public async Task Update_ShouldReturnOk_WhenTransactionExists()
    {
        var transaction = new Transaction
        {
            Id = 1,
            Amount = 200,
            Type = "Expense",
            Category = "Transport",
            Description = "Taxi"
        };

        var mockService = new Mock<ITransactionService>();

        mockService
            .Setup(x => x.UpdateAsync(1, It.IsAny<Transaction>()))
            .ReturnsAsync(transaction);

        var controller = new TransactionsController(mockService.Object);

        var dto = new ExpenseTrackerApi.DTOs.TransactionUpdateDto
        {
            Amount = 200,
            Type = "Expense",
            Category = "Transport",
            Description = "Taxi"
        };

        var result = await controller.Update(1, dto);

        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task Get_ShouldReturnOk_WithTransactions()
    {
        var transactions = new List<Transaction>
    {
        new Transaction
        {
            Id = 1,
            Amount = 100,
            Type = "Expense",
            Category = "Food",
            Description = "Dinner"
        },
        new Transaction
        {
            Id = 2,
            Amount = 500,
            Type = "Income",
            Category = "Salary",
            Description = "Salary"
        }
    };

        var mockService = new Mock<ITransactionService>();

        mockService
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(transactions);

        var controller = new TransactionsController(mockService.Object);

        var result = await controller.Get(null, null, null, null);

        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task Get_ShouldFilterByCategory()
    {
        var transactions = new List<Transaction>
    {
        new Transaction
        {
            Id = 1,
            Amount = 100,
            Type = "Expense",
            Category = "Food",
            Description = "Dinner"
        },
        new Transaction
        {
            Id = 2,
            Amount = 500,
            Type = "Income",
            Category = "Salary",
            Description = "Salary"
        }
    };

        var mockService = new Mock<ITransactionService>();

        mockService
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(transactions);

        var controller = new TransactionsController(mockService.Object);

        var result = await controller.Get("Food", null, null, null);

        var okResult = Assert.IsType<OkObjectResult>(result);

        var resultList = Assert.IsAssignableFrom<List<TransactionResponseDto>>(
            okResult.Value);

        Assert.Single(resultList);
        Assert.Equal("Food", resultList[0].Category);
    }

    [Fact]
    public async Task Get_ShouldFilterByType()
    {
        var transactions = new List<Transaction>
    {
        new Transaction
        {
            Id = 1,
            Amount = 100,
            Type = "Expense",
            Category = "Food",
            Description = "Dinner"
        },
        new Transaction
        {
            Id = 2,
            Amount = 500,
            Type = "Income",
            Category = "Salary",
            Description = "Salary"
        }
    };

        var mockService = new Mock<ITransactionService>();

        mockService
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(transactions);

        var controller = new TransactionsController(mockService.Object);

        var result = await controller.Get(null, "Income", null, null);

        var okResult = Assert.IsType<OkObjectResult>(result);

        var resultList = Assert.IsAssignableFrom<List<TransactionResponseDto>>(
            okResult.Value);

        Assert.Single(resultList);
        Assert.Equal("Income", resultList[0].Type);
    }

    [Fact]
    public async Task Get_ShouldFilterByFromDate()
    {
        var transactions = new List<Transaction>
    {
        new Transaction
        {
            Id = 1,
            Amount = 100,
            Type = "Expense",
            Category = "Food",
            Description = "Old",
            CreatedAt = new DateTime(2026, 9, 1)
        },
        new Transaction
        {
            Id = 2,
            Amount = 200,
            Type = "Expense",
            Category = "Food",
            Description = "New",
            CreatedAt = new DateTime(2026, 9, 25)
        }
    };

        var mockService = new Mock<ITransactionService>();

        mockService
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(transactions);

        var controller = new TransactionsController(mockService.Object);

        var result = await controller.Get(
            null,
            null,
            new DateTime(2026, 9, 20),
            null);

        var okResult = Assert.IsType<OkObjectResult>(result);

        var resultList = Assert.IsAssignableFrom<List<TransactionResponseDto>>(
            okResult.Value);

        Assert.Single(resultList);
        Assert.Equal(2, resultList[0].Id);
    }

    [Fact]
    public async Task Get_ShouldFilterByToDate()
    {
        var transactions = new List<Transaction>
    {
        new Transaction
        {
            Id = 1,
            Amount = 100,
            Type = "Expense",
            Category = "Food",
            Description = "Before",
            CreatedAt = new DateTime(2026, 9, 10)
        },
        new Transaction
        {
            Id = 2,
            Amount = 200,
            Type = "Expense",
            Category = "Food",
            Description = "After",
            CreatedAt = new DateTime(2026, 9, 25)
        }
    };

        var mockService = new Mock<ITransactionService>();

        mockService
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(transactions);

        var controller = new TransactionsController(mockService.Object);

        var result = await controller.Get(
            null,
            null,
            null,
            new DateTime(2026, 9, 20));

        var okResult = Assert.IsType<OkObjectResult>(result);

        var resultList = Assert.IsAssignableFrom<List<TransactionResponseDto>>(
            okResult.Value);

        Assert.Single(resultList);
        Assert.Equal(1, resultList[0].Id);
    }

    [Fact]
    public async Task GetSummary_ShouldReturnCorrectTotals()
    {
        var transactions = new List<Transaction>
    {
        new Transaction
        {
            Id = 1,
            Amount = 1000,
            Type = "Income",
            Category = "Salary",
            Description = "Salary"
        },
        new Transaction
        {
            Id = 2,
            Amount = 300,
            Type = "Expense",
            Category = "Food",
            Description = "Dinner"
        }
    };

        var mockService = new Mock<ITransactionService>();

        mockService
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(transactions);

        var controller = new TransactionsController(mockService.Object);

        var result = await controller.GetSummary();

        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.NotNull(okResult.Value);

        var json = System.Text.Json.JsonSerializer.Serialize(okResult.Value);

        Assert.Contains("\"totalIncome\":1000", json);
        Assert.Contains("\"totalExpense\":300", json);
        Assert.Contains("\"balance\":700", json);
    }

    [Fact]
    public async Task GetSummary_ShouldReturnZeros_WhenThereAreNoTransactions()
    {
        var mockService = new Mock<ITransactionService>();

        mockService
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Transaction>());

        var controller = new TransactionsController(mockService.Object);

        var result = await controller.GetSummary();

        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.NotNull(okResult.Value);

        var json = System.Text.Json.JsonSerializer.Serialize(okResult.Value);

        Assert.Contains("\"totalIncome\":0", json);
        Assert.Contains("\"totalExpense\":0", json);
        Assert.Contains("\"balance\":0", json);
    }

    [Fact]
    public async Task Update_ShouldThrow_WhenTransactionDoesNotExist()
    {
        var mockService = new Mock<ITransactionService>();

        mockService
            .Setup(x => x.UpdateAsync(999, It.IsAny<Transaction>()))
            .ThrowsAsync(new NotFoundException("Transaction not found"));

        var controller = new TransactionsController(mockService.Object);

        var dto = new TransactionUpdateDto
        {
            Amount = 100,
            Type = "Expense",
            Category = "Food",
            Description = "Test"
        };

        await Assert.ThrowsAsync<NotFoundException>(
            () => controller.Update(999, dto));
    }

    [Fact]
    public async Task Delete_ShouldThrow_WhenTransactionDoesNotExist()
    {
        var mockService = new Mock<ITransactionService>();

        mockService
            .Setup(x => x.DeleteAsync(999))
            .ThrowsAsync(new NotFoundException("Transaction not found"));

        var controller = new TransactionsController(mockService.Object);

        await Assert.ThrowsAsync<NotFoundException>(
            () => controller.Delete(999));
    }

    [Fact]
    public async Task Add_ShouldThrow_WhenAmountExceedsLimit()
    {
        var mockService = new Mock<ITransactionService>();

        mockService
            .Setup(x => x.AddAsync(It.IsAny<Transaction>()))
            .ThrowsAsync(
                new BadRequestException(
                    "Transaction amount cannot exceed 1,000,000"));

        var controller = new TransactionsController(mockService.Object);

        var dto = new TransactionCreateDto
        {
            Amount = 1_000_001,
            Type = "Expense",
            Category = "Food",
            Description = "Test"
        };

        await Assert.ThrowsAsync<BadRequestException>(
            () => controller.Add(dto));
    }

    [Fact]
    public async Task Update_ShouldPassCorrectDataToService()
    {
        var mockService = new Mock<ITransactionService>();

        mockService
            .Setup(x => x.UpdateAsync(1, It.IsAny<Transaction>()))
            .ReturnsAsync(new Transaction
            {
                Id = 1,
                Amount = 500,
                Type = "Expense",
                Category = "Food",
                Description = "Dinner"
            });

        var controller = new TransactionsController(mockService.Object);

        var dto = new TransactionUpdateDto
        {
            Amount = 500,
            Type = "Expense",
            Category = "Food",
            Description = "Dinner"
        };

        await controller.Update(1, dto);

        mockService.Verify(
            x => x.UpdateAsync(
                1,
                It.Is<Transaction>(t =>
                    t.Amount == 500 &&
                    t.Type == "Expense" &&
                    t.Category == "Food" &&
                    t.Description == "Dinner")),
            Times.Once);
    }

    [Fact]
    public async Task Add_ShouldPassCorrectDataToService()
    {
        var mockService = new Mock<ITransactionService>();

        mockService
            .Setup(x => x.AddAsync(It.IsAny<Transaction>()))
            .ReturnsAsync((Transaction transaction) =>
            {
                transaction.Id = 1;
                return transaction;
            });

        var controller = new TransactionsController(mockService.Object);

        var dto = new TransactionCreateDto
        {
            Amount = 500,
            Type = "Income",
            Category = "Salary",
            Description = "Monthly salary"
        };

        await controller.Add(dto);

        mockService.Verify(
            x => x.AddAsync(
                It.Is<Transaction>(t =>
                    t.Amount == 500 &&
                    t.Type == "Income" &&
                    t.Category == "Salary" &&
                    t.Description == "Monthly salary")),
            Times.Once);
    }
}