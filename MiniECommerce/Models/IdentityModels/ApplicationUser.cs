using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MiniECommerce.Models.IdentityModels
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(20, ErrorMessage = "First Name Can't Exceed 20 Characters")]
        public string FirstName { get; set; } = null!;
        [Required]
        [StringLength(20, ErrorMessage = "Last Name Can't Exceed 20 Characters")]
        public string LastName { get; set; } = null!;

        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }

        public bool IsActive { get; set; } = true;

        [DataType(DataType.ImageUrl)]
        public string? ProfilePicture { get; set; }


        // Nav 
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
        public ICollection<Order> Orders { get;set; } = new List<Order>();
    }
}
