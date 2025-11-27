using System.Text.Json.Serialization;

namespace E_Commerce.Application.DTOs.Mpesa;

public class MpesaAccessTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;
    
    [JsonPropertyName("expires_in")]
    public string ExpiresIn { get; set; } = string.Empty;
}