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
        foreach (var Entity in modelBuilder.Model.GetEntityTypes())
        {
             var tableName = Entity.GetTableName();

            // Convertir el nombre de la tabla a snake_case
            if(string.IsNullOrEmpty(tableName) == false)
            {
                Entity.SetTableName(ToSnakeCase(tableName));
            }

            // Convertir el nombre de las columnas a snake_case
            foreach (var property in Entity.GetProperties())
            {
                var columnName = property.GetColumnName();
                if(string.IsNullOrEmpty(columnName))
                {
                    property.SetColumnName(ToSnakeCase(columnName));
                }
            }

            // Convertir el nombre de las claves foraneas y primarias a snake_case
            foreach (var Key in Entity.GetKeys())
            {
                var KeyName = Key.GetName();
                if(string.IsNullOrEmpty(KeyName) == false)
                {
                    Key.SetName(ToSnakeCase(KeyName));
                }
            }

            // Convertir el nombre de los indices a snake_case
            foreach (var index in Entity.GetIndexes())
            {
                var indexName = index.GetDatabaseName();
                if(string.IsNullOrEmpty(indexName) == false)
                {
                    index.SetDatabaseName(ToSnakeCase(indexName));
                }
            }
        }

        //Configurar nuestra entidad user
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasMaxLength(16)
                .ValueGeneratedOnAdd();
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(25);
            entity.Property(e => e.SurName)
                .IsRequired()
                .HasMaxLength(25);
            entity.Property(e => e.UserName)
                .IsRequired();
            entity.Property(e => e.Email)
                .IsRequired();
            entity.Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.Status)
                .HasDefaultValue(false);
            entity.Property(e => e.CreatedAt)
                .IsRequired();
          
            // Definir indices unicos 1:1
            entity.HasIndex(e => e.UserName).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();

            // Configurar relaciones
            entity.HasOne(e => e.UserProfile)
                     .WithOne(up => up.User)
                     .HasForeignKey<UserProfile>(up => up.UserId);

            // Configurar relaciones 1:N
             entity.HasMany(e => e.UserRoles)
                    .WithOne(ur => ur.User)
                    .HasForeignKey(ur => ur.UserId);
            
            //relacion netre usuario a la entidad UserEmail 1:1
            entity.HasOne(e => e.UserEmail)
                  .WithOne(ue => ue.User)
                  .HasForeignKey<UserEmail>(ue => ue.UserId);

            //relacion entre usuario a la entidad UserPasswordReset 1:1
            entity.HasOne(e => e.UserPasswordReset)
                  .WithOne(upr => upr.User)
                  .HasForeignKey<UserPasswordReset>(upr => upr.UserId);

        });

            // configurar el userProfile para que se cree en cascada
            modelBuilder.Entity<UserProfile>(entity => 
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .HasMaxLength(16)
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.UserId)
                    .HasMaxLength(16)
                    .IsRequired();
                entity.Property(e=> e.ProfilePicture).HasDefaultValue("");
                entity.Property(e => e.Phone).HasMaxLength(8);
            });


            //configurar la entidad Role
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .HasMaxLength(16)
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.Name)
                    .IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired();
            });

            //configurar la entidad UserRole

            modelBuilder.Entity<UserRole>(entity =>
             {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .HasMaxLength(16)
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.UserId)
                    .HasMaxLength(16);
                entity.Property(e => e.RoleId)
                    .HasMaxLength(16);
                entity.Property(e => e.CreatedAt)
                    .IsRequired();
                entity.Property(e => e.UpdatedAt)
                    .IsRequired();
                entity.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId);
                entity.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId);
            });

            //configurar la entidad UserEmail

            modelBuilder.Entity<UserEmail>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .HasMaxLength(16)
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.UserId)
                    .HasMaxLength(16);
                entity.Property(e => e.EmailVerified).HasDefaultValue(false);
                entity.Property(e => e.EmailVerificationToken).HasMaxLength(256);
            });

            //configurar la entidad UserPasswordReset
            modelBuilder.Entity<UserPasswordReset>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .HasMaxLength(16)
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.UserId)
                    .HasMaxLength(16);
                entity.Property(e => e.PasswordResetToken).HasMaxLength(256);
            });
    }


    // Sobrescribir el metodo de guardado para manejar timestamps
    public override int SaveChanges()
    {
        UpdateTimestamp();
        return base.SaveChanges();
    }
 
    // Sobrescribir los metodos asincronos de guardado para manejar timestamps
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    // Sobrescribir los metodos asincronos de guardado para manejar timestamps
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamp();
       return base.SaveChangesAsync(cancellationToken);
    }
    
    // Actualizar los timestamps antes de guardar los cambios
    private void UpdateTimestamp()
    {
        // Obtener las entradas que han sido agregadas o modificadas
        var entries = ChangeTracker.Entries()
              .Where(e => e.Entity is User || e.Entity is Role || e.Entity is UserRole
                        && (e.State == EntityState.Added || e.State == EntityState.Modified));

        // Actualizar los timestamps segun el estado de la entidad
        foreach (var entry in entries)
        {
            // Verificar el tipo de entidad y actualizar los campos CreatedAt y UpdatedAt
            if(entry.Entity is User user)
            {
                if(entry.State == EntityState.Added)
                {
                    user.CreatedAt = DateTime.UtcNow;
                }
                user.UpdatedAt = DateTime.UtcNow;
            }
            else if(entry.Entity is Role role)
            {
                if(entry.State == EntityState.Added)
                {
                    role.CreatedAt = DateTime.UtcNow;
                }
                role.UpdatedAt = DateTime.UtcNow;
            }
            else if(entry.Entity is UserRole userRole)
            {
                if(entry.State == EntityState.Added)
                {
                    userRole.CreatedAt = DateTime.UtcNow;
                }
                userRole.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
 
    //formato especifico a las entidades
    private static string ToSnakeCase(string input)
    {
        if(string.IsNullOrEmpty(input))
        {
            return input;
        }

        return string.Concat(
            input.Select((c, i) => i > 0 && char.IsUpper(c) ? "_" + c : c.ToString())).ToLower();
       
    }
    
}