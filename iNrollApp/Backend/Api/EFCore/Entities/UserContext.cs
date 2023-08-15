using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Api.EFCore.Entities
{
    public class UserContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-A56FHCK\\SQLEXPRESS;Database=STUDENROLL;Trusted_Connection=True;TrustServerCertificate=True");
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime Birthdate { get; set; }
        public string Photo { get; set; }
        public string Password { get; set; }
        public string LevelOfStudy { get; set; }
        public string Program { get; set; }
        public string FacultyDivision { get; set; }
    }
}