using System.ComponentModel.DataAnnotations;

namespace Model;

public record ActivityLog()
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; } = DateTime.UtcNow;

    [Required]
    [StringLength(60, MinimumLength = 3)]
    [RegularExpression(@"^(?![\s.]+$)[a-zA-Z\u00C0-\u017F\s]*$")]
    public string User { get; set; }
        
    [Required]
    public string Activity { get; set; }
}



