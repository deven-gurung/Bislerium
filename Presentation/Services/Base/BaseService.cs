using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;

namespace Presentation.Services.Base;

public class BaseService(HttpClient httpClient, ILocalStorageService localStorage)
{
    private readonly string _baseUrl = AppConfiguration.BaseUrl;

    public async Task<bool> IsUserLoggedIn()
    {
        var accessToken = await localStorage.GetItemAsync<string>("access_token");

        return accessToken != null;
    }

    public async Task LogOutUser()
    {
        await localStorage.RemoveItemAsync("access_token");
    }
    
    public async Task<T?> GetAsync<T>(string endpoint, IDictionary<string, string>? parameters = null)
    {
        var accessToken = await localStorage.GetItemAsync<string>("access_token");

        if (accessToken != null)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        var fullUrl = $"{_baseUrl}/api/{endpoint}";

        if (parameters is { Count: > 0 })
        {
            var queryString = string.Join("&", parameters.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
            
            fullUrl += "?" + queryString;
        }

        var response = await httpClient.GetAsync(fullUrl);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<T>();
        }

        throw new ApplicationException($"Error: {response.StatusCode}");
    }


    public async Task<T?> PostAsync<T>(string endpoint, StringContent stringContent)
    {
        var accessToken = await localStorage.GetItemAsync<string>("access_token");

        if (accessToken != null)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
        
        var response = await httpClient.PostAsync($"{_baseUrl}/api/{endpoint}", stringContent);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<T>();
        }

        throw new ApplicationException($"Error: {response.StatusCode}");
    }
    
    public async Task<T?> UpdateAsync<T>(string endpoint, StringContent stringContent)
    {
        var accessToken = await localStorage.GetItemAsync<string>("access_token");

        if (accessToken != null)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
        
        var response = await httpClient.PatchAsync($"{_baseUrl}/api/{endpoint}", stringContent);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<T>();
        }

        throw new ApplicationException($"Error: {response.StatusCode}");
    }

    public async Task<T?> DeleteAsync<T>(string endpoint)
    {
        var accessToken = await localStorage.GetItemAsync<string>("access_token");

        if (accessToken != null)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
        
        var response = await httpClient.DeleteAsync($"{_baseUrl}/api/{endpoint}");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<T>();
        }

        throw new ApplicationException($"Error: {response.StatusCode}");
    }
}