namespace ExpenseTrackerApi.DTOs;

public class TransactionResponseDto
{
    public int Id { get; set; }

    public decimal Amount { get; set; }

    public string Type { get; set; } = "";

    public string Category { get; set; } = "";

    public string Description { get; set; } = "";

    public DateTime CreatedAt { get; set; }
}