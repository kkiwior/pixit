using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
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

        private string _clue;

        protected override Task OnInitializedAsync()
        {
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