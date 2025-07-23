using System.Data;
using Microsoft.AspNetCore.Identity;

namespace wacdoprojet.ViewModels
{
    
        public class UserWithRoleViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }

        public string? SelectedRoleId { get; set; } 
        public string RoleName { get; set; } 
        public List<IdentityRole> AvailableRoles { get; set; } = new(); 
    }

    
}
