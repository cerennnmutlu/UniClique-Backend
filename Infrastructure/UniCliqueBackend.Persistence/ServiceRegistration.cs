using Microsoft.Extensions.DependencyInjection;
using UniCliqueBackend.Application.Interfaces.Repositories;
using UniCliqueBackend.Application.Interfaces.Security;
using UniCliqueBackend.Application.Interfaces.Services;
using UniCliqueBackend.Persistence.Repositories;
using UniCliqueBackend.Persistence.Security;
using UniCliqueBackend.Persistence.Tokens;
using UniCliqueBackend.Persistence.Email;

namespace UniCliqueBackend.Persistence
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
            services.AddScoped<ITokenService, JwtTokenService>();
            services.AddScoped<IEmailService, SmtpEmailService>();

            return services;
        }
    }
}
