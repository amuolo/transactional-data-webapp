using System.ComponentModel.DataAnnotations;

namespace Model;

public record ActivityLog()
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [DataType(DataType.Date)]
    public DateTime Date { get; set; } = DateTime.UtcNow;

    public string User { get; set; } = string.Empty;

    public string Activity { get; set; } = string.Empty;
}



