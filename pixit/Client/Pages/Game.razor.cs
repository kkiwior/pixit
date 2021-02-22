using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using pixit.Client.Services;
using pixit.Client.Utils;
using pixit.Shared.Models;
using pixit.Shared.Models.Events;

namespace pixit.Client.Pages
{
    partial class Game
    {
        [Inject] private StateContainer State { get; set; }
        [Inject] private IJSRuntime JsRuntime { get; set; }
        [Inject] private SendEventService Event { get; set; }
        [Inject] private ILocalStorageService LocalStorage { get; set; }

        [Inject] private IStringLocalizer<Language> Localization { get; set; }


        private string _clue;

        protected override Task OnInitializedAsync()
        {
            if (State.Room.Name == null)
            {
                TryReconnect();
            }
            
            State.PropertyChanged += (_, _) => StateHasChanged();
            return base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JsRuntime.InvokeVoidAsync("handleResize");
            }
        }

        private async Task TryReconnect()
        {
            string roomId = await LocalStorage.GetItemAsync<string>("room");
            string token = await LocalStorage.GetItemAsync<string>("token");
            await Event.Reconnect(new ReconnectToGameEvent()
            {
                RoomId = roomId,
                Token = token
            });
        }

        private async Task SendCard(CardModel card)
        {
            if (!State.Room.Game.Waiting.Contains(State.UserId)) return;
            await Event.SelectCard(new SelectCardEvent()
            {
                Card = card,
                Clue = (State.Room.Game.Narrator.UserId == State.UserId ? _clue : null)
            });
        }

        private async Task VoteCard(string cardId)
        {
            if (!State.Room.Game.Waiting.Contains(State.UserId)) return;
            await Event.VoteCard(cardId);
        }
    }
}