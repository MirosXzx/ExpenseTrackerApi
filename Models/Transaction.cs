using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackerApi.Models;

public class Transaction
{
    public int Id { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [RegularExpression("Income|Expense")]

    public string Type { get; set; } = "";

    [Required]

    public string Category { get; set; } = "";

    [StringLength(500)]

    public string Description { get; set; } = "";

    public DateTime CreatedAt { get; set; }
}