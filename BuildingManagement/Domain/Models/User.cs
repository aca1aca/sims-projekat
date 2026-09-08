namespace Sims.BuildingManagement.Domain.Models;

// Struktura podataka koja modeluje korisnika sistema (Administrator, Upravnik ili Stanar).

public class User
{
    public string Jmbg { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string MobilePhone { get; private set; }
    public UserRole Role { get; private set; }

    public User(string jmbg, string email, string password, string firstName,
        string lastName, string mobilePhone, UserRole role)
    {
        Jmbg = jmbg;
        Email = email;
        Password = password;
        FirstName = firstName;
        LastName = lastName;
        MobilePhone = mobilePhone;
        Role = role;
    }

    public string FullName => $"{FirstName} {LastName}";

    public bool IsAdministrator => Role == UserRole.Administrator;

    public bool IsManager => Role == UserRole.Manager;

    public bool IsTenant => Role == UserRole.Tenant;

    public bool HasPassword(string password) => Password == password;
}
