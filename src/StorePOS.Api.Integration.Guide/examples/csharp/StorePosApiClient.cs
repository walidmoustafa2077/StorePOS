using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;

public class StorePosApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private string? _accessToken;
    
    public StorePosApiClient(string baseUrl = "http://localhost:5062")
    {
        _httpClient = new HttpClient();
        _baseUrl = baseUrl;
    }
    
    public async Task<bool> LoginAsync(string usernameOrEmail, string password)
    {
        var loginData = new
        {
            usernameOrEmail = usernameOrEmail,
            password = password
        };
        
        var json = JsonSerializer.Serialize(loginData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync($"{_baseUrl}/api/auth/login", content);
        
        if (response.IsSuccessStatusCode)
        {
            var responseJson = await response.Content.ReadAsStringAsync();
            var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseJson);
            
            if (authResponse?.Success == true)
            {
                _accessToken = authResponse.Data?.AccessToken;
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", _accessToken);
                return true;
            }
        }
        
        return false;
    }
    
    public async Task<List<Product>?> GetProductsAsync()
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/api/products");
        
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Product>>(json);
        }
        
        return null;
    }
    
    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}

// Data Transfer Objects
public record AuthResponse(bool Success, string? Message, AuthData? Data);
public record AuthData(string AccessToken, string RefreshToken, DateTime ExpiresAt);
public record Product(int Id, string Sku, string Name, decimal Price, int StockQty);

// Usage example
var client = new StorePosApiClient();
try 
{
    if (await client.LoginAsync("admin@storepos.com", "Admin123!"))
    {
        var products = await client.GetProductsAsync();
        Console.WriteLine($"Retrieved {products?.Count} products");
    }
}
finally 
{
    client.Dispose();
}
