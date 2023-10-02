using Dominio.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistencia.Data.Configuration;
    public class UsuarioRolConfiguration : IEntityTypeConfiguration<UsuarioRol>
    {
        public void Configure(EntityTypeBuilder<UsuarioRol> builder)
        {
            builder.ToTable("UsuarioRol");

            builder.Property(p => p.Id)
                .HasAnnotation("MySqlValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .HasColumnName("Id_UsuarioRol")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(p => p.UsuarioId)
                .HasColumnName("Usuario_Id")
                .HasColumnType("int")
                .IsRequired();

            builder.HasOne(p => p.Usuarios)
                .WithMany(p => p.UsuarioRoles)
                .HasForeignKey(p => p.UsuarioId);

            builder.Property(p => p.RolId)
                .HasColumnName("Rol_Id")
                .HasColumnType("int")
                .IsRequired();

            builder.HasOne(p => p.Roles)
                .WithMany(p => p.UsuarioRoles)
                .HasForeignKey(p => p.RolId);

            builder.HasData(
                new {
                    Id = 1,
                    UsuarioId = 1,
                    RolId = 1
                },
                new {
                    Id = 2,
                    UsuarioId = 2,
                    RolId = 3
                }
            );

        }
    }
