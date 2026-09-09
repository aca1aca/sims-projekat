using System;
using System.IO;
using System.Windows;
using Sims.BuildingManagement.Application.UseCases;
using Sims.BuildingManagement.Domain.Models;
using Sims.BuildingManagement.Domain.RepositoryInterfaces;
using Sims.BuildingManagement.Repositories;
using Sims.BuildingManagement.WPF.ViewModels;
using Sims.BuildingManagement.WPF.Views;

namespace Sims.BuildingManagement;


// Koren aplikacije (Injector): 
// kreira csv-ove,
// sastavlja servise i pokrece prozor za prijavu. 
// Zavisnosti se ubrizgavaju kroz konstruktore.

public partial class App : System.Windows.Application
{
    private AuthenticationService _authenticationService;
    private UserService _userService;
    private BuildingService _buildingService;
    private ApartmentService _apartmentService;
    private AccessRequestService _accessRequestService;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ComposeServices(ResolveDataDirectory());
        ShowLoginWindow();
    }

    private void ComposeServices(string dataDirectory)
    {
        IUserRepository userRepository =
            new CsvUserRepository(Path.Combine(dataDirectory, "users.csv"));
        IBuildingRepository buildingRepository =
            new CsvBuildingRepository(Path.Combine(dataDirectory, "buildings.csv"));
        IApartmentRepository apartmentRepository =
            new CsvApartmentRepository(Path.Combine(dataDirectory, "apartments.csv"));
        IAccessRequestRepository accessRequestRepository =
            new CsvAccessRequestRepository(Path.Combine(dataDirectory, "access_requests.csv"));

        _authenticationService = new AuthenticationService(userRepository);
        _userService = new UserService(userRepository);
        _buildingService =
            new BuildingService(buildingRepository, apartmentRepository, userRepository);
        _apartmentService =
            new ApartmentService(apartmentRepository, buildingRepository);
        _accessRequestService =
            new AccessRequestService(accessRequestRepository, buildingRepository, apartmentRepository);
    }

    private void ShowLoginWindow()
    {
        LoginViewModel loginViewModel = new LoginViewModel(_authenticationService, _userService);
        loginViewModel.LoginSucceeded += OpenMainWindow;

        LoginWindow loginWindow = new LoginWindow { DataContext = loginViewModel };
        MainWindow = loginWindow;
        loginWindow.Show();
    }

    private void OpenMainWindow(User signedInUser)
    {
        MainViewModel mainViewModel = new MainViewModel(signedInUser, _userService,
            _buildingService, _apartmentService, _accessRequestService);
        mainViewModel.SignOutRequested += ReturnToLogin;

        MainWindow mainWindow = new MainWindow { DataContext = mainViewModel };
        Window previousWindow = MainWindow;
        MainWindow = mainWindow;
        mainWindow.Show();
        previousWindow?.Close();
    }

    private void ReturnToLogin()
    {
        Window previousWindow = MainWindow;
        ShowLoginWindow();
        previousWindow?.Close();
    }



    //Pronalazi apsolutnu putanju do foldera Data/ 
    // u kom su CSV datoteke i vraca string sa putanjom
    
    
    private static string ResolveDataDirectory()
    {
        string directory = AppContext.BaseDirectory;
        for (int i = 0; i < 6; i++)
        {
            string candidate = Path.Combine(directory, "Data");
            if (Directory.Exists(candidate))
                return candidate;

            directory = Path.GetFullPath(Path.Combine(directory, ".."));
        }

        return Path.Combine(AppContext.BaseDirectory, "Data");
    }
}
