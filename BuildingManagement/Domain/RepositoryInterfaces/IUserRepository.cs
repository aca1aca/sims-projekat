using System.Collections.Generic;
using Sims.BuildingManagement.Domain.Models;

namespace Sims.BuildingManagement.Domain.RepositoryInterfaces;

// funkcije koje treba da se implementiaju kad se pozove interfejs

public interface IUserRepository
{
    List<User> GetAll();

    User GetByEmail(string email);

    User GetByJmbg(string jmbg);

    void Create(User user);
}
