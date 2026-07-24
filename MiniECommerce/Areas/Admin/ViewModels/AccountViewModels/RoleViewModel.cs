using System.ComponentModel.DataAnnotations;

namespace MiniECommerce.Areas.Admin.ViewModels.AccountViewModels
{
    
    public class RoleViewModel
    {

        public string? Id { get; set; }
        [Required(ErrorMessage = "Role name is required")]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Can't Exceed 100 in Description")]
        public string? RoleDescription { get; set; }

        [Required(ErrorMessage = "Please choose a role icon.")]
        [Display(Name = "Role Icon")]
        [RegularExpression(
            @"^fa-solid fa-[a-z0-9-]+$",
            ErrorMessage = "Please choose a valid role icon.")]
        public string RoleImageURL { get; set; } = "fa-solid fa-user-shield";

    }

}
