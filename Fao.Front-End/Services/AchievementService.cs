namespace Fao.Front_End.Services;

using Fao.Front_End.Helpers;
using Fao.Front_End.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

public class AchievementService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public AchievementService(HttpClient httpClient, AuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    public async Task<List<AchievementDTO?>> GetAllAchievementsAsync(bool getPhoto = false)
    {
        var achievements = new List<AchievementDTO?>();

        var uuid = await _authService.GetUUIDFromToken();
        if (uuid == null) return achievements;

        var token = await _authService.GetToken();
        if (string.IsNullOrEmpty(token))
            throw new InvalidOperationException("User is not authenticated.");

        if (!_httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await _httpClient.GetAsync($"api/Achievement/{uuid}/achievements");
        if (!response.IsSuccessStatusCode)
            return achievements;
        response.EnsureSuccessStatusCode();

        var jsonString = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(jsonString).RootElement;

        foreach (var element in json.EnumerateArray())
        {
            var a = new AchievementDTO
            {
                Id = element.SafeInt("id") ?? 0,
                Date = element.SafeStringAsDateOnly("dateAchieved"),
                Description = element.SafeString("description")!
            };

            if (element.TryGetProperty("achievementType", out var t) && t.ValueKind == JsonValueKind.Object)
            {
                a.Name = t.SafeString("name")!;
                a.Type = t.SafeString("type");

                a.Weight = t.SafeInt("weight");
                a.Hips = t.SafeInt("hips");
                a.Waist = t.TryGetProperty("waist", out var waistProp) && waistProp.ValueKind == JsonValueKind.Number ? waistProp.GetInt32() : null;
                if (getPhoto)
                {
                    a.Photo = t.SafeByteArray("photo");
                }
            }
            else
            {
                a.Type = "free-comment";
                a.Name = "Commentaire";
            }

            achievements.Add(a);
            achievements = achievements.OrderByDescending(ach => ach!.Id).ToList();
        }

        return achievements;
    }
    public async Task<bool> CreateWeightAchievementAsync(string description, int weight, DateOnly dateAchieved)
    {
        var uuid = await _authService.GetUUIDFromToken();
        if (uuid == null) return false;

        var token = await _authService.GetToken();
        if (string.IsNullOrEmpty(token))
            throw new InvalidOperationException("User is not authenticated.");

        if (!_httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        WeightAchievementDTO achievementData = new WeightAchievementDTO
        {
            DateAchieved = dateAchieved,
            Description = description,
            Weight = weight
        };

        var response = await _httpClient.PostAsJsonAsync($"/api/Achievement/{uuid}/achievements/weight", achievementData);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> CreateMeasurementAchievementAsync(string description, int hips, int waist, DateOnly dateAchieved)
    {
        var uuid = await _authService.GetUUIDFromToken();
        if (uuid == null) return false;

        var token = await _authService.GetToken();
        if (string.IsNullOrEmpty(token))
            throw new InvalidOperationException("User is not authenticated.");

        if (!_httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        MeasurementAchievementDTO achievementData = new MeasurementAchievementDTO
        {
            DateAchieved = dateAchieved,
            Description = description,
            HipsMeasurement = hips,
            WaistMeasurement = waist
        };

        var response = await _httpClient.PostAsJsonAsync($"/api/Achievement/{uuid}/achievements/measurement", achievementData);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> CreatePhotoAchievementAsync(
    string description,
    byte[] photo,
    DateOnly dateAchieved
)
    {
        var uuid = await _authService.GetUUIDFromToken();
        if (uuid == null) return false;

        var token = await _authService.GetToken();
        if (string.IsNullOrEmpty(token))
            throw new InvalidOperationException("User is not authenticated.");

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var url = $"/api/Achievement/{uuid}/achievements/photo";

        var content = new MultipartFormDataContent();

        content.Add(new StringContent(description), "Description");
        content.Add(new StringContent(dateAchieved.ToString("yyyy-MM-dd")), "DateAchieved");

        var photoContent = new ByteArrayContent(photo);
        photoContent.Headers.ContentType =
            new MediaTypeHeaderValue("image/jpeg");

        content.Add(photoContent, "Photo", "photo.jpg");

        var response = await _httpClient.PostAsync(url, content);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> CreateFreeCommentAchievementAsync(string description, DateOnly dateAchieved)
    {
        var uuid = await _authService.GetUUIDFromToken();
        if (uuid == null) return false;

        var token = await _authService.GetToken();
        if (string.IsNullOrEmpty(token))
            throw new InvalidOperationException("User is not authenticated.");

        if (!_httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        FreeCommentDTO achievementData = new FreeCommentDTO
        {
            DateAchieved = dateAchieved,
            Description = description
        };

        var response = await _httpClient.PostAsJsonAsync($"/api/Achievement/{uuid}/achievements/free-comment", achievementData);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAchievementAsync(int achievementId)
    {
        var uuid = await _authService.GetUUIDFromToken();
        if (uuid == null) return false;

        var token = await _authService.GetToken();
        if (string.IsNullOrEmpty(token))
            throw new InvalidOperationException("User is not authenticated.");

        if (!_httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await _httpClient.DeleteAsync($"/api/Achievement/{uuid}/achievements/{achievementId}");
        return response.IsSuccessStatusCode;
    }
}