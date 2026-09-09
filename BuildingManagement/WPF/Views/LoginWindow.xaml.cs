using System.Windows;
using System.Windows.Controls;
using Sims.BuildingManagement.WPF.ViewModels;

namespace Sims.BuildingManagement.WPF.Views;


// prosledjivanje lozinke iz PasswordBox
// (koji ne podrzava binding) i otvaranje forme za registracija
// stanara

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
    }

    private LoginViewModel ViewModel => (LoginViewModel)DataContext;

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        => ViewModel.Password = ((PasswordBox)sender).Password;

    private void Register_Click(object sender, RoutedEventArgs e)
    {
        RegisterTenantWindow dialog = new RegisterTenantWindow
        {
            DataContext = ViewModel.CreateRegistrationViewModel(),
            Owner = this
        };
        dialog.ShowDialog();
    }
}
