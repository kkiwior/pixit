using Microsoft.AspNetCore.Components;

namespace pixit.Client.Shared
{
    public partial class Modal : ComponentBase
    {
        [Parameter] public RenderFragment Title { get; set; }
        [Parameter] public RenderFragment Body { get; set; }
        [Parameter] public RenderFragment Footer { get; set; }

        private bool display = false;

        public void Toggle()
        {
            display = !display;
        }
    }
}