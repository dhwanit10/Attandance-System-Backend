using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Attandance_System.Data;
using Attandance_System.Models;

namespace Attandance_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : Controller
    {
        private readonly SchoolContext _context;

        public AttendanceController(SchoolContext context)
        {
            _context = context;
        }

        // 1️⃣ Get all attendance records
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Attendance>>> GetAllAttendance()
        {
            return await _context.Attendances
                                 .Include(a => a.Student)
                                 .ToListAsync();
        }

        // 2️⃣ Get attendance records by RollNo
        [HttpGet("{rollNo}")]
        public async Task<ActionResult<IEnumerable<Attendance>>> GetAttendanceByRollNo(int rollNo)
        {
            var records = await _context.Attendances
                                        .Where(a => a.RollNo == rollNo)
                                        .Include(a => a.Student)
                                        .ToListAsync();

            if (!records.Any()) return NotFound($"No attendance records found for RollNo {rollNo}");
            return records;
        }

        // 3️⃣ Add attendance for a student
        [HttpPost("{rollNo}")]
        public async Task<IActionResult> AddAttendance(int rollNo, [FromBody] AttendanceDto dto)
        {
            var student = await _context.Students.FindAsync(rollNo);
            if (student == null) return NotFound($"Student with RollNo {rollNo} not found.");

            // Check if attendance for today already exists
            var existing = await _context.Attendances
                                         .FirstOrDefaultAsync(a => a.RollNo == rollNo && a.AttendanceDate.Date == dto.AttendanceDate.Date);
            if (existing != null)
                return BadRequest("Attendance for this student on this date already exists.");

            var attendance = new Attendance
            {
                RollNo = rollNo,
                AttendanceDate = dto.AttendanceDate.Date,
                Status = dto.Status // "Present" or "Absent"
            };

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAttendanceByRollNo), new { rollNo = rollNo }, attendance);
        }

        // 4️⃣ Automatically mark absent students for today
        // Call this at the end of the day or after attendance session closes
        [HttpPost("mark-absent-today")]
        public async Task<IActionResult> MarkAbsentToday()
        {
            var today = DateTime.Today;
            var allStudents = await _context.Students.ToListAsync();

            foreach (var student in allStudents)
            {
                bool hasAttendance = await _context.Attendances
                                                   .AnyAsync(a => a.RollNo == student.RollNo && a.AttendanceDate.Date == today);

                if (!hasAttendance)
                {
                    _context.Attendances.Add(new Attendance
                    {
                        RollNo = student.RollNo,
                        AttendanceDate = today,
                        Status = "Absent"
                    });
                }
            }

            await _context.SaveChangesAsync();
            return Ok("Absent students for today have been marked successfully.");
        }
    }

    // DTO class for posting attendance
    public class AttendanceDto
    {
        public DateTime AttendanceDate { get; set; }
        public string Status { get; set; } // "Present" or "Absent"
    }

}
