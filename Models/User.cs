using System;

namespace StudyNotesPlatform.Models;

public class User
{
    public int Id { get; set; } // Соответствует PRIMARY KEY 
    public int RoleId { get; set; } // Соответствует role_id 
    public int UniversityId { get; set; } // Соответствует university_id 
    public string FullName { get; set; } = string.Empty; // Соответствует full_name 
    public string Email { get; set; } = string.Empty; // Соответствует email 
    public string PasswordHash { get; set; } = string.Empty; // Соответствует password_hash 
    public DateTime CreatedAt { get; set; } // Соответствует created_at 
}