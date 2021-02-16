using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using pixit.Client.Utils;
using pixit.Shared.Models;

namespace pixit.Client.Pages
{
    partial class Game
    {
        [Inject] private StateContainer State { get; set; }
        [Inject] private Mediator Mediator { get; set; }
        [Inject] private IJSRuntime JSRuntime { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                JSRuntime.InvokeVoidAsync("handleResize");
            }
        }
        
        protected override async Task OnInitializedAsync()
        {
            JSRuntime.InvokeVoidAsync("console.log", "nie karty");

            await Mediator.Register<GameModel>(HandleGameUpdate);
        }
        

        private Task HandleGameUpdate(GameModel arg)
        {
            State.Game = arg;
            StateHasChanged();
            return Task.CompletedTask;
        }
    }
}