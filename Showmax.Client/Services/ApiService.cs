using Showmax.Shared.DTOs;
using System.Net.Http.Json;

namespace Showmax.Client.Services
{
    public class ApiService
    {
        private readonly HttpClient _http;

        public ApiService(HttpClient http)
        {
            _http = http;
        }

        // Auth
        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/auth/register", dto);
            return await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", dto);
            return await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        }

        // Content
        public async Task<List<ContentDto>?> GetAllContentAsync()
        {
            return await _http.GetFromJsonAsync<List<ContentDto>>("api/content");
        }

        public async Task<List<ContentDto>?> GetContentByTypeAsync(string type)
        {
            return await _http.GetFromJsonAsync<List<ContentDto>>($"api/content/type/{type}");
        }

        public async Task<List<ContentDto>?> SearchContentAsync(string query)
        {
            return await _http.GetFromJsonAsync<List<ContentDto>>($"api/content/search?query={query}");
        }

        public async Task<ContentDto?> GetContentByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<ContentDto>($"api/content/{id}");
        }

        public async Task<List<ContentDto>?> GetContentByGenreAsync(string genre)
        {
            return await _http.GetFromJsonAsync<List<ContentDto>>($"api/content/genre/{genre}");
        }

        // Payments
        public async Task<List<SubscriptionPlanDto>?> GetPlansAsync()
        {
            return await _http.GetFromJsonAsync<List<SubscriptionPlanDto>>("api/payments/plans");
        }

        public async Task<CheckoutSessionResponseDto?> CreateCheckoutSessionAsync(CreateCheckoutSessionDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/payments/create-checkout-session", dto);
            return await response.Content.ReadFromJsonAsync<CheckoutSessionResponseDto>();
        }

        public async Task<UserSubscriptionDto?> GetMySubscriptionAsync(string token)
        {
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return await _http.GetFromJsonAsync<UserSubscriptionDto>("api/payments/my-subscription");
        }

        // Upload
        public async Task<UploadResultDto?> UploadThumbnailAsync(MultipartFormDataContent content, string token)
        {
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _http.PostAsync("api/upload/thumbnail", content);
            return await response.Content.ReadFromJsonAsync<UploadResultDto>();
        }

        public async Task<UploadResultDto?> UploadVideoAsync(MultipartFormDataContent content, string token)
        {
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _http.PostAsync("api/upload/video", content);
            return await response.Content.ReadFromJsonAsync<UploadResultDto>();
        }

        public async Task<object?> UpdateContentThumbnailAsync(int contentId, MultipartFormDataContent content, string token)
        {
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _http.PostAsync($"api/upload/update-thumbnail/{contentId}", content);
            return await response.Content.ReadFromJsonAsync<object>();
        }

        public async Task<object?> UpdateContentVideoAsync(int contentId, MultipartFormDataContent content, string token)
        {
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _http.PostAsync($"api/upload/update-video/{contentId}", content);
            return await response.Content.ReadFromJsonAsync<object>();
        }

        // Admin
        public async Task<List<ContentDto>?> GetAdminContentAsync(string token)
        {
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return await _http.GetFromJsonAsync<List<ContentDto>>("api/admin/content");
        }

        public async Task<HttpResponseMessage> AddContentAsync(MultipartFormDataContent content, string token)
        {
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return await _http.PostAsync("api/admin/content", content);
        }

        public async Task<HttpResponseMessage> UpdateContentAsync(int id, MultipartFormDataContent content, string token)
        {
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return await _http.PutAsync($"api/admin/content/{id}", content);
        }

        public async Task<HttpResponseMessage> DeleteContentAsync(int id, string token)
        {
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return await _http.DeleteAsync($"api/admin/content/{id}");
        }

        public async Task<object?> GetAdminStatsAsync(string token)
        {
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return await _http.GetFromJsonAsync<object>("api/admin/stats");
        }

        // Watchlist
        public async Task<List<WatchlistDto>?> GetWatchlistAsync(string token)
        {
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return await _http.GetFromJsonAsync<List<WatchlistDto>>("api/watchlist");
        }

        public async Task<bool> AddToWatchlistAsync(int contentId, string token)
        {
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _http.PostAsync($"api/watchlist/{contentId}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveFromWatchlistAsync(int contentId, string token)
        {
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _http.DeleteAsync($"api/watchlist/{contentId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CheckWatchlistAsync(int contentId, string token)
        {
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var result = await _http.GetFromJsonAsync<WatchlistCheckDto>($"api/watchlist/check/{contentId}");
            return result?.IsInWatchlist ?? false;
        }

        private class WatchlistCheckDto
        {
            public bool IsInWatchlist { get; set; }
        }

        // Profile
        public async Task<UserProfileDto?> GetProfileAsync(string token)
        {
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return await _http.GetFromJsonAsync<UserProfileDto>("api/profile");
        }

        public async Task<bool> UpdateProfileAsync(UpdateProfileDto dto, string token)
        {
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _http.PutAsJsonAsync("api/profile", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordDto dto, string token)
        {
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _http.PostAsJsonAsync("api/profile/change-password", dto);
            return response.IsSuccessStatusCode;
        }
    }
}