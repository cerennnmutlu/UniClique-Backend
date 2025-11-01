using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UniCliqueBackend.Application.DTOs.Auth;
using UniCliqueBackend.Application.Interfaces.Repositories;
using UniCliqueBackend.Application.Interfaces.Services;
using UniCliqueBackend.Domain.Entities;
using UniCliqueBackend.Domain.Enums;

namespace UniCliqueBackend.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ITokenService _tokenService;
        private readonly IUserConsentRepository _userConsentRepository;

        public AuthService(IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository, ITokenService tokenService, IUserConsentRepository userConsentRepository)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _tokenService = tokenService;
            _userConsentRepository = userConsentRepository;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            if (await _userRepository.IsEmailTakenAsync(request.Email))
                throw new InvalidOperationException("Email zaten kullanımda.");

            if (await _userRepository.IsUsernameTakenAsync(request.Username))
                throw new InvalidOperationException("Kullanıcı adı zaten kullanımda.");

            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                Username = request.Username,
                PasswordHash = HashPassword(request.Password),
                PhoneNumber = request.PhoneNumber,
                BirthDate = request.BirthDate,
                Role = RoleType.User,
                IsActive = true,
                IsBanned = false,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            // Consents kayıtları
            var consents = new System.Collections.Generic.List<UserConsent>();
            if (request.KvkkAccepted)
                consents.Add(new UserConsent { UserId = user.Id, ConsentType = ConsentType.KVKK, IsAccepted = true, AcceptedAt = DateTime.UtcNow });
            if (request.PrivacyPolicyAccepted)
                consents.Add(new UserConsent { UserId = user.Id, ConsentType = ConsentType.PrivacyPolicy, IsAccepted = true, AcceptedAt = DateTime.UtcNow });
            if (request.TermsOfServiceAccepted)
                consents.Add(new UserConsent { UserId = user.Id, ConsentType = ConsentType.TermsOfService, IsAccepted = true, AcceptedAt = DateTime.UtcNow });

            if (consents.Count > 0)
            {
                await _userConsentRepository.AddRangeAsync(consents);
                await _userConsentRepository.SaveChangesAsync();
            }

            var expiresAt = DateTime.UtcNow.AddHours(1);
            var accessToken = _tokenService.CreateAccessToken(user, expiresAt);
            var refreshToken = await IssueRefreshTokenAsync(user.Id);

            return new AuthResponse
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = expiresAt
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByUsernameOrEmailAsync(request.UsernameOrEmail);
            if (user == null)
                throw new UnauthorizedAccessException("Geçersiz kullanıcı bilgileri.");

            // Not: Seed veride şifreler düz metin olabilir; burada SHA-256 ile karşılaştırıyoruz.
            var providedHash = HashPassword(request.Password);
            if (!string.Equals(user.PasswordHash, providedHash, StringComparison.Ordinal))
            {
                // Düz metin eşleştirme fallback (seed için)
                if (!string.Equals(user.PasswordHash, request.Password, StringComparison.Ordinal))
                    throw new UnauthorizedAccessException("Geçersiz şifre.");
            }

            if (!user.IsActive || user.IsBanned)
                throw new UnauthorizedAccessException("Hesap aktif değil ya da engellenmiş.");

            var expiresAt = DateTime.UtcNow.AddHours(1);
            var accessToken = _tokenService.CreateAccessToken(user, expiresAt);
            var refreshToken = await IssueRefreshTokenAsync(user.Id);

            return new AuthResponse
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = expiresAt
            };
        }

        public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
        {
            var existing = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
            if (existing == null || existing.IsRevoked || existing.Expiration <= DateTime.UtcNow)
                throw new UnauthorizedAccessException("Geçersiz veya süresi dolmuş refresh token.");

            var user = await _userRepository.GetByIdAsync(existing.UserId);
            if (user == null || !user.IsActive || user.IsBanned)
                throw new UnauthorizedAccessException("Kullanıcı geçersiz veya pasif.");

            // Eski refresh token'ı revoke et ve yeni ver
            await _refreshTokenRepository.RevokeAsync(existing);
            var newRefresh = await IssueRefreshTokenAsync(user.Id);

            var expiresAt = DateTime.UtcNow.AddHours(1);
            var accessToken = _tokenService.CreateAccessToken(user, expiresAt);

            await _refreshTokenRepository.SaveChangesAsync();

            return new AuthResponse
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                AccessToken = accessToken,
                RefreshToken = newRefresh.Token,
                ExpiresAt = expiresAt
            };
        }

        private async Task<RefreshToken> IssueRefreshTokenAsync(int userId)
        {
            var token = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                UserId = userId,
                Expiration = DateTime.UtcNow.AddDays(30),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            };

            await _refreshTokenRepository.AddAsync(token);
            await _refreshTokenRepository.SaveChangesAsync();
            return token;
        }

        private static string HashPassword(string password)
        {
            // PBKDF2-HMACSHA256, output 128 bytes -> 256 hex chars
            var iterations = 100_000;
            var outputLengthBytes = 128;
            // Not: Şu an salt kullanmıyoruz, 256 karakter sabit uzunluk gereksinimi için çıktı uzunluğu 256 hex.
            using var pbkdf2 = new Rfc2898DeriveBytes(password, Array.Empty<byte>(), iterations, HashAlgorithmName.SHA256);
            var bytes = pbkdf2.GetBytes(outputLengthBytes);
            return Convert.ToHexString(bytes);
        }
    }
}


