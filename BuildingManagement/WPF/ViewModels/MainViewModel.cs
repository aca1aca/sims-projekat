using System;
using System.Collections.Generic;
using System.Windows.Input;
using Sims.BuildingManagement.Application.UseCases;
using Sims.BuildingManagement.Domain.Models;

namespace Sims.BuildingManagement.WPF.ViewModels;

// ViewModel glavnog prozora. Sastavlja meni prema ulozi 
// prijavljenog korisnika i menja
// aktivni ekran (CurrentViewMode) prema izabranoj stavci menija.

public class MainViewModel : ViewModelBase
{
    private readonly User _currentUser;
    private readonly UserService _userService;
    private readonly BuildingService _buildingService;
    private readonly ApartmentService _apartmentService;
    private readonly AccessRequestService _accessRequestService;

    private ViewModelBase _currentViewModel;

    public MainViewModel(User currentUser, UserService userService, BuildingService buildingService,
        ApartmentService apartmentService, AccessRequestService accessRequestService)
    {
        _currentUser = currentUser;
        _userService = userService;
        _buildingService = buildingService;
        _apartmentService = apartmentService;
        _accessRequestService = accessRequestService;

        SignOutCommand = new RelayCommand(() => SignOutRequested?.Invoke());
        MenuItems = BuildMenu();
        OpenFirst();
    }

    /// Kada korisnik zatrazi odjavu
    public event Action SignOutRequested;

    public string UserName => _currentUser.FullName;

    public string RoleText => DescribeRole(_currentUser.Role);

    public IReadOnlyList<NavigationItem> MenuItems { get; }

    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        private set => SetProperty(ref _currentViewModel, value);
    }

    public ICommand SignOutCommand { get; }

    private List<NavigationItem> BuildMenu()
    {
        List<NavigationItem> items = new List<NavigationItem>
        {
            NavigationTo("Sve zgrade", () => new BuildingListViewModel(_buildingService)),
            NavigationTo("Pretraga zgrada", () => new BuildingSearchViewModel(_buildingService))
        };

        if (_currentUser.IsTenant)
            AddTenantMenu(items);
        else if (_currentUser.IsManager)
            AddManagerMenu(items);
        else if (_currentUser.IsAdministrator)
            AddAdministratorMenu(items);

        return items;
    }

    private void AddTenantMenu(List<NavigationItem> items)
    {
        items.Add(NavigationTo("Podnošenje zahteva",
            () => new SubmitRequestViewModel(_buildingService, _accessRequestService, _currentUser)));
        items.Add(NavigationTo("Moji zahtevi",
            () => new TenantRequestsViewModel(_accessRequestService, _currentUser)));
    }

    private void AddManagerMenu(List<NavigationItem> items)
    {
        items.Add(NavigationTo("Moje zgrade",
            () => new ManagerBuildingsViewModel(_buildingService, _currentUser)));
        items.Add(NavigationTo("Zahtevi za zgrade",
            () => new ManagerRequestsViewModel(_buildingService, _accessRequestService, _currentUser)));
        items.Add(NavigationTo("Unos stana",
            () => new AddApartmentViewModel(_buildingService, _apartmentService, _currentUser)));
    }

    private void AddAdministratorMenu(List<NavigationItem> items)
    {
        items.Add(NavigationTo("Registracija upravnika",
            () => new RegisterManagerViewModel(_userService)));
        items.Add(NavigationTo("Unos zgrade",
            () => new AddBuildingViewModel(_buildingService)));
    }

    private NavigationItem NavigationTo(string title, Func<ViewModelBase> createViewModel)
        => new NavigationItem(title, new RelayCommand(() => CurrentViewModel = createViewModel()));

    private void OpenFirst() => MenuItems[0].OpenCommand.Execute(null);

    private static string DescribeRole(UserRole role)
    {
        return role switch
        {
            UserRole.Administrator => "Administrator",
            UserRole.Manager => "Upravnik zgrade",
            UserRole.Tenant => "Stanar",
            _ => string.Empty
        };
    }
}
