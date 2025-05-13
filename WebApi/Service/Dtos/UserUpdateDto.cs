using System.ComponentModel.DataAnnotations;

namespace WebApi.Service.Dtos;

public class UserUpdateDto
{
    public string Id { get; set; } = null!;

    [Display(Name = "Email Address")]
    [Required(ErrorMessage = "You must enter a valid Email Address")]
    [DataType(DataType.EmailAddress)]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email need to be formatted as <name@domain.com>")]
    public string Email { get; set; } = null!;

    [Display(Name = "Phone Number")]
    [DataType(DataType.PhoneNumber)]
    [RegularExpression(@"^(?:\+46\s?|0)\d(?:[\s]?\d){8,11}$", ErrorMessage = "Phonenumber needs to be formatted as:\n+46 701231234 OR\n0701231234")]
    public string? PhoneNumber { get; set; }
}
