using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace pixit.Client.Pages
{
    partial class Game
    {
        private List<Player> Players = new();
        private List<Card> CardsHand = new();

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
            //funkcja i klasy tylko do testów
            Players.Add(new Player("Lorem ipsum", 21));
            Players.Add(new Player("Lorem ipsum", 21));
            Players.Add(new Player("Lorem ipsum", 21));
            Players.Add(new Player("Lorem ipsum", 21));
            Players.Add(new Player("Lorem ipsum", 21));
            Players.Add(new Player("Lorem ipsum", 21));
            Players.Add(new Player("Lorem ipsum", 21));



            for (int i = 0; i < Players.Count; ++i)
            {
                CardsHand.Add(new Card($"images/card{i+1}.webp"));
            }
        }
        
    }

    public class Player
    {
        public string Name { get; set; }
        public int Score { get; set; }

        public Player(string n, int s)
        {
            Name = n;
            Score = s;
        }
    }

    public class Card
    {
        public string Url { get; set; }

        public Card(string url)
        {
            Url = url;
        }
    }
}