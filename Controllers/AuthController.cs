using Microsoft.AspNetCore.Mvc;
using Npgsql;
using StudyNotesPlatform.Models;
using System;
using System.Threading.Tasks;

namespace StudyNotesPlatform.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    // Обязательно проверь правильность пароля и имени БД из pgAdmin
    private readonly string _connectionString = "Host=127;Username=postgres;Password=123;Database=Конспекты";

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
    {
        try
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            string sql = "INSERT INTO users (role_id, university_id, full_name, email, password_hash) VALUES (@r, @u, @n, @e, @p) RETURNING id;";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("r", 1); // По умолчанию роль студента [cite: 1]
            cmd.Parameters.AddWithValue("u", request.UniversityId); // Внешний ключ на вуз [cite: 2]
            cmd.Parameters.AddWithValue("n", request.FullName);
            cmd.Parameters.AddWithValue("e", request.Email);
            cmd.Parameters.AddWithValue("p", request.Password);

            var result = await cmd.ExecuteScalarAsync();
            return Ok(new { Id = result, Message = "Успешная регистрация в PostgreSQL" });
        }
        catch (Exception ex)
        {
            return BadRequest($"Ошибка БД: {ex.Message}");
        }
    }
}

public class UserRegistrationRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int UniversityId { get; set; }
}