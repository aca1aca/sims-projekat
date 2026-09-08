using System.Linq;
using Sims.BuildingManagement.Domain.Models;
using Sims.BuildingManagement.Domain.RepositoryInterfaces;

namespace Sims.BuildingManagement.Application.UseCases;

// Koordinator za registraciju korisnika
// Stanar se registruje sam, 
// a upravnika registruje administrator

public class UserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public User RegisterTenant(User tenant)
    {
        RequireUniqueEmail(tenant.Email);
        RequireUniqueJmbg(tenant.Jmbg);
        RequireUniquePassword(tenant.Password);
        _userRepository.Create(tenant);
        return tenant;
    }

    public User RegisterManager(User manager)
    {
        RequireUniqueEmail(manager.Email);
        RequireUniqueJmbg(manager.Jmbg);
        _userRepository.Create(manager);
        return manager;
    }

    private void RequireUniqueEmail(string email)
    {
        if (_userRepository.GetByEmail(email) != null)
            throw new DomainException("Korisnik sa datim email-om već postoji.");
    }

    private void RequireUniqueJmbg(string jmbg)
    {
        if (_userRepository.GetByJmbg(jmbg) != null)
            throw new DomainException("Korisnik sa datim JMBG-om već postoji.");
    }

    private void RequireUniquePassword(string password)
    {
        bool passwordTaken = _userRepository.GetAll().Any(user => user.HasPassword(password));
        if (passwordTaken)
            throw new DomainException("Uneta lozinka je već u upotrebi, izaberite drugu.");
    }
}
