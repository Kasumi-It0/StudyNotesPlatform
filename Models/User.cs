namespace StudyNotesPlatform.Models;

// Временное хранилище (вместо базы данных)
public static class UserStorage
{
    public static List<User> Users = new List<User>();
}

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string University { get; set; } = string.Empty;
}