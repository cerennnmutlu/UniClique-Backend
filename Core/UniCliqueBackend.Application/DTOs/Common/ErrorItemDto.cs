namespace UniCliqueBackend.Application.DTOs.Common
{
    public class ErrorItemDto
    {
        public string Field { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string Message { get; set; } = null!;
    }
}
