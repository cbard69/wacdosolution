using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using wacdoprojet.Data;
using wacdoprojet.Models;

namespace wacdoprojet.Controllers
{
    [Authorize]
    public class AffectationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AffectationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Affectations
        [AllowAnonymous]
        
        public async Task<IActionResult> Index(string poste, string datedebut, string datefin, string ville)
        {
            ViewBag.PosteRecherche = poste;
            ViewBag.DatedebutRecherche = datedebut;
            ViewBag.DatefinRecherche = datefin;
            ViewBag.VilleRecherche = ville;

            var affectations = _context.Affectations
                .Include(a => a.Collaborateur)
                .Include(a => a.Restaurant)
                .Include(a => a.Poste)
                .AsQueryable();

            if (!string.IsNullOrEmpty(poste))
            {
                affectations = affectations.Where(a =>
                    a.Poste != null &&
                    a.Poste.Intituleposte != null &&
                    EF.Functions.Like(a.Poste.Intituleposte, $"%{poste}%"));
            }

            if (!string.IsNullOrEmpty(ville))
            {
                affectations = affectations.Where(a =>
                    a.Restaurant != null &&
                    a.Restaurant.ville != null &&
                    EF.Functions.Like(a.Restaurant.ville, $"%{ville}%"));
            }

            if (DateTime.TryParse(datedebut, out var debutDate))
            {
                affectations = affectations.Where(a => a.Datedebut.Date == debutDate.Date);
            }

            if (DateTime.TryParse(datefin, out var finDate))
            {
                affectations = affectations.Where(a => a.Datefin.HasValue && a.Datefin.Value.Date == finDate.Date);
            }

            var results = await affectations.ToListAsync();

            ViewBag.AucunResultat = (!string.IsNullOrEmpty(poste) || !string.IsNullOrEmpty(ville) || !string.IsNullOrEmpty(datedebut) || !string.IsNullOrEmpty(datefin))
                                    && !results.Any();

            return View(results);
        }


        // GET: Affectations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)

            {
                return NotFound();
            }

            //  var affectation = await _context.Affectations
            //    .FirstOrDefaultAsync(m => m.Id == id);
            var affectation = await _context.Affectations
           .Include(a => a.Collaborateur)
           .Include(a => a.Restaurant)
           .Include(a => a.Poste)
           .FirstOrDefaultAsync(m => m.Id == id);

            if (affectation == null)
            {
                return NotFound();
            }

            return View(affectation);
        }

        // GET: Affectations/Create
        public async Task<IActionResult> Create(int? restaurantId = null, string? collaborateurId = null, bool? retourrestaurant = null, bool? retourcollaborateur = null)
        {
            var affectation = new Affectation
            {
                CollaborateurId = collaborateurId ?? "",  // nécessaire si [required]
                RestaurantId = restaurantId ?? 0          // même remarque
            };

            ViewBag.RetourRestaurant = retourrestaurant;
            ViewBag.RetourCollaborateur = retourcollaborateur;

            ViewData["PosteId"] = new SelectList(_context.Postes, "Id", "Intituleposte");
            ViewData["CollaborateurId"] = new SelectList(_context.Collaborateurs, "Id", "Nom", affectation.CollaborateurId);
            ViewData["RestaurantId"] = restaurantId.HasValue
                ? (object)restaurantId.Value
                : new SelectList(_context.Restaurants, "Id", "name");

            // Pour l'affichage des affectations existantes (si retour depuis fiche restaurant)
            if (restaurantId.HasValue)
            {
                var restaurant = await _context.Restaurants
                    .Include(r => r.RestaurantAffectations!)
                        .ThenInclude(a => a.Collaborateur)
                    .Include(r => r.RestaurantAffectations!)
                        .ThenInclude(a => a.Poste)
                    .FirstOrDefaultAsync(r => r.Id == restaurantId);

                if (restaurant == null)
                    return NotFound();

                ViewBag.AffectationsExistantes = restaurant.RestaurantAffectations;
            }

            return View(affectation);
        }

        // POST: Affectations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Create(Affectation affectation, bool? retourrestaurant, bool? retourcollaborateur)
        {
            if (ModelState.IsValid)
            {
                _context.Add(affectation);
                await _context.SaveChangesAsync();

                if (retourrestaurant == true)
                    return RedirectToAction("Details", "Restaurants", new { id = affectation.RestaurantId });

                if (retourcollaborateur == true)
                    return RedirectToAction("Details", "Collaborateurs", new { id = affectation.CollaborateurId });

                return RedirectToAction("Index");
            }

            ViewData["RestaurantId"] = new SelectList(_context.Restaurants, "Id", "name", affectation.RestaurantId);
            ViewData["CollaborateurId"] = new SelectList(_context.Collaborateurs, "Id", "Nom", affectation.CollaborateurId);

            return View(affectation);
        }

        // GET: Affectations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var affectation = await _context.Affectations.FindAsync(id);
            if (affectation == null)
            {
                return NotFound();
            }

            // On met à dispo les différentes infos dont on peut avoir besoin pour mettre à jour  les affactations
            ViewData["PosteId"] = new SelectList(_context.Postes, "Id", "Intituleposte"); // liste de choix
            ViewData["CollaborateurId"] = new SelectList(_context.Collaborateurs, "Id", "Nom");
            ViewData["RestaurantId"] = new SelectList(_context.Restaurants, "Id", "name", "ville");

            return View(affectation);
        }

        // POST: Affectations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RestaurantId,PosteId,CollaborateurId,Datedebut,Datefin")] Affectation affectation , bool? retourCollaborateur)
        {
            if (id != affectation.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(affectation);
                    await _context.SaveChangesAsync();

                    // Si on veut revenir sur le détail du collaborateur
                    if (retourCollaborateur == true)
                    {
                        return RedirectToAction("Details", "Collaborateurs", new { id = affectation.CollaborateurId });
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Affectations.Any(e => e.Id == affectation.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            // ... viewbags si retour
            ViewData["PosteId"] = new SelectList(_context.Postes, "Id", "Intituleposte", affectation.PosteId);
            ViewData["CollaborateurId"] = new SelectList(_context.Collaborateurs, "Id", "Nom", affectation.CollaborateurId);
            ViewData["RestaurantId"] = new SelectList(_context.Restaurants, "Id", "name", affectation.RestaurantId);


            return View(affectation);
        }
        
        
        
        
        
        // GET: Affectations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var affectation = await _context.Affectations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (affectation == null)
            {
                return NotFound();
            }
            ViewData["PosteId"] = new SelectList(_context.Postes, "Id", "Intituleposte", affectation.PosteId);
            ViewData["CollaborateurId"] = new SelectList(_context.Collaborateurs, "Id", "Nom", affectation.CollaborateurId);
            ViewData["RestaurantId"] = new SelectList(_context.Restaurants, "Id", "name", affectation.RestaurantId);

            return View(affectation);
        }

        // POST: Affectations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var affectation = await _context.Affectations.FindAsync(id);
            if (affectation != null)
            {
                _context.Affectations.Remove(affectation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AffectationExists(int id)
        {
            return _context.Affectations.Any(e => e.Id == id);
        }
    }
}
