using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("DateInformation")]
public class DateInformation
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string WorkDay { get; set; } = null!;
    
    [Range(1900,2100)]
    public int Year { get; set; }

    [Range(1,12)]
    public int Month { get; set; }

}