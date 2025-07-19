using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Crypto;

namespace wacdoprojet.Models
{
    public class Collaborateur: IdentityUser
        {
        public required string Nom { get; set; } = "";
        public  string? Prénom {  get; set; }

        // public string email { get; set; }
        // On n'ajoute pas email car déjà dans classe IdentityUser
        public DateTime Datepremiereembauche { get; set; }
        public bool Connectable { get; set; }

        // public string? Password { get; set; } 

        // Un collaborateur peut avoir de 0 à N affectations
        public List<Affectation> Collaborateuraffectation { get; set; } =  new List<Affectation>();
        
    }
}
