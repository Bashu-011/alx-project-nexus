using System.Text.Json.Serialization;

namespace E_Commerce.Application.DTOs.Mpesa;

public class StkPushResponse
{
    [JsonPropertyName("MerchantRequestID")]
    public string MerchantRequestID { get; set; } = string.Empty;

    [JsonPropertyName("CheckoutRequestID")]
    public string CheckoutRequestID { get; set; } = string.Empty;

    [JsonPropertyName("ResponseCode")]
    public string ResponseCode { get; set; } = string.Empty;

    [JsonPropertyName("ResponseDescription")]
    public string ResponseDescription { get; set; } = string.Empty;

    [JsonPropertyName("CustomerMessage")]
    public string CustomerMessage { get; set; } = string.Empty;
}