namespace Fao.Front_End.Services;

using System.Net.Http.Headers;
using System.Net.Http.Json;
using Fao.Front_End.Models;
using Microsoft.AspNetCore.Components;

public class UserServices
{
    #region Fields
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    private readonly NavigationManager _nav;
    #endregion

    #region Constructor
    public UserServices(HttpClient httpClient, AuthService authService, NavigationManager nav)
    {
        _httpClient = httpClient;
        _authService = authService;
        _nav = nav;
    }
    #endregion

    #region Current User Methods

    /// <summary>
    /// Get information about the currently authenticated user.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<UserDTO> GetUserInfoAsync()
    {
        var token = await _authService.GetToken();

        var uuid = await _authService.GetUUIDFromToken();

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Users/{uuid}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var userInfo = await response.Content.ReadFromJsonAsync<UserDTO>();
        if (userInfo == null)
        {
            throw new InvalidOperationException("Failed to retrieve user information.");
        }

        return userInfo;
    }


    /// <summary>
    /// Get a list of customers associated with the currently authenticated user.
    /// </summary>
    /// <returns></returns>
    public async Task<List<UserDTO?>> GetCustomersAsync()
    {
        var customersUuids = await GetCustomersUuidsAsync();
        var customers = new List<UserDTO?>();

        foreach (var customerUuid in customersUuids)
        {
            var customerInfo = await GetUserByUuidAsync(customerUuid.Uuid);
            if (customerInfo != null)
            {
                customers.Add(customerInfo);
            }
        }

        return customers;
    }

    /// <summary>
    /// Get a list of UUIDs for customers associated with the currently authenticated coach.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<List<UuidDTO>> GetCustomersUuidsAsync()
    {
        var token = await _authService.GetToken();

        var uuid = await _authService.GetUUIDFromToken();

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Users/{uuid}/customers");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var customersUuids = await response.Content.ReadFromJsonAsync<List<UuidDTO>>();
        if (customersUuids == null)
        {
            throw new InvalidOperationException("Failed to retrieve customers UUIDs.");
        }

        return customersUuids;
    }
    #endregion

    #region User Management Methods

    /// <summary>
    /// Get user information by UUID.
    /// </summary>
    /// <param name="uuid"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<UserDTO?> GetUserByUuidAsync(string uuid)
    {
        var token = await _authService.GetToken();

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Users/{uuid}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
            return null;
        response.EnsureSuccessStatusCode();

        var userInfo = await response.Content.ReadFromJsonAsync<UserDTO>();
        if (userInfo == null)
        {
            throw new InvalidOperationException("Failed to retrieve user information.");
        }

        return userInfo;
    }

    /// <summary>
    /// Get the UUID of the coach associated with the currently authenticated user.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<UuidDTO?> GetCoachUuidAsync()
    {
        var token = await _authService.GetToken();

        var uuid = await _authService.GetUUIDFromToken();

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Users/{uuid}/coach-user");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var coachUuid = await response.Content.ReadFromJsonAsync<UuidDTO>();
        if (coachUuid == null)
        {
            throw new InvalidOperationException("Failed to retrieve coach UUID.");
        }

        return coachUuid;
    }

    /// <summary>
    /// Create a new customer user.
    /// </summary>
    /// <param name="customer"></param>
    /// <returns></returns>
    public async Task<UserDTO> CreateCustomerAsync(RegisterDTO customer)
    {
        var token = await _authService.GetToken();


        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/Users/register");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(customer);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var newUser = await response.Content.ReadFromJsonAsync<UserDTO>();
        if (newUser == null)
        {
            throw new InvalidOperationException("Failed to create customer.");
        }
        return newUser;
    }

    /// <summary>
    /// Delete a user by UUID.
    /// </summary>
    /// <param name="uuid"></param>
    /// <returns></returns>
    public async Task DeleteUserAsync(string uuid)
    {
        var token = await _authService.GetToken();

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/Users/{uuid}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    #endregion

    #region User Profile Methods

    /// <summary>
    /// Get the user profile by UUID.
    /// </summary>
    /// <param name="uuid"></param>
    /// <returns></returns>
    public async Task<UserProfileDTO?> GetUserProfileAsync(string uuid)
    {
        var token = await _authService.GetToken();

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Users/{uuid}/profile");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            return null;

        var userProfile = await response.Content.ReadFromJsonAsync<UserProfileDTO>();
        return userProfile;
    }

    /// <summary>
    /// Set the user profile for a given user UUID.
    /// </summary>
    /// <param name="UserUuid"></param>
    /// <param name="profile"></param>
    /// <returns></returns>
    public async Task SetUserProfileAsync(string UserUuid, UserProfileDTO profile)
    {
        var token = await _authService.GetToken();

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/Users/set-profile");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(new
        {
            uuid = UserUuid,
            size = profile.Size,
            weight = profile.Weight,
            physicalActivity = profile.PhysicalActivity,
            job = profile.Job,
            energyRequirement = profile.EnergyRequirement
        });

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Update the user profile for a given user UUID.
    /// </summary>
    /// <param name="UserUuid"></param>
    /// <param name="profile"></param>
    /// <returns></returns>
    public async Task UpdateUserProfileAsync(string UserUuid, UserProfileDTO profile)
    {
        var token = await _authService.GetToken();

        var request = new HttpRequestMessage(HttpMethod.Patch, $"/api/Users/profile/{UserUuid}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(new
        {
            uuid = UserUuid,
            size = profile.Size,
            weight = profile.Weight,
            physicalActivity = profile.PhysicalActivity,
            job = profile.Job,
            energyRequirement = profile.EnergyRequirement
        });

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
    #endregion

    #region User Helper Methods
    /// <summary>
    /// Route guard to protect pages based on user role and UUID.
    /// 
    /// This method checks the user's role and UUID from the token and redirects them to the appropriate page if they are not authorized to access the current page.
    /// 
    /// For example, if a user with the role "ROLE_USER" tries to access a page that requires a different UUID, they will be redirected to the login page. If a user with the role "ROLE_COACH" tries to access a page that requires a UUID that is not associated with their customers, they will be redirected to the coach dashboard.
    /// </summary>
    /// <param name="uuid"></param>
    /// <returns></returns>
    public async Task<string> RouteGuardAsync(string uuid, string redirectUri = "")
    {
        var tokenRole = await _authService.GetRoleFromToken();
        var tokenUuid = await _authService.GetUUIDFromToken();
        if (string.IsNullOrWhiteSpace(redirectUri))
        {
            redirectUri = _nav.Uri;
        }

        if (string.IsNullOrWhiteSpace(tokenRole) || string.IsNullOrWhiteSpace(tokenUuid))
        {
            _nav.NavigateTo("/login", forceLoad: false);
            return string.Empty;
        }

        switch (tokenRole)
        {
            case "ROLE_USER":
                {
                    var targetUuid = tokenUuid;
                    var targetUri = redirectUri;
                    Console.Write(targetUri);

                    if (!string.IsNullOrWhiteSpace(uuid) && uuid != tokenUuid)
                        _nav.NavigateTo($"/planning/{tokenUuid}", forceLoad: true);

                    return tokenUuid;
                }

            case "ROLE_COACH":
                {
                    if (string.IsNullOrWhiteSpace(uuid) || uuid == tokenUuid)
                    {
                        _nav.NavigateTo("/coach-dashboard");
                        return string.Empty;
                    }

                    var usersUuid = await GetCustomersUuidsAsync();
                    if (usersUuid == null || !usersUuid.Any(u => u.Uuid == uuid))
                    {
                        _nav.NavigateTo("/coach-dashboard");
                        return string.Empty;
                    }

                    return uuid;
                }

            default:
                _nav.NavigateTo("/login");
                return string.Empty;
        }
    }
    #endregion
}