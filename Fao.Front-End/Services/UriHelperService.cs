using Microsoft.AspNetCore.Components;

namespace Fao.Front_End.Services
{
    public class UriHelperService
    {
        private readonly NavigationManager _navigationManager;

        public UriHelperService(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        public string GetCurrentUri()
        {
            return _navigationManager.Uri;
        }
        public string GetUuidFromUri()
        {
            var uri = new Uri(_navigationManager.Uri);
            var segments = uri.Segments;
            if (segments.Length > 2)
            {
                return segments[2].TrimEnd('/');
            }
            return string.Empty;
        }
    }
}