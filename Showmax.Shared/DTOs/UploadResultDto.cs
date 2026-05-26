namespace Showmax.Shared.DTOs
{
    public class UploadResultDto
    {
        public bool IsSuccess { get; set; }
        public string Url { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}