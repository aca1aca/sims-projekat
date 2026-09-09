using System;
using System.Windows.Input;
using Sims.BuildingManagement.Application.UseCases;
using Sims.BuildingManagement.Domain.Models;

namespace Sims.BuildingManagement.WPF.ViewModels;


// ViewModel prozora za prijavu 
// Ista forma se koristi za 
// sve tipove korisnika


public class LoginViewModel : ViewModelBase
{
    private readonly AuthenticationService _authenticationService;
    private readonly UserService _userService;

    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _errorMessage = string.Empty;

    public LoginViewModel(AuthenticationService authenticationService, UserService userService)
    {
        _authenticationService = authenticationService;
        _userService = userService;
        LogInCommand = new RelayCommand(LogIn);
    }

    //Podize se kada je prijava uspesna; 
    // salje prijavljenog korisnika
    public event Action<User> LoginSucceeded;

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

    public ICommand LogInCommand { get; }

    public RegisterTenantViewModel CreateRegistrationViewModel()
        => new RegisterTenantViewModel(_userService);

    private void LogIn()
    {
        try
        {
            User user = _authenticationService.LogIn(Email, Password);
            ErrorMessage = string.Empty;
            LoginSucceeded?.Invoke(user);
        }
        catch (DomainException exception)
        {
            ErrorMessage = exception.Message;
        }
    }
}
