﻿using LecturerHourlyClaimApp.Models;
using Microsoft.EntityFrameworkCore;


namespace LecturerHourlyClaimApp.Data
{
    public class LecturerDbContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<User> Users { get; set; }

       

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName:"Claims");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            

            // Define primary keys
            modelBuilder.Entity<Person>().HasKey(p => p.Id);
            modelBuilder.Entity<Claim>().HasKey(c => c.Id);
            modelBuilder.ApplyConfiguration(new UserConfiguration());

            // Configure relationships
            modelBuilder.Entity<Claim>()
                .HasOne(c => c.Person)
                .WithMany(p => p.Claims)
                .HasForeignKey(c => c.PersonId);

           

            // Seed data for Persons
            modelBuilder.Entity<Person>().HasData(
                new Person { Id = 1, FirstName = "John", LastName = "Doe" },
                new Person { Id = 2, FirstName = "Jane", LastName = "Smith" }
            );

            // Seed data for Claims (if needed)
            modelBuilder.Entity<Claim>().HasData(
                new Claim { Id = 1, StartDate = DateTime.Now.AddDays(-7), EndDate = DateTime.Now.AddDays(-1), HoursWorked = 10, HourlyRate = 50, PersonId = 1, Notes = "Sample claim for John Doe" },
                new Claim { Id = 2, StartDate = DateTime.Now.AddDays(-5), EndDate = DateTime.Now.AddDays(-2), HoursWorked = 8, HourlyRate = 50, PersonId = 2, Notes = "Sample claim for Jane Smith" }
            );
            base.OnModelCreating(modelBuilder);
        }
    }
}
