using System.Windows;
using System.Windows.Controls;
using Sims.BuildingManagement.WPF.ViewModels;

namespace Sims.BuildingManagement.WPF.Views;

// Prosledjuje lozinku iz PasswordBox u ViewModel
public partial class RegisterManagerView : UserControl
{
    public RegisterManagerView()
    {
        InitializeComponent();
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        => ((RegisterManagerViewModel)DataContext).Password = ((PasswordBox)sender).Password;
}
