using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Sims.BuildingManagement.Application.UseCases;
using Sims.BuildingManagement.Domain.Models;

namespace Sims.BuildingManagement.WPF.ViewModels;


// Lista zgrada prijavljenog upravnika 
// (na cekanju i odobrenih). Upravnik potvrdjuje zgradu koju
// je administrator uneo - tek tada je zgrada 
// vidljiva ostalima - ili je odbija ako je pogresno
// dodeljena.

public class ManagerBuildingsViewModel : ViewModelBase
{
    private readonly BuildingService _buildingService;
    private readonly User _manager;

    private Building _selectedBuilding;
    private string _message = string.Empty;

    public ManagerBuildingsViewModel(BuildingService buildingService, User manager)
    {
        _buildingService = buildingService;
        _manager = manager;

        Buildings = new ObservableCollection<Building>();
        ApproveCommand = new RelayCommand(Approve, CanDecide);
        RejectCommand = new RelayCommand(Reject, CanDecide);
        Reload();
    }

    public ObservableCollection<Building> Buildings { get; }

    public Building SelectedBuilding
    {
        get => _selectedBuilding;
        set => SetProperty(ref _selectedBuilding, value);
    }

    public string Message { get => _message; private set => SetProperty(ref _message, value); }

    public ICommand ApproveCommand { get; }
    public ICommand RejectCommand { get; }

    private bool CanDecide() => _selectedBuilding != null && _selectedBuilding.IsPendingApproval;

    private void Approve()
    {
        Decide(() => _buildingService.ApproveBuilding(_selectedBuilding.Code, _manager.Jmbg),
            "Zgrada je odobrena i sada je vidljiva ostalim korisnicima.");
    }

    private void Reject()
    {
        Decide(() => _buildingService.RejectBuilding(_selectedBuilding.Code, _manager.Jmbg),
            "Zgrada je odbijena.");
    }

    private void Decide(System.Action decision, string successMessage)
    {
        try
        {
            decision();
            Message = successMessage;
            Reload();
        }
        catch (DomainException exception)
        {
            Message = exception.Message;
        }
    }

    private void Reload()
    {
        List<Building> buildings = _buildingService.GetBuildingsManagedBy(_manager.Jmbg);
        Buildings.Clear();
        for (int i = 0; i < buildings.Count; i++)
            Buildings.Add(buildings[i]);

        SelectedBuilding = null;
    }
}
