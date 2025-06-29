using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using wacdoprojet.Data;
using wacdoprojet.Models;

namespace wacdoprojet.Views
{
    public class CreateModel : PageModel
    {
        private readonly wacdoprojet.Data.ApplicationDbContext _context;

        public CreateModel(wacdoprojet.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Poste Poste { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Postes.Add(Poste);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
