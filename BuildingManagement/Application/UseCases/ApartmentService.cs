using System.Collections.Generic;
using Sims.BuildingManagement.Domain.Models;
using Sims.BuildingManagement.Domain.RepositoryInterfaces;

namespace Sims.BuildingManagement.Application.UseCases;


// Upravnik unosi stanove isključivo u sopstvene zgrade.
//poslovna logika


public class ApartmentService
{
    private readonly IApartmentRepository _apartmentRepository;
    private readonly IBuildingRepository _buildingRepository;

    public ApartmentService(IApartmentRepository apartmentRepository, IBuildingRepository buildingRepository)
    {
        _apartmentRepository = apartmentRepository;
        _buildingRepository = buildingRepository;
    }

    public List<Apartment> GetApartments(string buildingCode)
        => _apartmentRepository.GetByBuilding(buildingCode);

    public void AddApartment(Apartment apartment, string managerJmbg)
    {
        Building building = _buildingRepository.GetByCode(apartment.BuildingCode);
        if (building == null)
            throw new DomainException("Zgrada za koju se unosi stan ne postoji.");
        if (!building.IsManagedBy(managerJmbg))
            throw new DomainException("Stanove je moguće uneti samo u sopstvene zgrade.");
        if (_apartmentRepository.Get(apartment.BuildingCode, apartment.Number) != null)
            throw new DomainException("Stan sa datim brojem već postoji u zgradi.");

        _apartmentRepository.Create(apartment);
    }
}
