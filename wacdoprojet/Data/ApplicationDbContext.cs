using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using wacdoprojet.Models;

namespace wacdoprojet.Data
{
    //  avant  le  1105202  public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    public class ApplicationDbContext : IdentityDbContext<Collaborateur>
    
    {
        // mise place  dece cosntructeur le 11052025
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
       : base(options)
        {
        }
        // fin mise en place constructeur le 11052025
        public DbSet<Poste> Postes { get; set; }
         public DbSet<Restaurant> Restaurants { get; set; }
         public DbSet<Collaborateur> Collaborateurs { get; set; }
         public DbSet<Affectation> Affectations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(string) && property.GetMaxLength() == null)
                    {
                        property.SetMaxLength(255);
                    }
                }
            }
        }







    }
}
