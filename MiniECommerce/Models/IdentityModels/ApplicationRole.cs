using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MiniECommerce.Models.IdentityModels
{
    public class ApplicationRole : IdentityRole<string>
    {

        [StringLength(100,ErrorMessage ="Can't Exceed 100 in Description")]
        public string? RoleDescription { get; set; }

        [DataType(DataType.ImageUrl)]
        public string? RoleImageURL { get; set; }
    }
}
