using UniCliqueBackend.Application.DTOs.Auth;
using UniCliqueBackend.Application.Interfaces.Repositories;
using UniCliqueBackend.Application.Interfaces.Security;
using UniCliqueBackend.Application.Interfaces.Services;
using UniCliqueBackend.Domain.Entities;
using UniCliqueBackend.Domain.Enums;

namespace UniCliqueBackend.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private static readonly string[] _allowedProviders = new[] { "Google", "Facebook", "Instagram" };

        public AuthService(
            IUserRepository userRepository, 
            IPasswordHasher passwordHasher, 
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        public async Task RegisterAsync(RegisterRequestDto request)
        {
            if (await _userRepository.ExistsAsync(request.Email, request.Username))
            {
                throw new Exception("User with this email or username already exists.");
            }

            var (phoneE164, phoneLocal) = GetTrPhoneVariants(request.PhoneNumber);
            if (await _userRepository.PhoneExistsAsync(phoneE164, phoneLocal))
            {
                throw new Exception("Phone already exists.");
            }

            var passwordHash = _passwordHasher.HashPassword(request.Password);

            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                Username = request.Username,
                PhoneNumber = request.PhoneNumber,
                BirthDate = request.BirthDate.ToUniversalTime(), // Postgres prefers UTC
                PasswordHash = passwordHash,
                Role = RoleType.User,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            // Add consents
            if (request.AcceptKvkk)
            {
                user.UserConsents.Add(new UserConsent { ConsentType = ConsentType.KVKK, IsAccepted = true, AcceptedAt = DateTime.UtcNow });
            }
            if (request.AcceptTerms)
            {
                user.UserConsents.Add(new UserConsent { ConsentType = ConsentType.TermsOfService, IsAccepted = true, AcceptedAt = DateTime.UtcNow });
            }
            if (request.AcceptPrivacy)
            {
                user.UserConsents.Add(new UserConsent { ConsentType = ConsentType.PrivacyPolicy, IsAccepted = true, AcceptedAt = DateTime.UtcNow });
            }

            await _userRepository.AddAsync(user);
        }

        public async Task<TokenResponseDto> LoginAsync(LoginRequestDto request)
        {
            User? user = null;

            if (request.EmailOrUsername.Contains("@"))
            {
                user = await _userRepository.GetByEmailAsync(request.EmailOrUsername);
            }
            else
            {
                user = await _userRepository.GetByUsernameAsync(request.EmailOrUsername);
            }

            if (user == null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                throw new Exception("Invalid credentials.");
            }

            if (!user.IsActive || user.IsBanned)
            {
                throw new Exception("Account is not active.");
            }

            return await GenerateTokensForUser(user);
        }

        public async Task<TokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var hash = _tokenService.HashRefreshToken(request.RefreshToken);
            var token = await _userRepository.GetRefreshTokenAsync(hash);
            if (token == null)
                throw new Exception("Invalid refresh token.");
            if (token.IsRevoked)
                throw new Exception("Refresh token revoked.");
            if (token.ExpiresAt <= DateTime.UtcNow)
                throw new Exception("Refresh token expired.");

            var user = await _userRepository.GetByIdAsync(token.UserId);
            if (user == null)
                throw new Exception("User not found.");

            var result = await GenerateTokensForUser(user);
            var newHash = _tokenService.HashRefreshToken(result.RefreshToken);
            await _userRepository.RevokeRefreshTokenAsync(token, newHash);
            return result;
        }

        public async Task LogoutAsync(Guid userId, string refreshToken)
        {
            var hash = _tokenService.HashRefreshToken(refreshToken);
            var token = await _userRepository.GetRefreshTokenAsync(hash);
            if (token != null && token.UserId == userId && !token.IsRevoked)
            {
                await _userRepository.RevokeRefreshTokenAsync(token, null);
            }
        }

        public async Task<TokenResponseDto> ExternalLoginAsync(ExternalLoginRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Provider) || string.IsNullOrWhiteSpace(request.ProviderUserId))
                throw new Exception("Invalid external login request.");

            var provider = _allowedProviders.FirstOrDefault(p => string.Equals(p, request.Provider, StringComparison.OrdinalIgnoreCase));
            if (provider == null)
                throw new Exception("Unsupported provider.");

            var user = await _userRepository.GetByExternalLoginAsync(provider, request.ProviderUserId);
            if (user == null)
            {
                if (string.IsNullOrWhiteSpace(request.Email))
                    throw new Exception("Email is required for first-time external login.");

                user = await _userRepository.GetByEmailAsync(request.Email);
                if (user == null)
                {
                    var baseUsername = request.Email.Split('@')[0];
                    var usernameCandidate = baseUsername;
                    var rnd = new Random();
                    while (await _userRepository.GetByUsernameAsync(usernameCandidate) != null)
                    {
                        usernameCandidate = $"{baseUsername}{rnd.Next(1000, 9999)}";
                    }

                    var randomPassword = Guid.NewGuid().ToString("N");
                    var passwordHash = _passwordHasher.HashPassword(randomPassword);

                    user = new User
                    {
                        FullName = string.IsNullOrWhiteSpace(request.FullName) ? baseUsername : request.FullName,
                        Email = request.Email,
                        Username = usernameCandidate,
                        PhoneNumber = "+000000000000",
                        BirthDate = DateTime.UtcNow.AddYears(-18),
                        PasswordHash = passwordHash,
                        Role = RoleType.User,
                        IsActive = true,
                        IsEmailVerified = true,
                        EmailVerifiedAt = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        LastLoginAt = DateTime.UtcNow
                    };

                    await _userRepository.AddAsync(user);
                }

                var external = new UserExternalLogin
                {
                    Provider = provider,
                    ProviderUserId = request.ProviderUserId,
                    ProviderEmail = request.Email
                };
                await _userRepository.AddExternalLoginAsync(user.Id, external);
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            return await GenerateTokensForUser(user);
        }

        private async Task<TokenResponseDto> GenerateTokensForUser(User user)
        {
            var accessTokenExpires = DateTime.UtcNow.AddMinutes(15);
            var refreshTokenExpires = DateTime.UtcNow.AddDays(7);

            var accessToken = _tokenService.GenerateAccessToken(user, accessTokenExpires);
            var refreshToken = _tokenService.GenerateRefreshToken();
            var refreshTokenHash = _tokenService.HashRefreshToken(refreshToken);

            var userRefreshToken = new UserRefreshToken
            {
                TokenHash = refreshTokenHash,
                ExpiresAt = refreshTokenExpires,
                IpAddress = "::1", // Placeholder
                CreatedAt = DateTime.UtcNow,
                UserId = user.Id
            };

            await _userRepository.AddRefreshTokenAsync(user.Id, userRefreshToken);

            return new TokenResponseDto
            {
                AccessToken = accessToken,
                AccessTokenExpiresAt = accessTokenExpires,
                RefreshToken = refreshToken,
                RefreshTokenExpiresAt = refreshTokenExpires
            };
        }

        private static (string e164, string local) GetTrPhoneVariants(string phone)
        {
            phone = (phone ?? "").Trim();
            var digits = new string(phone.Where(char.IsDigit).ToArray());
            // +905XXXXXXXXX
            var e164Match = System.Text.RegularExpressions.Regex.Match(phone, @"^\+90(5\d{9})$");
            if (e164Match.Success)
            {
                var ten = e164Match.Groups[1].Value; // 5xxxxxxxxx
                var e164 = $"+90{ten}";
                var local = $"0{ten}";
                return (e164, local);
            }
            // 05XXXXXXXXX
            var localMatch = System.Text.RegularExpressions.Regex.Match(phone, @"^0(5\d{9})$");
            if (localMatch.Success)
            {
                var ten = localMatch.Groups[1].Value; // 5xxxxxxxxx
                var local = $"0{ten}";
                var e164 = $"+90{ten}";
                return (e164, local);
            }
            // fallback: treat input as both
            if (phone.StartsWith("+"))
            {
                return (phone, phone);
            }
            return (phone, phone);
        }
    }
}
