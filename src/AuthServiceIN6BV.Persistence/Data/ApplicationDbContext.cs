using AuthServiceIN6BV.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
 
namespace AuthServiceIN6BV.Persistence.Data;
 
public class ApplicationDbContext (DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    // Definicion de las tablas en la base de datos
   public DbSet<User> Users { get; set;  }
   public DbSet<UserProfile> UserProfiles { get; set;  }
   public DbSet<Role> Roles { get; set;  }
   public DbSet<UserRole> UserRoles { get; set;  }
   public DbSet<UserEmail> UserEmails { get; set;  }
   public DbSet<UserPasswordReset> UserPasswordResets { get; set;  }

    // Configuracion del modelo de datos
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); //permite crear cada entidad en postman :D
 
    }

    // Sobrescribir los metodos de guardado para manejar timestamps
 
    public override int SaveChanges()
    {
        return base.SaveChanges();
    }
 
    // Sobrescribir los metodos asincronos de guardado para manejar timestamps
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    // Manejo de timestamps antes de guardar los cambios
 
    private void UpdateTimestamp()
    {
       
    }
 
    //formato especifico a las entidades
    private static string ToSnakeCase(string input)
    {
       
    }



    
}