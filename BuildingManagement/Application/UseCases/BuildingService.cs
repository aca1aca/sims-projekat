using System.Collections.Generic;
using System.Linq;
using Sims.BuildingManagement.Application.Search;
using Sims.BuildingManagement.Domain.Models;
using Sims.BuildingManagement.Domain.RepositoryInterfaces;

namespace Sims.BuildingManagement.Application.UseCases;


// Poslovna logike nad zgradama: 
// unos od strane administratora, odobravanje/odbijanje
// od strane upravnika, prikaz odobrenih zgrada i pretraga.

public class BuildingService
{
    private readonly IBuildingRepository _buildingRepository;
    private readonly IApartmentRepository _apartmentRepository;
    private readonly IUserRepository _userRepository;

    public BuildingService(IBuildingRepository buildingRepository,
        IApartmentRepository apartmentRepository, IUserRepository userRepository)
    {
        _buildingRepository = buildingRepository;
        _apartmentRepository = apartmentRepository;
        _userRepository = userRepository;
    }

    public void RegisterBuilding(Building building)
    {
        if (_buildingRepository.GetByCode(building.Code) != null)
            throw new DomainException("Zgrada sa datom šifrom već postoji.");

        User assignedManager = _userRepository.GetByJmbg(building.ManagerJmbg);
        if (assignedManager == null || !assignedManager.IsManager)
            throw new DomainException("Ne postoji upravnik sa unetim JMBG-om.");

        _buildingRepository.Create(building);
    }

    public List<Building> GetApprovedBuildings()
        => _buildingRepository.GetAll().Where(building => building.IsApproved).ToList();

    public List<Building> GetApprovedBuildingsSortedByFloorCount()
        => GetApprovedBuildings().OrderBy(building => building.FloorCount).ToList();

    public List<Building> GetBuildingsManagedBy(string managerJmbg)
        => _buildingRepository.GetByManager(managerJmbg);

    public List<Building> Search(BuildingSearchCriteria criteria)
    {
        List<Building> approvedBuildings = GetApprovedBuildings();
        List<Building> matches = new List<Building>();

        for (int i = 0; i < approvedBuildings.Count; i++)
        {
            Building building = approvedBuildings[i];
            List<Apartment> apartments = _apartmentRepository.GetByBuilding(building.Code);
            if (BuildingSearchFilter.Matches(building, apartments, criteria))
                matches.Add(building);
        }

        return matches;
    }

    public void ApproveBuilding(string buildingCode, string managerJmbg)
    {
        Building building = RequireManagedBuilding(buildingCode, managerJmbg);
        building.Approve();
        _buildingRepository.Update(building);
    }

    public void RejectBuilding(string buildingCode, string managerJmbg)
    {
        Building building = RequireManagedBuilding(buildingCode, managerJmbg);
        building.Reject();
        _buildingRepository.Update(building);
    }

    private Building RequireManagedBuilding(string buildingCode, string managerJmbg)
    {
        Building building = _buildingRepository.GetByCode(buildingCode);
        if (building == null)
            throw new DomainException("Tražena zgrada ne postoji.");
        if (!building.IsManagedBy(managerJmbg))
            throw new DomainException("Zgrada ne pripada prijavljenom upravniku.");

        return building;
    }
}
