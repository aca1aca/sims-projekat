using System.Collections.Generic;
using System.Linq;
using Sims.BuildingManagement.Domain.Models;
using Sims.BuildingManagement.Domain.RepositoryInterfaces;

namespace Sims.BuildingManagement.Application.UseCases;

//radnje nad zahtevima koje obuhvataju vise entiteta
//poslovna logika

public class AccessRequestService
{
    private readonly IAccessRequestRepository _requestRepository;
    private readonly IBuildingRepository _buildingRepository;
    private readonly IApartmentRepository _apartmentRepository;

    public AccessRequestService(IAccessRequestRepository requestRepository,
        IBuildingRepository buildingRepository, IApartmentRepository apartmentRepository)
    {
        _requestRepository = requestRepository;
        _buildingRepository = buildingRepository;
        _apartmentRepository = apartmentRepository;
    }

    public bool IsApartmentOccupied(string buildingCode, int apartmentNumber)
    {
        List<AccessRequest> buildingRequests = _requestRepository.GetByBuilding(buildingCode);
        return buildingRequests.Any(request =>
            request.Targets(buildingCode, apartmentNumber) && request.IsApproved);
    }

    public AccessRequest SubmitRequest(string tenantJmbg, string buildingCode, int apartmentNumber)
    {
        Building building = _buildingRepository.GetByCode(buildingCode);
        if (building == null || !building.IsApproved)
            throw new DomainException("Izabrana zgrada nije dostupna.");
        if (_apartmentRepository.Get(buildingCode, apartmentNumber) == null)
            throw new DomainException("Uneti broj stana ne postoji u izabranoj zgradi.");

        AccessRequest request = AccessRequest.CreateNew(tenantJmbg, buildingCode, apartmentNumber);
        _requestRepository.Create(request);
        return request;
    }

    public List<AccessRequest> GetTenantRequests(string tenantJmbg, RequestStatus? statusFilter)
    {
        List<AccessRequest> requests = _requestRepository.GetByTenant(tenantJmbg);
        return FilterByStatus(requests, statusFilter);
    }

    public List<AccessRequest> GetBuildingRequestsForManager(string buildingCode, RequestStatus? statusFilter)
    {
        List<AccessRequest> requests = _requestRepository.GetByBuilding(buildingCode);
        List<AccessRequest> pendingOrApproved = requests.Where(request => !request.IsRejected).ToList();
        return FilterByStatus(pendingOrApproved, statusFilter);
    }

    public void WithdrawRequest(string requestId, string tenantJmbg)
    {
        AccessRequest request = RequireOwnRequest(requestId, tenantJmbg);
        if (!request.IsPending)
            throw new DomainException("Moguće je povući samo zahteve koji čekaju odobrenje.");

        _requestRepository.Delete(requestId);
    }

    public void ApproveRequest(string requestId, string managerJmbg)
    {
        AccessRequest request = RequireRequestForManager(requestId, managerJmbg);
        request.Approve();
        _requestRepository.Update(request);
    }

    public void RejectRequest(string requestId, string managerJmbg, string reason)
    {
        AccessRequest request = RequireRequestForManager(requestId, managerJmbg);
        request.Reject(reason);
        _requestRepository.Update(request);
    }

    private static List<AccessRequest> FilterByStatus(List<AccessRequest> requests, RequestStatus? statusFilter)
    {
        if (statusFilter == null)
            return requests;

        return requests.Where(request => request.Status == statusFilter.Value).ToList();
    }

    private AccessRequest RequireOwnRequest(string requestId, string tenantJmbg)
    {
        AccessRequest request = _requestRepository.GetById(requestId);
        if (request == null || !request.SubmittedBy(tenantJmbg))
            throw new DomainException("Zahtev nije pronađen.");

        return request;
    }

    private AccessRequest RequireRequestForManager(string requestId, string managerJmbg)
    {
        AccessRequest request = _requestRepository.GetById(requestId);
        if (request == null)
            throw new DomainException("Zahtev nije pronađen.");

        Building building = _buildingRepository.GetByCode(request.BuildingCode);
        if (building == null || !building.IsManagedBy(managerJmbg))
            throw new DomainException("Zahtev ne pripada zgradama prijavljenog upravnika.");

        return request;
    }
}
