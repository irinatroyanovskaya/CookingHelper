using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramCookingHelper.Classes
{
    public class DatabaseRepository
    {
        public Context Context { get; set; }

        public List<Meal> ShowMeals() 
        {
            return Context.Meals.ToList();
        }

        public List<Dish> ShowPossibleDishes(MainIngredient ingr) //возвращает список блюд, которые можно приготовить из данного ингредиента
        {
            return Context.Dishes.Where(d => d.MainIngredient.Id == ingr.Id).ToList();
        }

        public MainIngredient FindRandomIngredient(Meal meal) //находит случайны ингредиент для данного типа еды 
        {
            Random rnd = new Random();
            var numberOfIngredient = rnd.Next(0, Context.Dishes.Where(d=>d.Meal.Id==meal.Id).Select(d=>d.MainIngredient).Count() - 1);
            return Context.Ingredients.ToList()[numberOfIngredient];
        }

        public void SaveDish(Dish dish, User user)
        {
            Context.Users.First(u => u.Id == user.Id).FavouriteDishes.Add(dish);
            Context.SaveChanges();
        }

        public void DeleteDish(Dish dish, User user)
        {
            Context.Users.First(u => u.Id == user.Id).FavouriteDishes.Remove(dish);
            Context.SaveChanges();
        }

        public List<Dish> ShowSavedDishes(User user)
        {
            return Context.Users.First(u => u.Id == user.Id).FavouriteDishes.ToList();
        }

        public void CreateUser(string name)
        {
            if (name == null)
                return;
            if (IsNameUnique(name))
            {
                var user = new User
                {
                    Name = name,
                    FavouriteDishes = new List<Dish>()
                };
                Context.Users.Add(user);
                Context.SaveChanges();
            }
        }

        public bool IsNameUnique(string name) //проверяет, уникально ли имя пользователя
        {                                      
            if (Context.Users.Any(u => u.Name == name))
                return false;
            return true;
        }
    }
}
