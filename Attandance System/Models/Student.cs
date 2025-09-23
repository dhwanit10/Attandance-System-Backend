using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attandance_System.Models
{
    [Table("student")]
    public class Student
    {
        [Key]
        public int RollNo { get; set; }

        public string Name { get; set; }
        public int Sem { get; set; }
        public string Class { get; set; }
        public DateTime Dob { get; set; }

        public ICollection<Attendance> Attendances { get; set; }
    }
}
