using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

public class VerifyIdentityViewModel
{
    [Required]
    [StringLength(40, ErrorMessage = "First Name cannot exceed 40 characters.")]
    public string FirstName { get; set; }

    [Required]
    [StringLength(40, ErrorMessage = "Last Name cannot exceed 40 characters.")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Birth date is required.")]
    [DataType(DataType.Date)]
    public DateTime BarthDate { get; set; }

    [Required]
    [StringLength(20, ErrorMessage = "ID number cannot exceed 20 characters.")]
    public string IdNumber { get; set; }


    [Required(ErrorMessage = "National ID Front Image is required.")]
    public IFormFile NationalIdFrontImage { get; set; }

    [Required(ErrorMessage = "National ID Back Image is required.")]
    public IFormFile NationalIdBackImage { get; set; }
}
