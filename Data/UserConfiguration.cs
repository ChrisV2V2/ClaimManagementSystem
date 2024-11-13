using LecturerHourlyClaimApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;


namespace LecturerHourlyClaimApp.Data
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<User> builder)
        {
            builder.HasKey(b=>b.Id);
            builder.HasData(
              new User { Id = 1, Username = "lecturer1", Password = "password1", Role = "Lecturer" },
              new User { Id = 2, Username = "admin1", Password = "password2", Role = "Admin" },
              new User { Id = 3, Username = "manager", Password = "manager1", Role = "AcademicManager" }
          );
        }
    }
}
