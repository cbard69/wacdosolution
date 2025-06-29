using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using wacdoprojet.Data;
using wacdoprojet.Models;

namespace wacdoprojet.Views
{
    public class DeleteModel : PageModel
    {
        private readonly wacdoprojet.Data.ApplicationDbContext _context;

        public DeleteModel(wacdoprojet.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Poste Poste { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var poste = await _context.Postes.FirstOrDefaultAsync(m => m.Id == id);

            if (poste is not null)
            {
                Poste = poste;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var poste = await _context.Postes.FindAsync(id);
            if (poste != null)
            {
                Poste = poste;
                _context.Postes.Remove(Poste);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
