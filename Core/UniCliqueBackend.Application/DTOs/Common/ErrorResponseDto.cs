namespace UniCliqueBackend.Application.DTOs.Common
{
    public class ErrorResponseDto
    {
        public bool Success { get; set; } = false;
        public string Code { get; set; } = "";
        public string Message { get; set; } = "";
        public List<ErrorItemDto>? Errors { get; set; }
        public string? TraceId { get; set; }
    }
}
