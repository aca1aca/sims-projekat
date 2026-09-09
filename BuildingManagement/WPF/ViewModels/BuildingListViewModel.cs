using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sims.BuildingManagement.Application.UseCases;
using Sims.BuildingManagement.Domain.Models;

namespace Sims.BuildingManagement.WPF.ViewModels;


// Prikaz svih odobrenih zgrada (bez JMBG-a upravnika)
// uz opciju sortiranja po broju spratova.

public class BuildingListViewModel : ViewModelBase
{
    private readonly BuildingService _buildingService;
    private bool _sortByFloorCount;

    public BuildingListViewModel(BuildingService buildingService)
    {
        _buildingService = buildingService;
        Buildings = new ObservableCollection<Building>();
        Reload();
    }

    public ObservableCollection<Building> Buildings { get; }

    public bool SortByFloorCount
    {
        get => _sortByFloorCount;
        set
        {
            if (SetProperty(ref _sortByFloorCount, value))
                Reload();
        }
    }

    private void Reload()
    {
        List<Building> buildings = _sortByFloorCount
            ? _buildingService.GetApprovedBuildingsSortedByFloorCount()
            : _buildingService.GetApprovedBuildings();

        Buildings.Clear();
        for (int i = 0; i < buildings.Count; i++)
            Buildings.Add(buildings[i]);
    }
}
