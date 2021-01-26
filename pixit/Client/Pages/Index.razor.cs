using System;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using pixit.Shared.Models;

namespace pixit.Client.Pages
{
    public partial class Index
    {
        [Inject] private ILocalStorageService LocalStorage { get; set; }
        [Inject] private NavigationManager Navigation { get; set; }
        
        private UserModel User = new();
        
        protected override async Task OnInitializedAsync()
        {
            string username = await LocalStorage.GetItemAsync<string>("username");
            if (!String.IsNullOrEmpty(username))
            {
                User.Name = username;
                await Submit();
            }
        }

        private async Task Submit()
        {
            await LocalStorage.SetItemAsync("username", User.Name);
            Navigation.NavigateTo("lobby");
        }
    }
}