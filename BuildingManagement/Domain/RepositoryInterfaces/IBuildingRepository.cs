using System.Collections.Generic;
using Sims.BuildingManagement.Domain.Models;

namespace Sims.BuildingManagement.Domain.RepositoryInterfaces;

// funkcije koje treba da se implementiaju kad se pozove interfejs

public interface IBuildingRepository
{
    List<Building> GetAll();

    Building GetByCode(string code);

    List<Building> GetByManager(string managerJmbg);

    void Create(Building building);

    void Update(Building building);
}
