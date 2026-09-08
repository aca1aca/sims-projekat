namespace Sims.BuildingManagement.Application.Search;


// DTO koji sabira sve parametre pretrage zgrada u jedan objekat

public class BuildingSearchCriteria
{
    public BuildingSearchField Field { get; set; }

    public string Text { get; set; } = string.Empty;

    public int FloorCount { get; set; }

    public ApartmentSearchField ApartmentField { get; set; }

    public int RoomCount { get; set; }

    public int TenantCapacity { get; set; }

    public SearchOperator Operator { get; set; }
}
