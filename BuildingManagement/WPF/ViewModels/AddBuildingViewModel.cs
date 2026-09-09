using System;
using System.Globalization;
using System.Windows.Input;
using Sims.BuildingManagement.Application.UseCases;
using Sims.BuildingManagement.Domain.Models;

namespace Sims.BuildingManagement.WPF.ViewModels;

// Unos nove zgrade od strane administratora. 
// Zgrada se kreira u statusu "ceka odobrenje"
// postaje vidljiva tek kada je odobri upravnik 
// naveden preko svog JMBG-a


public class AddBuildingViewModel : ViewModelBase
{
    private readonly BuildingService _buildingService;

    private string _code = string.Empty;
    private string _street = string.Empty;
    private string _number = string.Empty;
    private string _neighborhood = string.Empty;
    private string _city = string.Empty;
    private string _country = string.Empty;
    private string _floorCountText = string.Empty;
    private string _managerJmbg = string.Empty;
    private string _message = string.Empty;
    private bool _isError;

    public AddBuildingViewModel(BuildingService buildingService)
    {
        _buildingService = buildingService;
        AddCommand = new RelayCommand(Add);
    }

    public string Code { get => _code; set => SetProperty(ref _code, value); }
    public string Street { get => _street; set => SetProperty(ref _street, value); }
    public string Number { get => _number; set => SetProperty(ref _number, value); }
    public string Neighborhood { get => _neighborhood; set => SetProperty(ref _neighborhood, value); }
    public string City { get => _city; set => SetProperty(ref _city, value); }
    public string Country { get => _country; set => SetProperty(ref _country, value); }
    public string FloorCountText { get => _floorCountText; set => SetProperty(ref _floorCountText, value); }
    public string ManagerJmbg { get => _managerJmbg; set => SetProperty(ref _managerJmbg, value); }

    public string Message { get => _message; private set => SetProperty(ref _message, value); }
    public bool IsError { get => _isError; private set => SetProperty(ref _isError, value); }

    public ICommand AddCommand { get; }

    private void Add()
    {
        try
        {
            Building building = new Building(Code, Street, Number, Neighborhood, City, Country,
                ParseInt(_floorCountText), ManagerJmbg, BuildingStatus.PendingApproval);
            _buildingService.RegisterBuilding(building);

            IsError = false;
            Message = $"Zgrada {building.Code} je uneta i čeka odobrenje upravnika.";
            ClearInputs();
        }
        catch (FormatException)
        {
            Fail("Broj spratova mora biti ceo broj.");
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
        Code = string.Empty;
        Street = string.Empty;
        Number = string.Empty;
        Neighborhood = string.Empty;
        City = string.Empty;
        Country = string.Empty;
        FloorCountText = string.Empty;
        ManagerJmbg = string.Empty;
    }

    private static int ParseInt(string value)
        => int.Parse(value.Trim(), CultureInfo.InvariantCulture);
}
