using System.Collections.Generic;
using Sims.BuildingManagement.Domain.Models;

namespace Sims.BuildingManagement.Domain.RepositoryInterfaces;

// funkcije koje treba da se implementiaju kad se pozove interfejs

public interface IAccessRequestRepository
{
    List<AccessRequest> GetAll();

    AccessRequest GetById(string id);

    List<AccessRequest> GetByTenant(string tenantJmbg);

    List<AccessRequest> GetByBuilding(string buildingCode);

    void Create(AccessRequest request);

    void Update(AccessRequest request);

    void Delete(string id);
}
