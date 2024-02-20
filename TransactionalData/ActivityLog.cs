using System.ComponentModel.DataAnnotations;

namespace Model;

public record ActivityLog()
{
    [Key]
    public Guid Id { get; set; }

    [DataType(DataType.Date)]
    public DateTime Date { get; } = DateTime.UtcNow;

    public string User { get; } = string.Empty;

    public string Activity { get; set; } = string.Empty;
}



