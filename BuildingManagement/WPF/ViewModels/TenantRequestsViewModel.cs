using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Sims.BuildingManagement.Application.UseCases;
using Sims.BuildingManagement.Domain.Models;

namespace Sims.BuildingManagement.WPF.ViewModels;


// Prikaz svih zahteva prijavljenog stanara uz filtriranje 
// po statusu. Zahteve koji cekaju
// odobrenje stanar moze povuci, ako ih povuce 
// onda se brisu iz sistema.

public class TenantRequestsViewModel : ViewModelBase
{
    private readonly AccessRequestService _accessRequestService;
    private readonly User _tenant;

    private RequestStatus? _statusFilter;
    private AccessRequest _selectedRequest;
    private string _message = string.Empty;

    public TenantRequestsViewModel(AccessRequestService accessRequestService, User tenant)
    {
        _accessRequestService = accessRequestService;
        _tenant = tenant;

        Requests = new ObservableCollection<AccessRequest>();
        WithdrawCommand = new RelayCommand(Withdraw, CanWithdraw);
        Reload();
    }

    public IReadOnlyList<EnumOption<RequestStatus?>> StatusFilters { get; } = new[]
    {
        new EnumOption<RequestStatus?>(null, "Svi"),
        new EnumOption<RequestStatus?>(RequestStatus.PendingApproval, "Na čekanju"),
        new EnumOption<RequestStatus?>(RequestStatus.Approved, "Potvrđeni"),
        new EnumOption<RequestStatus?>(RequestStatus.Rejected, "Odbijeni")
    };

    public ObservableCollection<AccessRequest> Requests { get; }

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

    public string Message { get => _message; private set => SetProperty(ref _message, value); }

    public ICommand WithdrawCommand { get; }

    private bool CanWithdraw() => _selectedRequest != null && _selectedRequest.IsPending;

    private void Withdraw()
    {
        try
        {
            _accessRequestService.WithdrawRequest(_selectedRequest.Id, _tenant.Jmbg);
            Message = "Zahtev je povučen.";
            Reload();
        }
        catch (DomainException exception)
        {
            Message = exception.Message;
        }
    }

    private void Reload()
    {
        List<AccessRequest> requests = _accessRequestService.GetTenantRequests(_tenant.Jmbg, _statusFilter);
        Requests.Clear();
        for (int i = 0; i < requests.Count; i++)
            Requests.Add(requests[i]);

        SelectedRequest = null;
    }
}
