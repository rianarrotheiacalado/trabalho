using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Reunioes.Core.Models;
using System;

namespace Reunioes.Core.Data
{
    public class ReunioesDbContext : DbContext
    {
        public DbSet<Sala> Salas { get; set; }
        public DbSet<Reserva> Reservas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server=localhost;Database=reunioes;Username=postgres;Password=password");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sala>().Property(s => s.Nome).IsRequired();

            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Sala)
                .WithMany(s => s.Reservas)
                .HasForeignKey(r => r.SalaId);

            modelBuilder.Entity<Reserva>()
                .Property(r => r.Inicio)
                .HasColumnType("timestamp without time zone");

            modelBuilder.Entity<Reserva>()
                .Property(r => r.Fim)
                .HasColumnType("timestamp without time zone");
        }
    }
}