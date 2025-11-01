using System;
using UniCliqueBackend.Domain.Entities;

namespace UniCliqueBackend.Application.Interfaces.Services
{
    public interface ITokenService
    {
        string CreateAccessToken(User user, DateTime expiresAt);
    }
}


