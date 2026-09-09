using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Sims.BuildingManagement.Application.UseCases;
using Sims.BuildingManagement.Domain.Models;

namespace Sims.BuildingManagement.WPF.ViewModels;


// Prikaz zahteva (na cekanju i potvrdjenih) za izabranu zgradu 
// prijavljenog upravnika uz
// filtriranje po statusu. Upravnik potvrdjuje zahteve ili ih 
// odbija sa obrazlozenjem.

public class ManagerRequestsViewModel : ViewModelBase
{
    private readonly AccessRequestService _accessRequestService;
    private readonly User _manager;

    private Building _selectedBuilding;
    private RequestStatus? _statusFilter;
    private AccessRequest _selectedRequest;
    private string _rejectionReason = string.Empty;
    private string _message = string.Empty;

    public ManagerRequestsViewModel(BuildingService buildingService,
        AccessRequestService accessRequestService, User manager)
    {
        _accessRequestService = accessRequestService;
        _manager = manager;

        Buildings = new ObservableCollection<Building>(buildingService.GetBuildingsManagedBy(manager.Jmbg));
        Requests = new ObservableCollection<AccessRequest>();
        ApproveCommand = new RelayCommand(Approve, CanApprove);
        RejectCommand = new RelayCommand(Reject, CanReject);
    }

    public IReadOnlyList<EnumOption<RequestStatus?>> StatusFilters { get; } = new[]
    {
        new EnumOption<RequestStatus?>(null, "Na čekanju i potvrđeni"),
        new EnumOption<RequestStatus?>(RequestStatus.PendingApproval, "Na čekanju"),
        new EnumOption<RequestStatus?>(RequestStatus.Approved, "Potvrđeni")
    };

    public ObservableCollection<Building> Buildings { get; }
    public ObservableCollection<AccessRequest> Requests { get; }

    public Building SelectedBuilding
    {
        get => _selectedBuilding;
        set
        {
            if (SetProperty(ref _selectedBuilding, value))
                Reload();
        }
    }

    public RequestStatus? StatusFilter
    {
        get => _statusFilter;
        set
        {
            if (SetProperty(ref _statusFilter, value))
                Reload();
        }
    }

    public AccessRequest SelectedRequest
    {
        get => _selectedRequest;
        set => SetProperty(ref _selectedRequest, value);
    }

    public string RejectionReason
    {
        get => _rejectionReason;
        set => SetProperty(ref _rejectionReason, value);
    }

    public string Message { get => _message; private set => SetProperty(ref _message, value); }

    public ICommand ApproveCommand { get; }
    public ICommand RejectCommand { get; }

    private bool CanApprove() => _selectedRequest != null && _selectedRequest.IsPending;

    private bool CanReject()
        => CanApprove() && !string.IsNullOrWhiteSpace(_rejectionReason);

    private void Approve()
    {
        try
        {
            _accessRequestService.ApproveRequest(_selectedRequest.Id, _manager.Jmbg);
            Message = "Zahtev je potvrđen.";
            Reload();
        }
        catch (DomainException exception)
        {
            Message = exception.Message;
        }
    }

    private void Reject()
    {
        try
        {
            _accessRequestService.RejectRequest(_selectedRequest.Id, _manager.Jmbg, _rejectionReason);
            Message = "Zahtev je odbijen.";
            RejectionReason = string.Empty;
            Reload();
        }
        catch (DomainException exception)
        {
            Message = exception.Message;
        }
    }

    private void Reload()
    {
        Requests.Clear();
        if (_selectedBuilding == null)
            return;

        List<AccessRequest> requests = _accessRequestService.GetBuildingRequestsForManager(
            _selectedBuilding.Code, _statusFilter);
        for (int i = 0; i < requests.Count; i++)
            Requests.Add(requests[i]);

        SelectedRequest = null;
    }
}
