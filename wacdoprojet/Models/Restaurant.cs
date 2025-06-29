using System.Security.Policy;

namespace wacdoprojet.Models
{
    public class Restaurant
    {
        public int Id { get; set; }
        public string? name { get; set; }
        public string? adresse { get; set; }
        public string? cp { get; set; }
        public string? ville { get; set; }

        // A un restaurant correspond 0 ou N affectations
        public List<Affectation>? RestaurantAffectations { get; set; }

    }
}
