using System;
using System.Collections.Generic;
using System.Linq;
using Sims.BuildingManagement.Domain.Models;
using Sims.BuildingManagement.Domain.RepositoryInterfaces;
using Sims.BuildingManagement.Repositories.Csv;

namespace Sims.BuildingManagement.Repositories;

/// <summary>CSV implementacija skladišta korisnika.</summary>
public class CsvUserRepository : IUserRepository
{
    private static readonly string[] Header =
        { "Jmbg", "Email", "Password", "FirstName", "LastName", "MobilePhone", "Role" };

    private readonly string _filePath;

    public CsvUserRepository(string filePath)
    {
        _filePath = filePath;
    }

    public List<User> GetAll()
    {
        List<string[]> rows = CsvFile.ReadRows(_filePath);
        List<User> users = new List<User>();
        for (int i = 0; i < rows.Count; i++)
            users.Add(ToUser(rows[i]));

        return users;
    }

    public User GetByEmail(string email)
        => GetAll().FirstOrDefault(user => user.Email == email);

    public User GetByJmbg(string jmbg)
        => GetAll().FirstOrDefault(user => user.Jmbg == jmbg);

    public void Create(User user)
    {
        List<User> users = GetAll();
        users.Add(user);
        Save(users);
    }

    private void Save(List<User> users)
        => CsvFile.WriteRows(_filePath, Header, users.Select(ToRow));

    private static User ToUser(string[] row)
        => new User(row[0], row[1], row[2], row[3], row[4], row[5],
            Enum.Parse<UserRole>(row[6], ignoreCase: true));

    private static string[] ToRow(User user)
        => new[]
        {
            user.Jmbg, user.Email, user.Password, user.FirstName,
            user.LastName, user.MobilePhone, user.Role.ToString()
        };
}
