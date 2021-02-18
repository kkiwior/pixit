using System.Collections.Generic;
using Newtonsoft.Json;

namespace pixit.Shared.Models.Others
{
    public class CardOnTableModel
    {
        public string Id { get; set; }
        public CardModel Card { get; set; }
        [JsonIgnore]
        public string UserId { get; set; }
        [JsonIgnore]
        public bool IsNarratorCard { get; set; }

        public List<string> Votes { get; set; }

        public CardOnTableModel()
        {
            Votes = new();
        }
    }
}