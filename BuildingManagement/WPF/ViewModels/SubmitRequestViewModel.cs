using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using Sims.BuildingManagement.Application.UseCases;
using Sims.BuildingManagement.Domain.Models;

namespace Sims.BuildingManagement.WPF.ViewModels;


// Podnosenje zahteva za pristup stanu. Nakon unosa broja 
// stana korisnik pokrece proveru
// zauzetosti; upozorenje o zauzetom stanu ne 
// sprecava kreiranje zahteva. Zahtev se salje
// tek kada korisnik potvrdi.

public class SubmitRequestViewModel : ViewModelBase
{
    private readonly AccessRequestService _accessRequestService;
    private readonly User _tenant;

    private Building _selectedBuilding;
    private string _apartmentNumberText = string.Empty;
    private string _warning = string.Empty;
    private string _message = string.Empty;
    private bool _isChecked;

    public SubmitRequestViewModel(BuildingService buildingService,
        AccessRequestService accessRequestService, User tenant)
    {
        _accessRequestService = accessRequestService;
        _tenant = tenant;

        Buildings = new ObservableCollection<Building>(buildingService.GetApprovedBuildings());
        CheckApartmentCommand = new RelayCommand(CheckApartment, CanCheck);
        ConfirmCommand = new RelayCommand(Confirm, () => _isChecked);
        CancelCommand = new RelayCommand(ResetForm);
    }

    public ObservableCollection<Building> Buildings { get; }

    public Building SelectedBuilding
    {
        get => _selectedBuilding;
        set
        {
            if (SetProperty(ref _selectedBuilding, value))
                InvalidateCheck();
        }
    }

    public string ApartmentNumberText
    {
        get => _apartmentNumberText;
        set
        {
            if (SetProperty(ref _apartmentNumberText, value))
                InvalidateCheck();
        }
    }

    public string Warning { get => _warning; private set => SetProperty(ref _warning, value); }
    public string Message { get => _message; private set => SetProperty(ref _message, value); }

    public ICommand CheckApartmentCommand { get; }
    public ICommand ConfirmCommand { get; }
    public ICommand CancelCommand { get; }

    private bool CanCheck()
        => _selectedBuilding != null && !string.IsNullOrWhiteSpace(_apartmentNumberText);

    private void CheckApartment()
    {
        try
        {
            int apartmentNumber = ParseApartmentNumber();
            bool occupied = _accessRequestService.IsApartmentOccupied(
                _selectedBuilding.Code, apartmentNumber);

            Warning = occupied
                ? "Upozorenje: za uneti stan je već učlanjen stanar. Proverite broj stana pre potvrde."
                : string.Empty;
            Message = "Broj stana je proveren. Potvrdite kreiranje zahteva ili izmenite broj stana.";
            _isChecked = true;
        }
        catch (FormatException)
        {
            Message = "Broj stana mora biti ceo broj.";
        }
    }

    private void Confirm()
    {
        try
        {
            int apartmentNumber = ParseApartmentNumber();
            _accessRequestService.SubmitRequest(_tenant.Jmbg, _selectedBuilding.Code, apartmentNumber);
            Message = "Zahtev je kreiran i čeka odobrenje upravnika.";
            ResetForm();
        }
        catch (FormatException)
        {
            Message = "Broj stana mora biti ceo broj.";
        }
        catch (DomainException exception)
        {
            Message = exception.Message;
        }
    }

    private void InvalidateCheck()
    {
        _isChecked = false;
        Warning = string.Empty;
    }

    private void ResetForm()
    {
        SelectedBuilding = null;
        ApartmentNumberText = string.Empty;
        Warning = string.Empty;
        _isChecked = false;
    }

    private int ParseApartmentNumber()
        => int.Parse(_apartmentNumberText.Trim(), CultureInfo.InvariantCulture);
}
