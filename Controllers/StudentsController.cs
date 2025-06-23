using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentApi.Data;
using StudentApi.Models;

namespace StudentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public StudentsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        => await _context.Students.ToListAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<Student>> GetStudent(int id)
    {
        var student = await _context.Students.FindAsync(id);
        return student is null ? NotFound() : student;
    }

    [HttpPost]
    public async Task<ActionResult<Student>> CreateStudent(Student student)
    {
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStudent(int id, Student updated)
    {
        if (id != updated.Id) return BadRequest();
        _context.Entry(updated).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStudent(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student is null) return NotFound();
        _context.Students.Remove(student);
        await _context.SaveChangesAsync();
        return NoContent();
    }
[HttpGet("filter")]
public async Task<ActionResult<IEnumerable<Student>>> FilterStudents(
    string? gender, string? course, DateTime? dobStart, DateTime? dobEnd)
{
    var query = _context.Students.AsQueryable();

    if (!string.IsNullOrEmpty(gender))
        query = query.Where(s => s.Gender == gender);

    if (!string.IsNullOrEmpty(course))
        query = query.Where(s => s.Courses.Contains(course));

    if (dobStart.HasValue)
        query = query.Where(s => s.DateOfBirth >= dobStart);

    if (dobEnd.HasValue)
        query = query.Where(s => s.DateOfBirth <= dobEnd);

    return await query.ToListAsync();
}

}
