using E_Commerce.Application.DTOs.Mpesa;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace E_Commerce.Application.Services;

//service to handle M-Pesa STK Push operations
public class MpesaService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<MpesaService> _logger;
    private readonly string _consumerKey;
    private readonly string _consumerSecret;
    private readonly string _passKey;
    private readonly string _shortCode;
    private readonly string _callbackUrl;
    private readonly string _baseUrl;


    //function to initialize MpesaService with necessary configurations
    public MpesaService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<MpesaService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;

        var mpesaSettings = configuration.GetSection("MpesaSettings");
        _consumerKey = mpesaSettings["ConsumerKey"]!;
        _consumerSecret = mpesaSettings["ConsumerSecret"]!;
        _passKey = mpesaSettings["PassKey"]!;
        _shortCode = mpesaSettings["ShortCode"]!;
        _callbackUrl = mpesaSettings["CallbackUrl"]!;

        var environment = mpesaSettings["Environment"];
        _baseUrl = environment == "production"
            ? "https://api.safaricom.co.ke"
            : "https://sandbox.safaricom.co.ke";
    }

    //get M-Pesa access token
    public async Task<string> GetAccessTokenAsync()
    {
        try
        {
            //basic auth credentials
            var credentials = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{_consumerKey}:{_consumerSecret}"));

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"{_baseUrl}/oauth/v1/generate?grant_type=client_credentials");

            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to get M-Pesa access token: {Content}", content);
                throw new Exception($"Failed to get access token: {content}");
            }

            var tokenResponse = JsonSerializer.Deserialize<MpesaAccessTokenResponse>(content);
            return tokenResponse?.AccessToken ?? throw new Exception("Access token is null");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting M-Pesa access token");
            throw;
        }
    }

    //initiaate stk push
    public async Task<StkPushResponse> InitiateStkPushAsync(
        string phoneNumber,
        decimal amount,
        string accountReference,
        string transactionDesc)
    {
        try
        {
            //get access token
            var accessToken = await GetAccessTokenAsync();

            //generate timestamp
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");

            //generate password
            var password = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{_shortCode}{_passKey}{timestamp}"));

            //format phone number
            phoneNumber = FormatPhoneNumber(phoneNumber);

            //create stk push
            var stkRequest = new StkPushRequest
            {
                BusinessShortCode = _shortCode,
                Password = password,
                Timestamp = timestamp,
                TransactionType = "CustomerPayBillOnline",
                Amount = Math.Round(amount).ToString(),  //no decimals
                PartyA = phoneNumber,
                PartyB = _shortCode,
                PhoneNumber = phoneNumber,
                CallBackURL = _callbackUrl,
                AccountReference = accountReference,
                TransactionDesc = transactionDesc
            };

            //send request to mpesa
            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"{_baseUrl}/mpesa/stkpush/v1/processrequest");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = new StringContent(
                JsonSerializer.Serialize(stkRequest),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("M-Pesa STK Push response: {Content}", content);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("M-Pesa STK Push failed: {Content}", content);
                throw new Exception($"STK Push failed: {content}");
            }

            var stkResponse = JsonSerializer.Deserialize<StkPushResponse>(content);
            return stkResponse ?? throw new Exception("STK Push response is null");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initiating STK Push");
            throw;
        }
    }

    //helper function to format phone number
    private string FormatPhoneNumber(string phoneNumber)
    {
        
        phoneNumber = phoneNumber.Replace(" ", "")
                                 .Replace("-", "")
                                 .Replace("(", "")
                                 .Replace(")", "");

        //remove any leading '+' or '0'
        if (phoneNumber.StartsWith("+"))
            phoneNumber = phoneNumber.Substring(1);

        if (phoneNumber.StartsWith("0"))
            phoneNumber = phoneNumber.Substring(1);

        //kenya code if not present
        if (!phoneNumber.StartsWith("254"))
            phoneNumber = "254" + phoneNumber;

        return phoneNumber;
    }
}