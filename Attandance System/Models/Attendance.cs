using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attandance_System.Models
{
    [Table("Attendance")]
    public class Attendance
    {
        [Key]
        public int AttendanceID { get; set; }

        [ForeignKey("Student")]
        public int RollNo { get; set; }

        public DateTime AttendanceDate { get; set; }

        public string Status { get; set; } // "Present" / "Absent"

        public Student Student { get; set; }
    }
}
