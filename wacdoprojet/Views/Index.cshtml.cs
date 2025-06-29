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
    public class IndexModel : PageModel
    {
        private readonly wacdoprojet.Data.ApplicationDbContext _context;

        public IndexModel(wacdoprojet.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Poste> Poste { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Poste = await _context.Postes.ToListAsync();
        }
    }
}
