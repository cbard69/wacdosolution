

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using wacdoprojet.Models;

namespace wacdoprojet.Controllers
{ 
    public class RolesController : Controller
    {
        private readonly UserManager<Collaborateur> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(UserManager<Collaborateur> userManager, RoleManager<IdentityRole> roleManager)
             { 
              _userManager = userManager;
              _roleManager = roleManager;
             }

    // Affiche la liste des utilisateurs et des rôles
    public async Task<IActionResult> Index()
    {
            var users = await _userManager.Users
                .Where(u => u.Connectable == true)
                .ToListAsync();
            var roles = _roleManager.Roles.ToList();
        ViewBag.Roles = roles;
        return View(users);
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
}
}