namespace wacdoprojet.Models
{
    public class Affectation
    {

        public int Id { get; set; }

        // Une affectaion ne déssigne qu'un seul restaurant  dont la classe a été définie 
        public int RestaurantId { get; set; }
        public  Restaurant? Restaurant {  get; set; }

        // Une affection ne désigne  qu'un seul poste 
        public int PosteId { get; set; }
        public  Poste? Poste { get; set; }


        // Une affectation ne  désigne qu'un seul collaborateur
        public required string CollaborateurId { get; set; }
        public  Collaborateur? Collaborateur {  get; set; }

        public DateTime  Datedebut { get; set; }
        public DateTime? Datefin { get; set; }


    }
}
