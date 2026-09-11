using System;
using System.Collections.Generic;
using Sims.BuildingManagement.Domain.Models;

namespace Sims.BuildingManagement.Application.Search;

// Funkcionalna klasa koja odlučuje da li jedna 
// zgrada odgovara zadatoj pretrazi
// Izdvojena iz servisa

public static class BuildingSearchFilter
{
    public static bool Matches(Building building, IReadOnlyList<Apartment> apartments,
        BuildingSearchCriteria criteria)
    {
        return criteria.Field switch
        {
            BuildingSearchField.Address => ContainsIgnoreCase(building.Address, criteria.Text),
            BuildingSearchField.Neighborhood => ContainsIgnoreCase(building.Neighborhood, criteria.Text),
            BuildingSearchField.FloorCount => building.FloorCount == criteria.FloorCount,
            BuildingSearchField.Apartments => AnyApartmentMatches(apartments, criteria),
            _ => false //vraca true ako zgrada zadovoljava pretragu 
            //inace false
        };
    }

    private static bool ContainsIgnoreCase(string value, string term)
        => value.Contains(term ?? string.Empty, StringComparison.OrdinalIgnoreCase);

    private static bool AnyApartmentMatches(IReadOnlyList<Apartment> apartments,
        BuildingSearchCriteria criteria)
    {
        for (int i = 0; i < apartments.Count; i++)
        {
            if (ApartmentMatches(apartments[i], criteria))
                return true;
        }
        return false;
    }

    private static bool ApartmentMatches(Apartment apartment, BuildingSearchCriteria criteria)
    {
        return criteria.ApartmentField switch
        {
            ApartmentSearchField.RoomCount => apartment.HasRoomCount(criteria.RoomCount),
            ApartmentSearchField.TenantCapacity => apartment.HasMaxTenantCount(criteria.TenantCapacity),
            ApartmentSearchField.RoomCountAndTenantCapacity => CombinedMatch(apartment, criteria),
            _ => false
        };
    }

    private static bool CombinedMatch(Apartment apartment, BuildingSearchCriteria criteria)
    {
        bool matchesRooms = apartment.HasRoomCount(criteria.RoomCount);
        bool matchesCapacity = apartment.HasMaxTenantCount(criteria.TenantCapacity);

        return criteria.Operator == SearchOperator.And
            ? matchesRooms && matchesCapacity
            : matchesRooms || matchesCapacity;
    } //da li jedan stan zadovoljava pretragu po 2 uslova istovremeno
}
