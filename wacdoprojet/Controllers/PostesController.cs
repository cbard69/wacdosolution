using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using wacdoprojet.Data;
using wacdoprojet.Models;

namespace wacdoprojet.Controllers
{
    public class PostesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PostesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Postes
        public async Task<IActionResult> Index(string intitulePoste, string sortOrder)
        {   //  Pour conserver la valeur saisie dans le formulaire
            ViewBag.IntitulePosteRecherche = intitulePoste;
           
            // Pour gérer les icônes fléchées
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NomSortParm = string.IsNullOrEmpty(sortOrder) || sortOrder == "Intituleposte" ? "Intituleposte_desc" : "Intituleposte";

            var postes = _context.Postes.AsQueryable();

            if (!string.IsNullOrEmpty(intitulePoste))
                postes = postes.Where(u => EF.Functions.Like(u.Intituleposte, $"%{intitulePoste}%"));



            postes = sortOrder switch
            {
                "Intituleposte_desc" => postes.OrderByDescending(r => r.Intituleposte),
                                   _ => postes.OrderBy(r => r.Intituleposte),
            };


            var postesList = await postes.ToListAsync();
            ViewBag.AucunResultat = (!string.IsNullOrEmpty(intitulePoste)) && !postesList.Any();

            return View(postesList);

        


            

        }

        // GET: Postes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var poste = await _context.Postes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (poste == null)
            {
                return NotFound();
            }

            return View(poste);
        }

        // GET: Postes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Postes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Intituleposte")] Poste poste)
        {
            if (ModelState.IsValid)
            {
                _context.Add(poste);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(poste);
        }

        // GET: Postes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var poste = await _context.Postes.FindAsync(id);
            if (poste == null)
            {
                return NotFound();
            }
            return View(poste);
        }

        // POST: Postes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Intituleposte")] Poste poste)
        {
            if (id != poste.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(poste);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PosteExists(poste.Id))
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
            return View(poste);
        }

        // GET: Postes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var poste = await _context.Postes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (poste == null)
            {
                return NotFound();
            }

            return View(poste);
        }

        // POST: Postes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var poste = await _context.Postes.FindAsync(id);
            if (poste != null)
            {
                _context.Postes.Remove(poste);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PosteExists(int id)
        {
            return _context.Postes.Any(e => e.Id == id);
        }
    }
}
