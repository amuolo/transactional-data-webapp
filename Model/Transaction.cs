using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Model.Enums;

namespace Model;

public record Transaction
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Transaction Date")]
    public DateTime TransactionDate { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Settlement Date")]
    public DateTime SettlementDate { get; set; } = DateTime.UtcNow;

    [Required]
    [StringLength(60, MinimumLength = 3)]
    [RegularExpression(@"^(?![\s.]+$)[a-zA-Z\u00C0-\u017F\s]*$")]
    public string User { get; set; }

    [Required]
    [EnumDataType(typeof(Currency))]
    public Currency Currency { get; set; }

    [Required]
    [EnumDataType(typeof(TransactionType))]
    public TransactionType Type { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,8)")]
    public decimal Amount { get; set; }
}

