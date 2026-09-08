using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sims.BuildingManagement.Domain.Models;
using Sims.BuildingManagement.Domain.RepositoryInterfaces;
using Sims.BuildingManagement.Repositories.Csv;

namespace Sims.BuildingManagement.Repositories;

//CSV implementacija interfejsa


public class CsvBuildingRepository : IBuildingRepository
{
    private static readonly string[] Header =
    {
        "Code", "Street", "Number", "Neighborhood", "City", "Country",
        "FloorCount", "ManagerJmbg", "Status"
    };

    private readonly string _filePath;

    public CsvBuildingRepository(string filePath)
    {
        _filePath = filePath;
    }

    public List<Building> GetAll()
    {
        List<string[]> rows = CsvFile.ReadRows(_filePath);
        List<Building> buildings = new List<Building>();
        for (int i = 0; i < rows.Count; i++)
            buildings.Add(ToBuilding(rows[i]));

        return buildings;
    }

    public Building GetByCode(string code)
        => GetAll().FirstOrDefault(building => building.Code == code);

    public List<Building> GetByManager(string managerJmbg)
        => GetAll().Where(building => building.IsManagedBy(managerJmbg)).ToList();

    public void Create(Building building)
    {
        List<Building> buildings = GetAll();
        buildings.Add(building);
        Save(buildings);
    }

    public void Update(Building building)
    {
        List<Building> buildings = GetAll();
        int index = buildings.FindIndex(existing => existing.Code == building.Code);
        if (index < 0)
            throw new DomainException("Zgrada koju treba izmeniti ne postoji.");

        buildings[index] = building;
        Save(buildings);
    }

    private void Save(List<Building> buildings)
        => CsvFile.WriteRows(_filePath, Header, buildings.Select(ToRow));

    private static Building ToBuilding(string[] row)
        => new Building(
            row[0], row[1], row[2], row[3], row[4], row[5],
            int.Parse(row[6], CultureInfo.InvariantCulture),
            row[7],
            Enum.Parse<BuildingStatus>(row[8], ignoreCase: true));

    private static string[] ToRow(Building building)
        => new[]
        {
            building.Code, building.Street, building.Number, building.Neighborhood,
            building.City, building.Country,
            building.FloorCount.ToString(CultureInfo.InvariantCulture),
            building.ManagerJmbg, building.Status.ToString()
        };
}
