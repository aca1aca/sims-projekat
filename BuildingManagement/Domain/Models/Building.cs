namespace Sims.BuildingManagement.Domain.Models;

// Zgrada koju unosi administrator, a odobrava upravnik naveden preko svog JMBG-a.
// Zgrada postaje vidljiva ostalim korisnicima tek kada je upravnik odobri.
// stanje se moze promeniti samo pomocu AcceptRequest ili RejectRequest,
// ako je u stanju pendingApproval

public class Building
{
    public string Code { get; private set; }
    public string Street { get; private set; }
    public string Number { get; private set; }
    public string Neighborhood { get; private set; }
    public string City { get; private set; }
    public string Country { get; private set; }
    public int FloorCount { get; private set; }
    public string ManagerJmbg { get; private set; }
    public BuildingStatus Status { get; private set; }

    public Building(string code, string street, string number, string neighborhood,
        string city, string country, int floorCount, string managerJmbg, BuildingStatus status)
    {
        Code = code;
        Street = street;
        Number = number;
        Neighborhood = neighborhood;
        City = city;
        Country = country;
        FloorCount = floorCount;
        ManagerJmbg = managerJmbg;
        Status = status;
    }

    public string Address => $"{Street} {Number}";

    public bool IsApproved => Status == BuildingStatus.Approved;

    public bool IsPendingApproval => Status == BuildingStatus.PendingApproval;

    public bool IsManagedBy(string managerJmbg) => ManagerJmbg == managerJmbg;

    public void Approve()
    {
        RequirePendingApproval();
        Status = BuildingStatus.Approved;
    }

    public void Reject()
    {
        RequirePendingApproval();
        Status = BuildingStatus.Rejected;
    }

    private void RequirePendingApproval()
    {
        if (Status != BuildingStatus.PendingApproval)
            throw new DomainException("Zgrada je već odobrena ili odbijena.");
    }
}
