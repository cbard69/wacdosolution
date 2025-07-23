using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace wacdoprojet.Models
{
    public class Poste

    {   public int Id { get; set; }

        [Display(Name = "Intitulé du poste")]
        public string? Intituleposte { get; set;}
        public List<Affectation> Posteaffectation { get; set; } = new List<Affectation>();

    }
}
