using System;

namespace PetHostelApi.Entities
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string Code { get; set; } = string.Empty;
        public Dictionary<string, object>? Parameters { get; set; }
    }

    public class ErrorResponse
    {
        public string Code { get; set; } = string.Empty;
        public Dictionary<string, object>? Parameters { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    // Respuesta simplificada solo con códigos
    public class MobileApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string Code { get; set; } = string.Empty;
        public Dictionary<string, object>? Params { get; set; }
        public long Timestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}