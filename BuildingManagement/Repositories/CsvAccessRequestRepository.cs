using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sims.BuildingManagement.Domain.Models;
using Sims.BuildingManagement.Domain.RepositoryInterfaces;
using Sims.BuildingManagement.Repositories.Csv;

namespace Sims.BuildingManagement.Repositories;

//CSV implementacija interfejsa

public class CsvAccessRequestRepository : IAccessRequestRepository
{
    private const string DateFormat = "yyyy-MM-ddTHH:mm:ss";

    private static readonly string[] Header =
    {
        "Id", "TenantJmbg", "BuildingCode", "ApartmentNumber",
        "CreatedAt", "Status", "RejectionReason"
    };

    private readonly string _filePath;

    public CsvAccessRequestRepository(string filePath)
    {
        _filePath = filePath;
    }

    public List<AccessRequest> GetAll()
    {
        List<string[]> rows = CsvFile.ReadRows(_filePath);
        List<AccessRequest> requests = new List<AccessRequest>();
        for (int i = 0; i < rows.Count; i++)
            requests.Add(ToRequest(rows[i]));

        return requests;
    }

    public AccessRequest GetById(string id)
        => GetAll().FirstOrDefault(request => request.Id == id);

    public List<AccessRequest> GetByTenant(string tenantJmbg)
        => GetAll().Where(request => request.SubmittedBy(tenantJmbg)).ToList();

    public List<AccessRequest> GetByBuilding(string buildingCode)
        => GetAll().Where(request => request.BuildingCode == buildingCode).ToList();

    public void Create(AccessRequest request)
    {
        List<AccessRequest> requests = GetAll();
        requests.Add(request);
        Save(requests);
    }

    public void Update(AccessRequest request)
    {
        List<AccessRequest> requests = GetAll();
        int index = requests.FindIndex(existing => existing.Id == request.Id);
        if (index < 0)
            throw new DomainException("Zahtev koji treba izmeniti ne postoji.");

        requests[index] = request;
        Save(requests);
    }

    public void Delete(string id)
    {
        List<AccessRequest> requests = GetAll();
        requests.RemoveAll(request => request.Id == id);
        Save(requests);
    }

    private void Save(List<AccessRequest> requests)
        => CsvFile.WriteRows(_filePath, Header, requests.Select(ToRow));

    private static AccessRequest ToRequest(string[] row)
        => new AccessRequest(
            row[0],
            row[1],
            row[2],
            int.Parse(row[3], CultureInfo.InvariantCulture),
            DateTime.ParseExact(row[4], DateFormat, CultureInfo.InvariantCulture),
            Enum.Parse<RequestStatus>(row[5], ignoreCase: true),
            row[6]); //rows od 0 do 6 je : "Id", "TenantJmbg", "BuildingCode", "ApartmentNumber","CreatedAt", "Status", "RejectionReason"

    private static string[] ToRow(AccessRequest request)
        => new[]
        {
            request.Id,
            request.TenantJmbg,
            request.BuildingCode,
            request.ApartmentNumber.ToString(CultureInfo.InvariantCulture),
            request.CreatedAt.ToString(DateFormat, CultureInfo.InvariantCulture),
            request.Status.ToString(),
            request.RejectionReason
        };
}
