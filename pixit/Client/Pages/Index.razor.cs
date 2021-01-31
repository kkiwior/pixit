using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using pixit.Shared.Models;

namespace pixit.Client.Pages
{
    public partial class Index
    {
        [Inject] private ILocalStorageService LocalStorage { get; set; }
        [Inject] private NavigationManager Navigation { get; set; }

        private Dictionary<string, string> Controls = new();
        private UserModel User = new();

        protected override async Task OnInitializedAsync()
        {  
            //lang support?

            string username = await LocalStorage.GetItemAsync<string>("username");
            if (!String.IsNullOrEmpty(username))
            {
                User.Name = username;
                await Submit();
            }
        }

        private async Task Submit()
        {
            await LocalStorage.SetItemAsync<UserModel>("user", User);
            Navigation.NavigateTo("lobby");
        }

        private void Increase(string prop) => setAvatarProperty(prop, 1);
        private void Decrease(string prop) => setAvatarProperty(prop, -1);

        private void setAvatarProperty(string prop, short val)
        {
            PropertyInfo property = User.Avatar.GetType().GetProperty(prop);
            int value = (prop == "Gender" ? val : Convert.ToInt16(property.GetValue(User.Avatar)) + val);
            property.SetValue(User.Avatar, value);
            ValidationContext vc = new ValidationContext(User.Avatar);
            Validator.TryValidateObject(User.Avatar, vc, null, true);
        }
    }
}