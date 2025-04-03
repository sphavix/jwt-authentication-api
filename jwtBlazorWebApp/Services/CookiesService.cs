using Microsoft.JSInterop;

namespace jwtBlazorWebApp.Services
{
    public class CookiesService(IJSRuntime runtime)
    {
        private readonly IJSRuntime _runtime = runtime;

        public async Task<string> GetCookie(string key)
        {
            return await _runtime.InvokeAsync<string>("getCookie", key);
        }

        public async Task RemoveCookie(string key)
        {
            await _runtime.InvokeVoidAsync("deleteCookie", key);
        }

        public async Task SetCookie(string key, string value, int days)
        {
            await _runtime.InvokeVoidAsync("setCookie", key, value, days);
        }
    }
}
