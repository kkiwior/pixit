using Newtonsoft.Json;

namespace pixit.Shared.Models.Others
{
    public class NarratorModel
    {
        public int Index { get; set; }
        public string UserId { get; set; }
        [JsonIgnore]
        public string Token { get; set; }

        public NarratorModel()
        {
            Index = -1;
        }
    }
}