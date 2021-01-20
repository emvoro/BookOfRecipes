using System;
using System.Text.Json.Serialization;

namespace BookOfRecipes
{
    public class Ingredient
    {
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("unit")]
        public string Unit { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
