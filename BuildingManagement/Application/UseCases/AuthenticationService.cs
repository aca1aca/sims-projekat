using Sims.BuildingManagement.Domain.Models;
using Sims.BuildingManagement.Domain.RepositoryInterfaces;

namespace Sims.BuildingManagement.Application.UseCases;


// Koordinator koji proverava vraca prijavljenog korisnika.
// Ista logika prijave se koristi za sve tipove korisnika.

public class AuthenticationService
{
    private readonly IUserRepository _userRepository;

    public AuthenticationService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public User LogIn(string email, string password)
    {
        User user = _userRepository.GetByEmail(email);
        if (user == null || !user.HasPassword(password))
            throw new DomainException("Pogrešan email ili lozinka.");

        return user;
    }
}
