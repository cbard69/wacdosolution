using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using wacdoprojet.Data;
using wacdoprojet.Models;
using System.Composition;
using Humanizer;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class CollaborateursController : Controller
{
    private readonly UserManager<Collaborateur> _userManager;
    private readonly ApplicationDbContext _context;
    // besoinde Usermanager pour UserManager<Collaborateur> est orienté utilisateur (authentification, sécurité, mot de passe, rôles, etc.).
    // Mais il n’expose pas directement les propriétés personnalisées que l'on veut modifier ou sauvegarder.

    //ApplicationDbContext pour manipuler les propriétés personnalisées  ajoutées à la classe Collaborateur en dehors de ce que IdentityUser fournit.


    public CollaborateursController(UserManager<Collaborateur> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    

    // GET: Collaborateurs/Create
    public IActionResult Create()
    {
        var collaborateur = new Collaborateur
        {
            Nom = "",
            Datepremiereembauche = DateTime.Today
        };
        return View(collaborateur);
    }

    // POST: Collaborateurs/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
  
    public async Task<IActionResult> Create(Collaborateur collaborateur, string? password)
    {
        if (!ModelState.IsValid)
            return View(collaborateur);

        // S’il est connectable, créer le compte Identity
        if (collaborateur.Connectable == true)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Password", "Le mot de passe est requis pour un administrateur.");
                return View(collaborateur);
            }

            // Email et UserName requis pour Identity
            collaborateur.UserName = collaborateur.Email;

            // permettre qu'un utilisateur puisse se connecter
            collaborateur.EmailConfirmed = true;

            var result = await _userManager.CreateAsync(collaborateur, password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return View(collaborateur);
            }
        }
        else
        {
            
            _context.Add(collaborateur);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }



    // GET: Collaborateurs
  
    public async Task<IActionResult> Index(string name, string prenom, string email, bool nonAffectes = false)
    {
        ViewBag.NomRecherche = name;
        ViewBag.PrenomRecherche = prenom;
        ViewBag.EmailRecherche = email;
        ViewBag.NonAffectes = nonAffectes;

        var collaborateurs = _userManager.Users
            .Include(c => c.Collaborateuraffectation)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            collaborateurs = collaborateurs.Where(c => c.Nom != null && c.Nom.Contains(name));

        if (!string.IsNullOrWhiteSpace(prenom))
            collaborateurs = collaborateurs.Where(c => c.Prénom != null && c.Prénom.Contains(prenom));

        if (!string.IsNullOrWhiteSpace(email))
            collaborateurs = collaborateurs.Where(c => c.Email != null && c.Email.Contains(email));

        if (nonAffectes)
            collaborateurs = collaborateurs.Where(c =>
                c.Collaborateuraffectation == null ||
                !c.Collaborateuraffectation.Any(a => a.Datefin == null));

        var list = await collaborateurs.ToListAsync();

        ViewBag.AucunResultat = !list.Any() && (!string.IsNullOrWhiteSpace(name) || !string.IsNullOrWhiteSpace(prenom) || !string.IsNullOrWhiteSpace(email) || nonAffectes);

        return View(list);
    }



    // GET: Collaborateurs/Details/id
    public async Task<IActionResult> Details(string id, string restaurant, string poste, DateTime? dateDebut)
    {
        if (id == null) return NotFound();

        var collaborateur = await _context.Collaborateurs
            .Include(c => c.Collaborateuraffectation)
                .ThenInclude(a => a.Restaurant)
            .Include(c => c.Collaborateuraffectation)
                .ThenInclude(a => a.Poste)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (collaborateur == null) return NotFound();

        // On récupère toutes  ses affectations

        var affectations = collaborateur.Collaborateuraffectation.AsQueryable();

        // On filtre dans la liste uniquement ses affectations
        
        if (!string.IsNullOrWhiteSpace(restaurant))
            affectations = affectations.Where(a => a.Restaurant.name != null && a.Restaurant.name.Contains(restaurant, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(poste))
            affectations = affectations.Where(a => a.Poste.Intituleposte != null && a.Poste.Intituleposte.Contains(poste, StringComparison.OrdinalIgnoreCase));

        if (dateDebut.HasValue)
            affectations = affectations.Where(a => a.Datedebut.Date == dateDebut.Value.Date);

        // Remplacement de la liste d’origine par la liste filtrée
        collaborateur.Collaborateuraffectation = affectations.ToList();

        ViewBag.FiltreRestaurant = restaurant;
        ViewBag.FiltrePoste = poste;
        ViewBag.FiltreDateDebut = dateDebut?.ToString("yyyy-MM-dd");

        return View(collaborateur);
    }


    // GET: Collaborateurs/Edit/id

    public async Task<IActionResult> Edit(string id)
    {
        if (id == null)
            return NotFound();

        var collaborateur = await _context.Users.FindAsync(id);

        if (collaborateur == null)
            return NotFound();

        return View(collaborateur);
    }


    // POST: Collaborateurs/Edit/id
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, Collaborateur collaborateur, string? newPassword)
    {
        if (id != collaborateur.Id)
            return NotFound();

        if (!ModelState.IsValid)
            return View(collaborateur);

        try
        {
            var userToUpdate = await _userManager.FindByIdAsync(collaborateur.Id);
            if (userToUpdate == null)
                return NotFound();

            // Mise à jour des champs standards
            userToUpdate.Nom = collaborateur.Nom;
            userToUpdate.Prénom = collaborateur.Prénom;
            userToUpdate.Email = collaborateur.Email;
            userToUpdate.UserName = collaborateur.Email;
            userToUpdate.Connectable = collaborateur.Connectable;
            userToUpdate.Datepremiereembauche = collaborateur.Datepremiereembauche;

            //=== DEBUT ======  IL N'EST PLUS ADMINISTRATEUR ==========

            if (!collaborateur.Connectable)
            {
                // Supprimer mot de passe s'il en a un
                var hasPassword = await _userManager.HasPasswordAsync(userToUpdate);
                if (hasPassword)
                {
                    var pwdResult = await _userManager.RemovePasswordAsync(userToUpdate);
                    if (!pwdResult.Succeeded)
                    {
                        foreach (var error in pwdResult.Errors)
                            ModelState.AddModelError(string.Empty, error.Description);
                        return View(collaborateur);
                    }
                }

                // Supprimer son UserName pour bloquer l'authentification
                userToUpdate.UserName = null;
            }

            //==== FIN =====  IL N'EST PLUS ADMINISTRATEUR ==========

            //==== DEBUT ==== IL EST COCHE ADMINISTRATEUR et IL L'ETAIT DEJA ===== ON REGARDE SI BESOIN MAJ PASSWORD
            else if (!string.IsNullOrWhiteSpace(newPassword) && (userToUpdate.Connectable = true))
            // Si un nouveau mot de passe est fourni (cela suppose qu'on veut le changer)

            {
                var hasPassword = await _userManager.HasPasswordAsync(userToUpdate);

                IdentityResult pwdResult;

                if (hasPassword)
                {
                    // Supprimer l'ancien mot de passe
                    pwdResult = await _userManager.RemovePasswordAsync(userToUpdate);
                    if (!pwdResult.Succeeded)
                    {
                        foreach (var error in pwdResult.Errors)
                            ModelState.AddModelError(string.Empty, error.Description);
                        return View(collaborateur);
                    }
                }

                // Ajouter le nouveau mot de passe
                pwdResult = await _userManager.AddPasswordAsync(userToUpdate, newPassword);
                if (!pwdResult.Succeeded)
                {
                    foreach (var error in pwdResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return View(collaborateur);
                }
            }


            //==== FIN ==== IL EST COCHE ADMINISTRATEUR et IL L'ETAIT DEJA ===== ON REGARDE SI BESOIN MAJ PASSWORD

            //==== DEBUT === IL EST ADMINISTRATEUR et IL NE L ETAIT PAS 
            else if (userToUpdate.Connectable == false)
            {
                if (string.IsNullOrWhiteSpace(newPassword))
                {
                    ModelState.AddModelError("Password", "Le mot de passe est requis pour un administrateur.");
                    return View(collaborateur);
                }



                var hasPassword = await _userManager.HasPasswordAsync(userToUpdate);

                IdentityResult pwdResult;

                if (hasPassword)
                {
                    // Supprimer l'ancien mot de passe
                    pwdResult = await _userManager.RemovePasswordAsync(userToUpdate);
                    if (!pwdResult.Succeeded)
                    {
                        foreach (var error in pwdResult.Errors)
                            ModelState.AddModelError(string.Empty, error.Description);
                        return View(collaborateur);
                    }
                }

                // Ajouter le nouveau mot de passe
                pwdResult = await _userManager.AddPasswordAsync(userToUpdate, newPassword);
                if (!pwdResult.Succeeded)
                {
                    foreach (var error in pwdResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return View(collaborateur);
                }


            }

            //==== FIN === IL EST ADMINISTRATEUR et IL NE L ETAIT PAS 

            // Mise à jour finale
            var result = await _userManager.UpdateAsync(userToUpdate);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return View(collaborateur);
            }
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Users.Any(e => e.Id == id))
                return NotFound();
            else
                throw;
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Collaborateurs/Delete/id
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        return View(user);
    }

    // POST: Collaborateurs/Delete/id
   
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        if (id == null)
            return NotFound();

        // var collaborateur = await _context.Collaborateurs
        //   .FirstOrDefaultAsync(c => c.Id == id);

        var collaborateur = await _context.Collaborateurs
           .Include(c => c.Collaborateuraffectation)
               .ThenInclude(a => a.Restaurant)
           .Include(c => c.Collaborateuraffectation)
               .ThenInclude(a => a.Poste)
           .FirstOrDefaultAsync(c => c.Id == id);



        if (collaborateur == null)
            return NotFound();

        // On regarde si  le collaborateur a des affectations
        var aDesAffectations = await _context.Affectations.AnyAsync(a => a.CollaborateurId == id);

        // s'il a des affections  en cours ou passées, la suppresion est impossible
        if (aDesAffectations)
        {
            TempData["ErreurSuppression"] = "Impossible de supprimer ce collaborateur car il a des affectations.";
            //            return RedirectToAction("Details", new { id });
            return View("Delete", collaborateur);      
        }

        _context.Collaborateurs.Remove(collaborateur);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

}
