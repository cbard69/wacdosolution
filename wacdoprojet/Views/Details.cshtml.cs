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
    public class DetailsModel : PageModel
    {
        private readonly wacdoprojet.Data.ApplicationDbContext _context;

        public DetailsModel(wacdoprojet.Data.ApplicationDbContext context)
        {
            _context = context;
        }

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
    }
}
