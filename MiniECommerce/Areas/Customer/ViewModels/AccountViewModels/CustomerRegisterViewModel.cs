using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MiniECommerce.Areas.Customer.ViewModels.AccountViewModels
{
    public class CustomerRegisterViewModel
    {
        [Required]
        [StringLength(20, ErrorMessage = "First Name Can't Exceed 20 Characters")]
        public string FirstName { get; set; } = null!;
        [Required]
        [StringLength(20, ErrorMessage = "Last Name Can't Exceed 20 Characters")]
        public string LastName { get; set; } = null!;
        
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "Password must be at least {2} characters long", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;


        [Required(ErrorMessage = "Please confirm your password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}


