using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramCookingHelper.Classes
{
    public class DatabaseRepository
    {
        public Context Context { get; set; } = new Context();

        public List<Meal> ShowMeals()
        {
            try
            {
                return Context.Meals.ToList();
            }
            catch
            {
                return new List<Meal>();
            }
        }

        public List<Dish> ShowPossibleDishes(MainIngredient ingr, Meal meal) //возвращает список блюд, которые можно приготовить из данного ингредиента
        {
            try
            {
                return Context.Dishes.Where(d => d.MainIngredient.Id == ingr.Id && d.Meal.Id == meal.Id).ToList();
            }
            catch
            {
                return new List<Dish>();
            }
        }

        public MainIngredient FindRandomIngredient(Meal meal) //находит случайны ингредиент для данного типа еды 
        {
            try
            {
                Random rnd = new Random();
                var numberOfIngredient = rnd.Next(0, Context.Dishes.Where(d => d.Meal.Id == meal.Id).Select(d => d.MainIngredient).Count());
                return Context.Dishes.Where(d => d.Meal.Id == meal.Id).Select(d => d.MainIngredient).ToList()[numberOfIngredient];
            }
            catch
            {
                return new MainIngredient();
            }
        }

        public void SaveDish(Dish dish, User user)
        {
            try
            {
                if (Context.SavedDishes.Any(sd => sd.User.Id == user.Id && sd.Dish.Id == dish.Id))
                    return;
                Context.SavedDishes.Add(new SavedDish { Dish = dish, User = user });
                Context.SaveChanges();
            }
            catch
            {
                
            }
        }

        public void DeleteDish(Dish dish, User user)
        {
            try
            {
                if (!Context.SavedDishes.Any(sd => sd.Dish.Id == dish.Id))
                    return;
                Context.SavedDishes.Remove(Context.SavedDishes.First(sd => sd.Dish.Id == dish.Id && sd.User.Id == user.Id));
                Context.SaveChanges();
            }
            catch
            { }
        }

        public List<Dish> ShowSavedDishes(User user)
        {
            try
            {
                return Context.SavedDishes.Where(sd => sd.User.Id == user.Id).Select(sd => sd.Dish).ToList();
            }
            catch
            {
                return new List<Dish>();
            }
        }

        public void CreateUser(string name)
        {
            try
            {
                if (name == null)
                    return;
                if (IsNameUnique(name))
                {
                    var user = new User
                    {
                        Name = name,
                    };
                    Context.Users.Add(user);
                    Context.SaveChanges();
                }
            }
            catch
            {
               
            }
        }

        public bool IsNameUnique(string name) //проверяет, уникально ли имя пользователя
        {
            try
            {
                if (Context.Users.Any(u => u.Name == name))
                    return false;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
