using System.ComponentModel.DataAnnotations;

namespace RoomBookingApp.Domain.BaseModels
{
  public abstract class RoomBookingBase : IValidatableObject
  {
    //[Required]
    [StringLength(80)]
    public string? FullName { get; set; } 
    //[Required]
    [StringLength(80)]
    [EmailAddress]
    public string? Email { get; set; }
    [DataType(DataType.Date)]
    public DateOnly Date { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
      if(Date <= DateOnly.FromDateTime(DateTime.Now))
      {
        yield return new ValidationResult("Date Must be In the Future", new[] { nameof(Date) });
      }
    }
  }
}