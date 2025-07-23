

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using wacdoprojet.Data;
using wacdoprojet.Models;
using wacdoprojet.ViewModels;

namespace wacdoprojet.Controllers
{
    [Authorize]
    public class RolesController : Controller
    {
        private readonly UserManager<Collaborateur> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        public RolesController(UserManager<Collaborateur> userManager, RoleManager<IdentityRole> roleManager , ApplicationDbContext context)
             { 
              _userManager = userManager;
              _roleManager = roleManager;
              _context = context;
             }

        // Affiche la liste des utilisateurs et des rôles
        // on  crée une jointure entre les tables concernées qui va constituer les données  pour un nouveau Viewmodel qui servira à la vue 
        public IActionResult Index()
        {
            var roles = _context.Roles.ToList();

            var collaborateursAvecRoles = (
    from user in _context.Users
    where user.Connectable == true

    join userRole in _context.UserRoles on user.Id equals userRole.UserId into userRoles
    from ur in userRoles.DefaultIfEmpty() // LEFT JOIN UserRoles

    join role in _context.Roles on ur.RoleId equals role.Id into rolesJoin
    from r in rolesJoin.DefaultIfEmpty() // LEFT JOIN Roles

    select new UserWithRoleViewModel
    {
        UserId = user.Id,
        UserName = user.UserName,
        RoleName = r != null ? r.Name : "Aucun", // ou null
        SelectedRoleId = r != null ? r.Id : null,
        AvailableRoles = _context.Roles.ToList() // ou passé depuis dehors
    }
).ToList();

            return View(collaborateursAvecRoles);
        }
        [HttpPost]
    public async Task<IActionResult> AssignRole(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null && await _roleManager.RoleExistsAsync(role))
        {
            await _userManager.AddToRoleAsync(user, role);
        }

        return RedirectToAction("Index");
    }

        
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRole(string userId, [FromForm] Dictionary<string, string> updatedRoles)
        {
            if (!string.IsNullOrEmpty(userId) && updatedRoles.TryGetValue(userId, out var newRoleId))
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, currentRoles); // supprime tous les rôles actuels
                    var role = await _roleManager.FindByIdAsync(newRoleId);
                    if (role != null)
                    {
                        await _userManager.AddToRoleAsync(user, role.Name);
                    }
                }
            }

            return RedirectToAction("Index");
        }




    }
}