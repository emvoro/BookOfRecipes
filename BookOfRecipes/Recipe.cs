using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BookOfRecipes
{
    public class Recipe
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("ingredients")]
        public List<Ingredient> Ingredients { get; set; }

        [JsonPropertyName("nutritionalValue")]
        public NutritionalValue NutritionalValue { get; set; }

        public override string ToString()
        {
            string recipe = " *" + this.Name + "*\n -----------\n Ingredients";
            foreach (Ingredient ingredient in this.Ingredients)
            {
                recipe += "\n - " + ingredient.Name + " " + ingredient.Amount + " " + ingredient.Unit;
            }
            recipe += $"\n\n Proteins : {NutritionalValue.Proteins} g" +
                $"\n Fats : {NutritionalValue.Fats} g" +
                $"\n Carbohydrates : {NutritionalValue.Carbohydrates} g\n" +
                $" -----------------------";
            return recipe;
        }
    }
}
