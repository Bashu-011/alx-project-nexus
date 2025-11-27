using System.Text.Json.Serialization;

namespace E_Commerce.Application.DTOs.Mpesa;

public class StkPushRequest
{
    [JsonPropertyName("BusinessShortCode")]
    public string BusinessShortCode { get; set; } = string.Empty;

    [JsonPropertyName("Password")]
    public string Password { get; set; } = string.Empty;

    [JsonPropertyName("Timestamp")]
    public string Timestamp { get; set; } = string.Empty;

    [JsonPropertyName("TransactionType")]
    public string TransactionType { get; set; } = string.Empty;

    [JsonPropertyName("Amount")]
    public string Amount { get; set; } = string.Empty;

    [JsonPropertyName("PartyA")]
    public string PartyA { get; set; } = string.Empty;

    [JsonPropertyName("PartyB")]
    public string PartyB { get; set; } = string.Empty;

    [JsonPropertyName("PhoneNumber")]
    public string PhoneNumber { get; set; } = string.Empty;

    [JsonPropertyName("CallBackURL")]
    public string CallBackURL { get; set; } = string.Empty;

    [JsonPropertyName("AccountReference")]
    public string AccountReference { get; set; } = string.Empty;

    [JsonPropertyName("TransactionDesc")]
    public string TransactionDesc { get; set; } = string.Empty;
}