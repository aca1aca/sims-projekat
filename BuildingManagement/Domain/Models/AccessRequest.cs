using System;

namespace Sims.BuildingManagement.Domain.Models;

// Zahtev stanara za pristup jednom stanu u jednoj zgradi
// Sadrzi stanara koji ga podnosi, ciljnu zgradu i stan, datum kreiranja i status

public class AccessRequest
{
    public string Id { get; private set; }
    public string TenantJmbg { get; private set; }
    public string BuildingCode { get; private set; }
    public int ApartmentNumber { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public RequestStatus Status { get; private set; }
    public string RejectionReason { get; private set; }

    public AccessRequest(string id, string tenantJmbg, string buildingCode, int apartmentNumber,
        DateTime createdAt, RequestStatus status, string rejectionReason)
    {
        Id = id;
        TenantJmbg = tenantJmbg;
        BuildingCode = buildingCode;
        ApartmentNumber = apartmentNumber;
        CreatedAt = createdAt;
        Status = status;
        RejectionReason = rejectionReason;
    }

    public static AccessRequest CreateNew(string tenantJmbg, string buildingCode, int apartmentNumber)
    {
        return new AccessRequest(Guid.NewGuid().ToString(), tenantJmbg, buildingCode, apartmentNumber,
            DateTime.Now, RequestStatus.PendingApproval, string.Empty);
    }

    public bool IsPending => Status == RequestStatus.PendingApproval;

    public bool IsApproved => Status == RequestStatus.Approved;

    public bool IsRejected => Status == RequestStatus.Rejected;

    public bool SubmittedBy(string tenantJmbg) => TenantJmbg == tenantJmbg;

    public bool Targets(string buildingCode, int apartmentNumber)
        => BuildingCode == buildingCode && ApartmentNumber == apartmentNumber;

    public void Approve()
    {
        RequirePending();
        Status = RequestStatus.Approved;
    }

    public void Reject(string reason)
    {
        RequirePending();
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Razlog odbijanja je obavezan.");
        Status = RequestStatus.Rejected;
        RejectionReason = reason;
    }

    private void RequirePending()
    {
        if (!IsPending)
            throw new DomainException("Zahtev je već obrađen.");
    }
}
