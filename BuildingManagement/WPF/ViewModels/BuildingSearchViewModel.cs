using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using Sims.BuildingManagement.Application.Search;
using Sims.BuildingManagement.Application.UseCases;
using Sims.BuildingManagement.Domain.Models;

namespace Sims.BuildingManagement.WPF.ViewModels;


// Pretraga zgrada po adresi, naselju, broju spratova 
// ili po stanovima. 
// ne razlikuje mala/velika slova. 
// Kombinovana pretraga po stanovima prihvata:
// broj soba / broj stanara / broj soba + broj stanara


public class BuildingSearchViewModel : ViewModelBase
{
    private readonly BuildingService _buildingService;

    private BuildingSearchField _field = BuildingSearchField.Address;
    private ApartmentSearchField _apartmentField = ApartmentSearchField.RoomCount;
    private string _text = string.Empty;
    private string _floorCountText = string.Empty;
    private string _roomCountText = string.Empty;
    private string _capacityText = string.Empty;
    private string _combinedQuery = string.Empty;
    private string _message = string.Empty;

    public BuildingSearchViewModel(BuildingService buildingService)
    {
        _buildingService = buildingService;
        Results = new ObservableCollection<Building>();
        SearchCommand = new RelayCommand(Search);
    }

    public IReadOnlyList<EnumOption<BuildingSearchField>> FieldOptions { get; } = new[]
    {
        new EnumOption<BuildingSearchField>(BuildingSearchField.Address, "Adresa"),
        new EnumOption<BuildingSearchField>(BuildingSearchField.Neighborhood, "Naselje"),
        new EnumOption<BuildingSearchField>(BuildingSearchField.FloorCount, "Broj spratova"),
        new EnumOption<BuildingSearchField>(BuildingSearchField.Apartments, "Stanovi")
    };

    public IReadOnlyList<EnumOption<ApartmentSearchField>> ApartmentFieldOptions { get; } = new[]
    {
        new EnumOption<ApartmentSearchField>(ApartmentSearchField.RoomCount, "Broj soba"),
        new EnumOption<ApartmentSearchField>(ApartmentSearchField.TenantCapacity, "Broj stanara"),
        new EnumOption<ApartmentSearchField>(
            ApartmentSearchField.RoomCountAndTenantCapacity, "Broj soba + broj stanara")
    };

    public BuildingSearchField Field
    {
        get => _field;
        set
        {
            if (SetProperty(ref _field, value))
                RaiseInputVisibilityChanged();
        }
    }

    public ApartmentSearchField ApartmentField
    {
        get => _apartmentField;
        set
        {
            if (SetProperty(ref _apartmentField, value))
                RaiseInputVisibilityChanged();
        }
    }

    public string Text { get => _text; set => SetProperty(ref _text, value); }
    public string FloorCountText { get => _floorCountText; set => SetProperty(ref _floorCountText, value); }
    public string RoomCountText { get => _roomCountText; set => SetProperty(ref _roomCountText, value); }
    public string CapacityText { get => _capacityText; set => SetProperty(ref _capacityText, value); }
    public string CombinedQuery { get => _combinedQuery; set => SetProperty(ref _combinedQuery, value); }
    public string Message { get => _message; private set => SetProperty(ref _message, value); }

    public bool IsTextInput => _field == BuildingSearchField.Address || _field == BuildingSearchField.Neighborhood;
    public bool IsFloorCountInput => _field == BuildingSearchField.FloorCount;
    public bool IsApartmentInput => _field == BuildingSearchField.Apartments;
    public bool IsRoomCountInput => IsApartmentInput && _apartmentField == ApartmentSearchField.RoomCount;
    public bool IsCapacityInput => IsApartmentInput && _apartmentField == ApartmentSearchField.TenantCapacity;
    public bool IsCombinedInput => IsApartmentInput && _apartmentField == ApartmentSearchField.RoomCountAndTenantCapacity;

    public ObservableCollection<Building> Results { get; }

    public ICommand SearchCommand { get; }

    private void Search()
    {
        try
        {
            BuildingSearchCriteria criteria = BuildCriteria();
            ShowResults(_buildingService.Search(criteria));
        }
        catch (FormatException)
        {
            Message = "Unesite ispravan broj.";
        }
    }

    private BuildingSearchCriteria BuildCriteria()
    {
        BuildingSearchCriteria criteria = new BuildingSearchCriteria { Field = _field };
        switch (_field)
        {
            case BuildingSearchField.Address:
            case BuildingSearchField.Neighborhood:
                criteria.Text = _text;
                break;
            case BuildingSearchField.FloorCount:
                criteria.FloorCount = ParseInt(_floorCountText);
                break;
            case BuildingSearchField.Apartments:
                FillApartmentCriteria(criteria);
                break;
        }

        return criteria;
    }

    private void FillApartmentCriteria(BuildingSearchCriteria criteria)
    {
        criteria.ApartmentField = _apartmentField;
        switch (_apartmentField)
        {
            case ApartmentSearchField.RoomCount:
                criteria.RoomCount = ParseInt(_roomCountText);
                break;
            case ApartmentSearchField.TenantCapacity:
                criteria.TenantCapacity = ParseInt(_capacityText);
                break;
            case ApartmentSearchField.RoomCountAndTenantCapacity:
                FillCombinedCriteria(criteria);
                break;
        }
    }

    private void FillCombinedCriteria(BuildingSearchCriteria criteria)
    {
        bool isOr = _combinedQuery.Contains('|');
        char separator = isOr ? '|' : '&';
        string[] parts = _combinedQuery.Split(separator);
        if (parts.Length != 2)
            throw new FormatException();

        criteria.Operator = isOr ? SearchOperator.Or : SearchOperator.And;
        criteria.RoomCount = ParseInt(parts[0]);
        criteria.TenantCapacity = ParseInt(parts[1]);
    }

    private void ShowResults(IReadOnlyList<Building> buildings)
    {
        Results.Clear();
        for (int i = 0; i < buildings.Count; i++)
            Results.Add(buildings[i]);

        Message = buildings.Count == 0 ? "Nema zgrada koje odgovaraju pretrazi." : string.Empty;
    }

    private void RaiseInputVisibilityChanged()
    {
        OnPropertyChanged(nameof(IsTextInput));
        OnPropertyChanged(nameof(IsFloorCountInput));
        OnPropertyChanged(nameof(IsApartmentInput));
        OnPropertyChanged(nameof(IsRoomCountInput));
        OnPropertyChanged(nameof(IsCapacityInput));
        OnPropertyChanged(nameof(IsCombinedInput));
    }

    private static int ParseInt(string value)
        => int.Parse(value.Trim(), CultureInfo.InvariantCulture);
}
