using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TelegramCookingHelper.Classes
{
    public class ReadData
    {
        public List<Dish> Dishes { get; set; }
        public List<Meal> Meals { get; set; }
        public List<MainIngredient> Ingredients { get; set; }
        const string filename = "Core.txt";

        public ReadData()
        {
            Meals = ReadMeals(new StreamReader(filename));
            Ingredients = ReadIngredients(new StreamReader(filename));
            Dishes = ReadDishes(new StreamReader(filename));
        }

        public List<Meal> ReadMeals(StreamReader sr)
        {
            try
            {
                var line = sr.ReadLine();
                var parts = line.Split(';');
                var meals = new List<Meal>();
                for (int i = 0; i < parts.Length; i++)
                {
                    var meal = new Meal { Name = parts[i] };
                    meals.Add(meal);
                }
                return meals;
            }
            catch
            {
                return new List<Meal>();
            }
        }


        public List<MainIngredient> ReadIngredients(StreamReader sr)
        {
            try
            {
                ReadMeals(sr);
                var ingredients = new List<MainIngredient>();
                int numberOfIngredients = int.Parse(sr.ReadLine());
                for (int i = 0; i < numberOfIngredients; i++)
                {
                    var line = sr.ReadLine();
                    var parts = line.Split(';');
                    var ingredient = new MainIngredient { Name = parts[0], Price = Decimal.Parse(parts[1]), WhereToBuy = parts[2], ImageReference = parts[3] };
                    ingredients.Add(ingredient);
                }
                return ingredients;
            }
            catch
            {
                return new List<MainIngredient>();
            }
        }

        public List<Dish> ReadDishes(StreamReader sr)
        {
            try
            {
                ReadIngredients(sr);
                var dishes = new List<Dish>();
                int numberOfDishes = int.Parse(sr.ReadLine());
                for (int i = 0; i < numberOfDishes; i++)
                {
                    var line = sr.ReadLine();
                    var parts = line.Split(';');
                    var dish = new Dish { Name = parts[0], Recipe = parts[3] };
                    foreach (var meal in ReadMeals(new StreamReader(filename)))
                    {
                        if (meal.Name == parts[1])
                            dish.Meal = meal;
                    }
                    foreach (var ingredient in ReadIngredients(new StreamReader(filename)))
                    {
                        if (ingredient.Name == parts[2])
                            dish.MainIngredient = ingredient;
                    }
                    dishes.Add(dish);
                }
                return dishes;
            }
            catch
            {
                return new List<Dish>();
            }
        }
    }
}
