using Microsoft.JSInterop;

namespace CCC.Services
{
    public class CookieService
    {
        private readonly IJSRuntime _js;
        public CookieService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task<string?> GetCookie(string name)
        {
            return await _js.InvokeAsync<string>("getCookie", name);
        }

        public async Task SetCookie(string name, string value, DateTime expirationDate)
        {
            await _js.InvokeVoidAsync("setCookie", name, value, ParseDateTimeToUTCString(expirationDate));
        }

        public async Task DeleteCookie(string name)
        {
            await _js.InvokeVoidAsync("deleteCookie", name);
        }

        private string ParseDateTimeToUTCString(DateTime dateTime)
        {
            return dateTime.ToUniversalTime().ToString("R");
        }
    }
}
