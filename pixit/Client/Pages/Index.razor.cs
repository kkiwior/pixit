using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using pixit.Client.Services;
using pixit.Shared.Models;

namespace pixit.Client.Pages
{
    public partial class Index : ComponentBase
    {
        //[Inject] private Mediator Mediator { get; set; }
        [Inject] protected SendEventService Event { get; set; }
        [Inject] protected NavigationManager Navigation { get; set; }
        
        private UserModel User = new UserModel();
        
        protected override async Task OnInitializedAsync()
        {
            // tu bedzie potem logika sprawdzajaca czy jest cookie i czy w grze a jak tak to bedzie powrot do gry 
        }

        private Task Submit()
        {
            //cookie
            Event.UserJoin(User);
            Navigation.NavigateTo("lobby");
            return Task.CompletedTask;
        }
    }
}