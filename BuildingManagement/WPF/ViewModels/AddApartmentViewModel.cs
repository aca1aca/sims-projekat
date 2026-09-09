using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using Sims.BuildingManagement.Application.UseCases;
using Sims.BuildingManagement.Domain.Models;

namespace Sims.BuildingManagement.WPF.ViewModels;

// Unos stana u jednu od zgrada prijavljenog upravnika. 
// Upravnik unosi sve podatke o stanu.

public class AddApartmentViewModel : ViewModelBase
{
    private readonly ApartmentService _apartmentService;
    private readonly User _manager;

    private Building _selectedBuilding;
    private string _numberText = string.Empty;
    private string _description = string.Empty;
    private string _roomCountText = string.Empty;
    private string _maxTenantCountText = string.Empty;
    private string _message = string.Empty;
    private bool _isError;

    public AddApartmentViewModel(BuildingService buildingService,
        ApartmentService apartmentService, User manager)
    {
        _apartmentService = apartmentService;
        _manager = manager;

        Buildings = new ObservableCollection<Building>(buildingService.GetBuildingsManagedBy(manager.Jmbg));
        AddCommand = new RelayCommand(Add);
    }

    public ObservableCollection<Building> Buildings { get; }

    public Building SelectedBuilding
    {
        get => _selectedBuilding;
        set => SetProperty(ref _selectedBuilding, value);
    }

    public string NumberText { get => _numberText; set => SetProperty(ref _numberText, value); }
    public string Description { get => _description; set => SetProperty(ref _description, value); }
    public string RoomCountText { get => _roomCountText; set => SetProperty(ref _roomCountText, value); }
    public string MaxTenantCountText { get => _maxTenantCountText; set => SetProperty(ref _maxTenantCountText, value); }

    public string Message { get => _message; private set => SetProperty(ref _message, value); }
    public bool IsError { get => _isError; private set => SetProperty(ref _isError, value); }

    public ICommand AddCommand { get; }

    private void Add()
    {
        if (_selectedBuilding == null)
        {
            Fail("Izaberite zgradu.");
            return;
        }

        try
        {
            Apartment apartment = new Apartment(_selectedBuilding.Code, ParseInt(_numberText),
                _description, ParseInt(_roomCountText), ParseInt(_maxTenantCountText));
            _apartmentService.AddApartment(apartment, _manager.Jmbg);

            IsError = false;
            Message = $"Stan {apartment.Number} je dodat u zgradu {apartment.BuildingCode}.";
            ClearInputs();
        }
        catch (FormatException)
        {
            Fail("Broj stana, broj soba i maksimalan broj stanara moraju biti celi brojevi.");
        }
        catch (DomainException exception)
        {
            Fail(exception.Message);
        }
    }

    private void Fail(string message)
    {
        IsError = true;
        Message = message;
    }

    private void ClearInputs()
    {
        NumberText = string.Empty;
        Description = string.Empty;
        RoomCountText = string.Empty;
        MaxTenantCountText = string.Empty;
    }

    private static int ParseInt(string value)
        => int.Parse(value.Trim(), CultureInfo.InvariantCulture);
}
