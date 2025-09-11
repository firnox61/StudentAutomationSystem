namespace StudentAutomation.Blazor.Models.Auth
{
    public class AccessTokenDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; } // UTC bekliyoruz (backend öyle döndürüyor)
    }
}
