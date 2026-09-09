using System;
using System.Globalization;
using System.Windows.Data;
using Sims.BuildingManagement.Domain.Models;

namespace Sims.BuildingManagement.WPF.Converters;


// Prevodi statuse zgrade i zahteva na srpski

public class StatusTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            BuildingStatus buildingStatus => Describe(buildingStatus),
            RequestStatus requestStatus => Describe(requestStatus),
            _ => value?.ToString() ?? string.Empty
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();

    private static string Describe(BuildingStatus status)
    {
        return status switch
        {
            BuildingStatus.PendingApproval => "Čeka odobrenje",
            BuildingStatus.Approved => "Odobrena",
            BuildingStatus.Rejected => "Odbijena",
            _ => status.ToString()
        };
    }

    private static string Describe(RequestStatus status)
    {
        return status switch
        {
            RequestStatus.PendingApproval => "Čeka odobrenje",
            RequestStatus.Approved => "Potvrđen",
            RequestStatus.Rejected => "Odbijen",
            _ => status.ToString()
        };
    }
}
