using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;
using UniCliqueBackend.Application.Interfaces.Services;
using UniCliqueBackend.Application.Services;

namespace UniCliqueBackend.Application
{
    public static class ServiceRegistration
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddScoped<IAuthService, AuthService>();
        }
    }
}
