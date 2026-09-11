namespace Sims.BuildingManagement.Domain.Models;

// Stan u okviru jedne zgrade
// Broj stana je jedinstven u okviru zgrade kojoj pripada

public class Apartment
{
    public string BuildingCode { get; private set; }
    public int Number { get; private set; }
    public string Description { get; private set; }
    public int RoomCount { get; private set; }
    public int MaxTenantCount { get; private set; }

    public Apartment(string buildingCode, int number, string description,
        int roomCount, int maxTenantCount)
    {
        BuildingCode = buildingCode;
        Number = number;
        Description = description;
        RoomCount = roomCount;
        MaxTenantCount = maxTenantCount;
    }

    public bool HasRoomCount(int roomCount) => RoomCount == roomCount;

    public bool HasMaxTenantCount(int tenantCount) => MaxTenantCount == tenantCount;
}
