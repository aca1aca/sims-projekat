using System.Windows.Input;
using Sims.BuildingManagement.Application.UseCases;
using Sims.BuildingManagement.Domain.Models;

namespace Sims.BuildingManagement.WPF.ViewModels;


// ViewModel forme za samostalnu registraciju stanara

public class RegisterTenantViewModel : ViewModelBase
{
    private readonly UserService _userService;

    private string _jmbg = string.Empty;
    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string _mobilePhone = string.Empty;
    private string _message = string.Empty;
    private bool _isError;

    public RegisterTenantViewModel(UserService userService)
    {
        _userService = userService;
        RegisterCommand = new RelayCommand(Register);
    }

    public string Jmbg { get => _jmbg; set => SetProperty(ref _jmbg, value); }
    public string Email { get => _email; set => SetProperty(ref _email, value); }
    public string Password { get => _password; set => SetProperty(ref _password, value); }
    public string FirstName { get => _firstName; set => SetProperty(ref _firstName, value); }
    public string LastName { get => _lastName; set => SetProperty(ref _lastName, value); }
    public string MobilePhone { get => _mobilePhone; set => SetProperty(ref _mobilePhone, value); }

    public string Message { get => _message; private set => SetProperty(ref _message, value); }
    public bool IsError { get => _isError; private set => SetProperty(ref _isError, value); }

    public ICommand RegisterCommand { get; }

    private void Register()
    {
        try
        {
            User tenant = new User(Jmbg, Email, Password, FirstName, LastName, MobilePhone, UserRole.Tenant);
            _userService.RegisterTenant(tenant);
            IsError = false;
            Message = "Registracija je uspešna. Sada se možete prijaviti.";
        }
        catch (DomainException exception)
        {
            IsError = true;
            Message = exception.Message;
        }
    }
}
