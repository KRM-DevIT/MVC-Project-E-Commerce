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

        [DataType(DataType.ImageUrl)]
        public string? RoleImageURL { get; set; }

    }

}