//using Microsoft.EntityFrameworkCore;
using Attandance_System.Data;
using Attandance_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Attandance_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : Controller
    {
        private readonly SchoolContext _context;
        public StudentsController(SchoolContext context)
        {
            _context = context;
        }
        // 1️⃣ Get all students
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetAllStudents()
        {
            return await _context.Students.ToListAsync();
        }

        // 2️⃣ Get student by RollNo
        [HttpGet("{rollNo}")]
        public async Task<ActionResult<Student>> GetStudent(int rollNo)
        {
            var student = await _context.Students.FindAsync(rollNo);
            if (student == null) return NotFound($"Student with RollNo {rollNo} not found.");
            return student;
        }

        // 3️⃣ Add a new student
        [HttpPost]
        public async Task<ActionResult<Student>> AddStudent([FromBody] Student student)
        {
            var exists = await _context.Students.AnyAsync(s => s.RollNo == student.RollNo);
            if (exists) return BadRequest($"Student with RollNo {student.RollNo} already exists.");

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStudent), new { rollNo = student.RollNo }, student);
        }

        // 4️⃣ Update existing student
        [HttpPut("{rollNo}")]
        public async Task<IActionResult> UpdateStudent(int rollNo, [FromBody] Student updatedStudent)
        {
            if (rollNo != updatedStudent.RollNo)
                return BadRequest("RollNo in URL and body must match.");

            var student = await _context.Students.FindAsync(rollNo);
            if (student == null) return NotFound($"Student with RollNo {rollNo} not found.");

            student.Name = updatedStudent.Name;
            student.Sem = updatedStudent.Sem;
            student.Class = updatedStudent.Class;
            student.Dob = updatedStudent.Dob;

            _context.Students.Update(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 5️⃣ Delete a student
        [HttpDelete("{rollNo}")]
        public async Task<IActionResult> DeleteStudent(int rollNo)
        {
            var student = await _context.Students.FindAsync(rollNo);
            if (student == null) return NotFound($"Student with RollNo {rollNo} not found.");

            // Optional: delete attendance records of this student
            var attendances = _context.Attendances.Where(a => a.RollNo == rollNo);
            _context.Attendances.RemoveRange(attendances);

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
