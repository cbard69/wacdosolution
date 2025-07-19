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
    public class RestaurantsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RestaurantsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [AllowAnonymous]
        // GET: Restaurants
        public async Task<IActionResult> Index(string name, string ville, string CP,  string sortOrder)
        {
            ViewBag.NomRecherche = name;
            ViewBag.VilleRecherche = ville;
            ViewBag.CPRecherche = CP;
            // Pour gérer les icônes fléchées
            ViewBag.CurrentSort = sortOrder;

           

            ViewBag.VilleSortParm = sortOrder == "ville" ? "ville_desc" : "ville";

            var restaurants = _context.Restaurants.AsQueryable();

            if (!string.IsNullOrEmpty(name))
                restaurants = restaurants.Where(u => EF.Functions.Like(u.name, $"%{name}%"));

            if (!string.IsNullOrEmpty(ville))
                restaurants = restaurants.Where(u => EF.Functions.Like(u.ville, $"%{ville}%"));

            if (!string.IsNullOrEmpty(CP))
                restaurants = restaurants.Where(u => EF.Functions.Like(u.cp, $"%{CP}%"));

            restaurants = sortOrder switch
            {
                "name_desc" => restaurants.OrderByDescending(r => r.name),
                "ville" => restaurants.OrderBy(r => r.ville),
                "ville_desc" => restaurants.OrderByDescending(r => r.ville),
                _ => restaurants.OrderBy(r => r.name)
            };

            var restaurantsList = await restaurants.ToListAsync();
            ViewBag.AucunResultat = (!string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(ville)|| !string.IsNullOrEmpty(CP)) && !restaurantsList.Any();

            return View(restaurantsList);
        }


        public async Task<IActionResult> Details(int? id, string nom, string prenom, string poste, DateTime? dateDebut)
        {
            if (id == null)
            {
                return NotFound();
            }
            //On ramène les données
            var restaurant = await _context.Restaurants
                .Include(r => r.RestaurantAffectations!)
                    .ThenInclude(a => a.Collaborateur)
                .Include(r => r.RestaurantAffectations!)
                    .ThenInclude(a => a.Poste)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (restaurant == null)
            {
                return NotFound();
            }

            // Filtrage côté serveur
            var affectations = restaurant.RestaurantAffectations!
                .Where(a => a.Datefin == null);

            if (!string.IsNullOrWhiteSpace(nom))
                affectations = affectations.Where(a => a.Collaborateur?.Nom != null && a.Collaborateur.Nom.Contains(nom, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(prenom))
                affectations = affectations.Where(a => a.Collaborateur?.Prénom != null && a.Collaborateur.Prénom.Contains(prenom, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(poste))
                affectations = affectations.Where(a => a.Poste?.Intituleposte != null && a.Poste.Intituleposte.Contains(poste, StringComparison.OrdinalIgnoreCase));

            if (dateDebut != null)
                affectations = affectations.Where(a => a.Datedebut.Date == dateDebut.Value.Date);

            // On injecte la liste filtrée dans le modèle
            restaurant.RestaurantAffectations = affectations.ToList();

            // Transmettre les filtres à la vue
            ViewBag.FiltreNom = nom;
            ViewBag.FiltrePrenom = prenom;
            ViewBag.FiltrePoste = poste;
            ViewBag.FiltreDateDebut = dateDebut?.ToString("yyyy-MM-dd");

            return View(restaurant);
        }

        // GET: Restaurants/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Restaurants/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,name,adresse,cp,ville")] Restaurant restaurant)
        {
            if (ModelState.IsValid)
            {
                _context.Add(restaurant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(restaurant);
        }

        // GET: Restaurants/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant == null)
            {
                return NotFound();
            }
            return View(restaurant);
        }

        // POST: Restaurants/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,name,adresse,cp,ville")] Restaurant restaurant)
        {
            if (id != restaurant.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(restaurant);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RestaurantExists(restaurant.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(restaurant);
        }

        // GET: Restaurants/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(m => m.Id == id);
            if (restaurant == null)
            {
                return NotFound();
            }

            return View(restaurant);
        }

        // POST: Restaurants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            // On vérifie que id n'est pas  null
            if (id == null)
                return NotFound();

            //var restaurant = await _context.Restaurants
              //  .FirstOrDefaultAsync(r => r.Id == id.Value);

            var restaurant = await _context.Restaurants
           .Include(c => c.RestaurantAffectations)
               .ThenInclude(a => a.Collaborateur)
           .Include(c => c.RestaurantAffectations)
               .ThenInclude(a => a.Poste)
           .FirstOrDefaultAsync(c => c.Id == id);

            if (restaurant == null)
                return NotFound();

            // On regarde si  le collaborateur a des affectations
            var aDesAffectations = await _context.Affectations.AnyAsync(a => a.RestaurantId  == id.Value);

            if (aDesAffectations)
            {
                TempData["ErreurSuppression"] = "Impossible de supprimer ce restaurant car il a des affectations.";
                return View("Delete", restaurant);
            }

            _context.Restaurants.Remove(restaurant);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool RestaurantExists(int id)
        {
            return _context.Restaurants.Any(e => e.Id == id);
        }
    }
}
