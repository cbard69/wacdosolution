using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace wacdoprojet.Models
{
    public class Poste

    {   public int Id { get; set; }
        public string? Intituleposte { get; set;}
        public List<Affectation> Posteaffectation { get; set; } = new List<Affectation>();

    }
}
