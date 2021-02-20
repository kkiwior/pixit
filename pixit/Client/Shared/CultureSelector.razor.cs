using System.Collections.Generic;
using System.Globalization;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;

namespace pixit.Client.Shared
{
    public partial class CultureSelector
    {
        [Inject] private ILocalStorageService LocalStorage { get; set; }
        [Inject] private NavigationManager Navigation { get; set; }
        
        private List<string> _supportedCultures = new()
        {
            "pl-PL", "en-US"
        };

        private void SetCulture(string culture)
        {
            if (CultureInfo.CurrentCulture.Name != culture)
            {
                LocalStorage.SetItemAsync("blazorCulture", culture);
                Navigation.NavigateTo(Navigation.Uri, forceLoad: true);
            }
        }
    }
}