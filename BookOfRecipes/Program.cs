using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.IO;
using System.Globalization;

namespace BookOfRecipes
{
    class Program
    {
        static NumberFormatInfo Nfi = new CultureInfo("en-US", false).NumberFormat;
        static List<Recipe> Recipes = new List<Recipe>();

        static void Main(string[] args)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            if (File.Exists("recipes.json"))
            {
                try
                {
                    Recipes = JsonSerializer.Deserialize<List<Recipe>>(File.ReadAllText("recipes.json"));
                }
                catch (JsonException)
                {
                    PrintInColor(ConsoleColor.Red, "Sorry, Your recipes file is corrupted.");
                    return;
                }
            }

            PrintInColor(ConsoleColor.Green, "\n Welcome to your Book of Recipes");
            int command = 0;

            while (command != 3)
            {
                PrintInColor(ConsoleColor.Cyan, "\n MAIN MENU");
                Console.WriteLine(" You can perform following actions :" +
                    "\n 1 - Add recipe" +
                    "\n 2 - Search recipe " +
                    "\n 3 - Exit");
                command = CheckInteger("Enter command number", 3);

                switch (command)
                {
                    case 1:
                        Recipe recipe = AddRecipe();
                        Recipes.Add(recipe);
                        break;
                    case 2:
                        if (Recipes == null || Recipes.Count == 0)
                            PrintInColor(ConsoleColor.Red, " You don't have any recipes");
                        else
                        {
                            bool isSearched = true;
                            Console.WriteLine(
                                "\n 1 - Search by recipe name" +
                                "\n 2 - Search by ingredient" +
                                "\n 3 - Go back to main menu");
                            List<Recipe> searchedRecipes = new List<Recipe>();
                            int searchCommand = CheckInteger("Choose search criteria", 3);

                            switch (searchCommand)
                            {
                                case 1:
                                    PrintInColor(ConsoleColor.Cyan, "Enter recipe name");
                                    string nameToSearch = Console.ReadLine().Trim();
                                    if (string.IsNullOrEmpty(nameToSearch))
                                        searchedRecipes = Recipes;
                                    else
                                        searchedRecipes = Recipes.Where(x => x.Name.Contains(nameToSearch)).ToList();
                                    break;
                                case 2:
                                    PrintInColor(ConsoleColor.Cyan, "Enter ingredient to search");
                                    string ingredientToSearch = Console.ReadLine().Trim();
                                    if (string.IsNullOrEmpty(ingredientToSearch))
                                        searchedRecipes = Recipes;
                                    else
                                        searchedRecipes = Recipes.Where(x => x.Ingredients
                                        .Any(x => x.Name.Contains(ingredientToSearch))).ToList();
                                    break;
                                case 3:
                                    isSearched = false;
                                    break;
                            }
                            if (isSearched && searchedRecipes.Count < 1)
                                PrintInColor(ConsoleColor.Red, " No such recipe found.");
                            else if (isSearched)
                            {
                                int recipeNumber = 0;

                                if (searchedRecipes.Count == 1)
                                    ViewRecipe(0, searchedRecipes);
                                else if (searchedRecipes.Count > 1)
                                {
                                    Console.WriteLine("\n How do you prefer to view results?\n" +
                                        " 1 - Sorted by name ascending\n" +
                                        " 2 - Sorted by name descending\n" +
                                        " 3 - Sorted by nutritional value ascending\n" +
                                        " 4 - Sorted by nutritional value descending\n" +
                                        " 5 - Not sorted");
                                    int sortCommand = CheckInteger("Enter command number", 5);

                                    switch (sortCommand)
                                    {
                                        case 1:
                                            searchedRecipes = searchedRecipes.OrderBy(x => x.Name).ToList();
                                            break;
                                        case 2:
                                            searchedRecipes = searchedRecipes.OrderByDescending(x => x.Name).ToList();
                                            break;
                                        case 3:
                                            searchedRecipes = OrderByNutritionalValue("ASC", searchedRecipes);
                                            break;
                                        case 4:
                                            searchedRecipes = OrderByNutritionalValue("DESC", searchedRecipes);
                                            break;
                                        case 5:
                                            break;
                                    }
                                    PrintRecipes(searchedRecipes);
                                    Console.WriteLine(
                                        "\n 1 - View recipe" +
                                        "\n 2 - Filter recipes" +
                                        "\n 3 - Go back to menu");
                                    int listCommand = CheckInteger("Enter command", 3);
                                    switch (listCommand)
                                    {
                                        case 1:
                                            recipeNumber = CheckInteger("Enter number of recipe to view",
                                                searchedRecipes.Count) - 1;
                                            ViewRecipe(recipeNumber, searchedRecipes);
                                            break;
                                        case 2:
                                            List<Recipe> filteredRecipes = FilterByNutritionalValue(searchedRecipes);
                                            PrintRecipes(filteredRecipes);
                                            if (filteredRecipes.Count > 0)
                                            {
                                                Console.WriteLine(
                                                    "\n 1 - View recipe" +
                                                    "\n 2 - Go back to main menu" +
                                                    "\n 3 - Exit");
                                                int filterCommand = CheckInteger("Enter command", 3);
                                                switch (filterCommand)
                                                {
                                                    case 1:
                                                        recipeNumber = CheckInteger("Enter number of recipe to view",
                                                            filteredRecipes.Count) - 1;
                                                        ViewRecipe(recipeNumber, filteredRecipes);
                                                        break;
                                                    case 2:
                                                        break;
                                                    case 3:
                                                        return;
                                                }
                                            }
                                            break;
                                        case 3:
                                            break;
                                    }
                                }
                                else
                                    PrintInColor(ConsoleColor.Red, "No such recipe.");
                            }
                        }
                        break;
                    case 3:
                        return;
                }
                var json = JsonSerializer.Serialize(Recipes, options);
                File.WriteAllText("recipes.json", json);
            }
        }

        static void ViewRecipe(int number, List<Recipe> recipes)
        {
            Console.WriteLine("\n " + recipes[number].ToString());
            //PrintRecipes(recipes);
            Console.WriteLine(
                "\n 1 - Edit recipe" +
                "\n 2 - Delete recipe" +
                "\n 3 - Go back to main menu"
                );
            int command = CheckInteger("Enter command", 3);
            switch (command)
            {
                case 1:
                    recipes[number] = EditRecipe(recipes[number]);
                    break;
                case 2:
                    DeleteRecipe(recipes[number].Name, recipes);
                    break;
                case 3:
                    break;
            }
        }

        static Recipe AddRecipe()
        {
            string recipeName = InputString("Enter recipe name");
            while (Recipes.Any(x => x.Name == recipeName))
            {
                PrintInColor(ConsoleColor.Red, "Recipe with this name already exists");
                recipeName = InputString($"Enter another recipe name");
            }
            int numberOfIngredients = CheckInteger("Enter number of ingredients", 50);
            var ingredients = AddIngredients(numberOfIngredients);
            var nutritionalValue = AddNutritionalValue();
            var recipe = new Recipe { Name = recipeName, Ingredients = ingredients, NutritionalValue = nutritionalValue };
            PrintInColor(ConsoleColor.Green, $" Recipe {recipe.Name} created.");
            return recipe;
        }

        static Recipe EditRecipe(Recipe recipe)
        {
            Console.WriteLine(
                "\n 1 - Edit name" +
                "\n 2 - Edit ingredients" +
                "\n 3 - Edit nutritional value");
            int command = CheckInteger("Enter command", 3);
            switch (command)
            {
                case 1:
                    recipe.Name = InputString("Enter new name:");
                    break;
                case 2:
                    int numberOfIngredients = CheckInteger("Enter number of ingredients", 100);
                    recipe.Ingredients = AddIngredients(numberOfIngredients);
                    break;
                case 3:
                    recipe.NutritionalValue = AddNutritionalValue();
                    break;
            }
            PrintInColor(ConsoleColor.Green, "Recipe edited.");
            return recipe;
        }

        static void DeleteRecipe(string name, List<Recipe> recipes)
        {
            string deleteConfirmation = InputString($"WARNING!" +
                $" Are you sure you want to delete recipe {name}? Y/N").ToLower();
            while (deleteConfirmation != "y" && deleteConfirmation != "n")
                deleteConfirmation = InputString($"Type Y or N.").ToLower();
            if (deleteConfirmation == "y")
            {
                recipes.Remove(recipes.Where(x => x.Name == name).First());
                PrintInColor(ConsoleColor.Green, $"Recipe {name} deleted");
            }
            else
            {
                PrintInColor(ConsoleColor.Green, $"Recipe {name} was not deleted");
            }
        }

        static void PrintRecipes(List<Recipe> recipes)
        {
            PrintInColor(ConsoleColor.Cyan, "SEARCH RESULTS");
            int number = 1;
            if (recipes.Count > 0)
            {
                while (number <= recipes.Count)
                {
                    Console.WriteLine("\n " + number + recipes[number - 1].ToString());
                    number++;
                }
            }
            else
                PrintInColor(ConsoleColor.Red, "No such recipe.");
        }

        static List<Ingredient> AddIngredients(int number)
        {
            var ingredients = new List<Ingredient>();
            for (int i = 0; i < number; i++)
            {
                string ingredientName = InputString($"Enter ingredient {i + 1} name");
                while (ingredients.Any(x => x.Name == ingredientName))
                {
                    PrintInColor(ConsoleColor.Red, "You have listed this ingredient already");
                    ingredientName = InputString($"Enter ingredient {i + 1} name");
                }

                decimal amount = CheckDecimal($"Enter amount of ingredient {ingredientName}");
                Console.WriteLine("\n Units:" +
                    "\n 1 - g" +
                    "\n 2 - kg" +
                    "\n 3 - ml" +
                    "\n 4 - l" +
                    "\n 5 - glass" +
                    "\n 6 - pc" +
                    "\n 7 - tsp" +
                    "\n 8 - tbsp");

                string unit = Enum.GetName(typeof(Unit),
                    CheckInteger($"Enter unit of ingredient {ingredientName}", 8));

                var ingredient = new Ingredient { Amount = amount, Unit = unit, Name = ingredientName };
                ingredients.Add(ingredient);
            }
            return ingredients;
        }

        static NutritionalValue AddNutritionalValue()
        {
            decimal proteins = CheckDecimal("Enter nutritional value\n Enter proteins");
            decimal fats = CheckDecimal("Enter fats");
            decimal carbohydrates = CheckDecimal("Enter carbohydrates");
            return new NutritionalValue { Proteins = proteins, Fats = fats, Carbohydrates = carbohydrates };
        }

        static List<Recipe> OrderByNutritionalValue(string mode, List<Recipe> recipes)
        {
            Console.WriteLine(
                " 1 - Proteins" +
                "\n 2 - Fats" +
                "\n 3 - Carbohydrates");
            int sortByValueType = CheckInteger("Enter type of nutrition value to sort by\n", 3);
            return sortByValueType switch
            {
                1 => mode == "ASC" ? recipes.OrderBy(x => x.NutritionalValue.Proteins).ToList()
                    : recipes.OrderByDescending(x => x.NutritionalValue.Proteins).ToList(),
                2 => mode == "ASC" ? recipes.OrderBy(x => x.NutritionalValue.Fats).ToList()
                    : recipes.OrderByDescending(x => x.NutritionalValue.Fats).ToList(),
                3 => mode == "ASC" ? recipes.OrderBy(x => x.NutritionalValue.Carbohydrates).ToList()
                    : recipes.OrderByDescending(x => x.NutritionalValue.Carbohydrates).ToList(),
                _ => recipes,
            };
        }

        static List<Recipe> FilterByNutritionalValue(List<Recipe> recipes)
        {
            Console.WriteLine(
                " Filter by" +
                "\n 1 - Proteins" +
                "\n 2 - Fats" +
                "\n 3 - Carbohybrates");
            int command = CheckInteger("Enter filter", 3);
            decimal minimum = 0;
            decimal maximum = 0;
            switch (command)
            {
                case 1:
                    minimum = CheckDecimal("Enter minimum of proteins");
                    maximum = CheckDecimal("Enter maximum of proteins");
                    recipes = recipes.Where(x => x.NutritionalValue.Proteins >= minimum
                        && x.NutritionalValue.Proteins <= maximum).ToList();
                    break;
                case 2:
                    minimum = CheckDecimal("Enter minimum of fats");
                    maximum = CheckDecimal("Enter maximum of fats");
                    recipes = recipes.Where(x => x.NutritionalValue.Fats >= minimum
                        && x.NutritionalValue.Fats <= maximum).ToList();
                    break;
                case 3:
                    minimum = CheckDecimal("Enter minimum of carbohydrates");
                    maximum = CheckDecimal("Enter maximum of carbohydrates");
                    recipes = recipes.Where(x => x.NutritionalValue.Carbohydrates >= minimum
                        && x.NutritionalValue.Carbohydrates <= maximum).ToList();
                    break;
            }
            return recipes;
        }

        static int CheckInteger(string message, int limit)
        {
            PrintInColor(ConsoleColor.Cyan, message);
            int command;
            while (!int.TryParse(Console.ReadLine().Trim(), out command)
                || command < 1 || command > limit)
                PrintInColor(ConsoleColor.Red, "Invalid input. " + message);
            return command;
        }

        static decimal CheckDecimal(string message)
        {
            PrintInColor(ConsoleColor.Cyan, message);
            decimal decimalToCheck;
            while (!decimal.TryParse(Console.ReadLine().Trim().Replace(',', '.'),
                NumberStyles.Any, Nfi, out decimalToCheck) || decimalToCheck <= 0)
                PrintInColor(ConsoleColor.Red, "Invalid input. " + message);
            return Math.Round(decimalToCheck, 2);
        }

        static string InputString(string message)
        {
            PrintInColor(ConsoleColor.Cyan, message);
            string input = Console.ReadLine();
            while (input.Trim().Length < 1)
            {
                PrintInColor(ConsoleColor.Red, "Invalid input. " + message);
                input = Console.ReadLine();
            }
            return input;
        }

        static void PrintInColor(ConsoleColor consoleColor, string message)
        {
            Console.ForegroundColor = consoleColor;
            Console.WriteLine("\n " + message);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
