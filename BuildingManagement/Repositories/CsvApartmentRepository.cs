using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sims.BuildingManagement.Domain.Models;
using Sims.BuildingManagement.Domain.RepositoryInterfaces;
using Sims.BuildingManagement.Repositories.Csv;

namespace Sims.BuildingManagement.Repositories;

//CSV implementacija interfejsa

public class CsvApartmentRepository : IApartmentRepository
{
    private static readonly string[] Header =
        { "BuildingCode", "Number", "Description", "RoomCount", "MaxTenantCount" };

    private readonly string _filePath;

    public CsvApartmentRepository(string filePath)
    {
        _filePath = filePath;
    }

    public List<Apartment> GetByBuilding(string buildingCode)
        => GetAll().Where(apartment => apartment.BuildingCode == buildingCode).ToList();

    public Apartment Get(string buildingCode, int number)
        => GetAll().FirstOrDefault(apartment =>
            apartment.BuildingCode == buildingCode && apartment.Number == number);

    public void Create(Apartment apartment)
    {
        List<Apartment> apartments = GetAll();
        apartments.Add(apartment);
        CsvFile.WriteRows(_filePath, Header, apartments.Select(ToRow));
    }

    private List<Apartment> GetAll()
    {
        List<string[]> rows = CsvFile.ReadRows(_filePath);
        List<Apartment> apartments = new List<Apartment>();
        for (int i = 0; i < rows.Count; i++)
            apartments.Add(ToApartment(rows[i]));

        return apartments;
    }

    private static Apartment ToApartment(string[] row)
        => new Apartment(
            row[0],
            int.Parse(row[1], CultureInfo.InvariantCulture),
            row[2],
            int.Parse(row[3], CultureInfo.InvariantCulture),
            int.Parse(row[4], CultureInfo.InvariantCulture));
        //"BuildingCode", "Number", "Description", "RoomCount", "MaxTenantCount"
    
    private static string[] ToRow(Apartment apartment)
        => new[]
        {
            apartment.BuildingCode,
            apartment.Number.ToString(CultureInfo.InvariantCulture),
            apartment.Description,
            apartment.RoomCount.ToString(CultureInfo.InvariantCulture),
            apartment.MaxTenantCount.ToString(CultureInfo.InvariantCulture)
        };
}
