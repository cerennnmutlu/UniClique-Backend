using System;
using UniCliqueBackend.Domain.Enums;

namespace UniCliqueBackend.Application.DTOs.Business
{
    public class BusinessRequestDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string BusinessName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public BusinessRequestStatus Status { get; set; }
        public string? AdminResponse { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
