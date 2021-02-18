using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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
        [Inject] private Mediator Mediator { get; set; }
        [Inject] private IJSRuntime JSRuntime { get; set; }
        [Inject] private SendEventService Event { get; set; }

        private string Clue;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("handleResize");
            }
        }
        
        protected override async Task OnInitializedAsync()
        {
            await Mediator.Register<GameModel>(HandleGameUpdate);
        }
        

        private Task HandleGameUpdate(GameModel arg)
        {
            State.Room.Game = arg;
            StateHasChanged();
            return Task.CompletedTask;
        }

        private async Task SendCard(CardModel card)
        {
            if (!State.Room.Game.Waiting.Contains(State.UserId)) return;
            await Event.SelectCard(new SelectCardEvent()
            {
                Card = card,
                Clue = (State.Room.Game.Narrator.UserId == State.UserId ? Clue : null)
            });
        }

        private async Task VoteCard(string cardId)
        {
            if (!State.Room.Game.Waiting.Contains(State.UserId)) return;
            await Event.VoteCard(cardId);
        }
    }
}