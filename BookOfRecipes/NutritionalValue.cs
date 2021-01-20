using System;
using System.Text.Json.Serialization;

namespace BookOfRecipes
{
    public class NutritionalValue
    {
        [JsonPropertyName("proteins")]
        public decimal Proteins { get; set; }

        [JsonPropertyName("fats")]
        public decimal Fats { get; set; }

        [JsonPropertyName("carbohydrates")]
        public decimal Carbohydrates { get; set; }
    }
}
