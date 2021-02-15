using System;
using System.Reflection;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using pixit.Client.Utils;
using pixit.Shared.Models;

namespace pixit.Client.Pages
{
    public partial class Index
    {
        [Inject] private ILocalStorageService LocalStorage { get; set; }
        [Inject] private NavigationManager Navigation { get; set; }
        [Inject] private StateContainer State { get; set; }

        private UserModel _user = new();

        protected override async Task OnInitializedAsync()
        {
            _user = await LocalStorage.GetItemAsync<UserModel>("user") ?? new UserModel();
        }

        private async Task Submit()
        {
            await LocalStorage.SetItemAsync("user", _user);
            if (State.JoinRoomAfterLogin != null)
            {
                Navigation.NavigateTo("room/" + State.JoinRoomAfterLogin);
                State.JoinRoomAfterLogin = null;
                return;
            }
            Navigation.NavigateTo("lobby");
        }

        private void Increase(string prop) => SetAvatarProperty(prop, 1);
        private void Decrease(string prop) => SetAvatarProperty(prop, -1);

        private void SetAvatarProperty(string prop, short val)
        {
            PropertyInfo property = _user.Avatar.GetType().GetProperty(prop);
            int value = (prop == "Gender" ? val : Convert.ToInt16(property?.GetValue(_user.Avatar)) + val);
            property?.SetValue(_user.Avatar, value);
            _user.Validate();
        }
    }
}