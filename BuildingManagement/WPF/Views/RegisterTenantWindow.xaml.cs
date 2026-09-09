using System.Windows;
using System.Windows.Controls;
using Sims.BuildingManagement.WPF.ViewModels;

namespace Sims.BuildingManagement.WPF.Views;

// Forme za registraciju stanara
// (prosledjivanje lozinke i zatvaranje)
public partial class RegisterTenantWindow : Window
{
    public RegisterTenantWindow()
    {
        InitializeComponent();
    }

    private RegisterTenantViewModel ViewModel => (RegisterTenantViewModel)DataContext;

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        => ViewModel.Password = ((PasswordBox)sender).Password;

    private void Close_Click(object sender, RoutedEventArgs e) => Close();
}
