using Attandance_System.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using Attandance_System.Models;

namespace Attandance_System.Data
{
    public class SchoolContext : DbContext
    {
        public SchoolContext(DbContextOptions<SchoolContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Attendance> Attendances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().HasKey(s => s.RollNo);
            modelBuilder.Entity<Attendance>().HasKey(a => a.AttendanceID);

            modelBuilder.Entity<Attendance>()
                        .HasOne(a => a.Student)
                        .WithMany(s => s.Attendances)
                        .HasForeignKey(a => a.RollNo);

            base.OnModelCreating(modelBuilder);

        }
    }
}

