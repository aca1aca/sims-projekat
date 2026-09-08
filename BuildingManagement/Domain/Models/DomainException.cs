using System;

namespace Sims.BuildingManagement.Domain.Models;

// Greska u poslovnom pravilu,
// poruka za korisnika
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}
