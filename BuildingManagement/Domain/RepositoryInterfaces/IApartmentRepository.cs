using System.Collections.Generic;
using Sims.BuildingManagement.Domain.Models;

namespace Sims.BuildingManagement.Domain.RepositoryInterfaces;

// funkcije koje treba da se implementiaju kad se pozove interfejs

public interface IApartmentRepository
{
    List<Apartment> GetByBuilding(string buildingCode);

    Apartment Get(string buildingCode, int number);

    void Create(Apartment apartment);
}
