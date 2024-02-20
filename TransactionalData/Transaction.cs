using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model;

public record Transaction()
{
    [Key]
    public Guid Id { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Transaction Date")]
    public DateTime TransactionDate { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Settlement Date")]
    public DateTime SettlementDate { get; set; } = DateTime.UtcNow;

    public string User { get; set; } = string.Empty;

    [DataType(DataType.Currency)]
    public string Currency { get; set; } = "CHF";

    [EnumDataType(typeof(TransactionType))]
    public TransactionType Type { get; set; }

    [Column(TypeName = "decimal(18,8)")]
    public decimal Amount { get; set; }
}

