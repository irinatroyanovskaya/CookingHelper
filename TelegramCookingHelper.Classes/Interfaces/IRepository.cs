using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TelegramCookingHelper.Classes.DatabaseRepository;

namespace TelegramCookingHelper.Classes.Interfaces
{

    public interface IRepository
    {

        IEnumerable<Dish> Dishes{ get; }
        IEnumerable<MainIngredient> MainIngredients { get; }
        IEnumerable<Meal> Meals { get; }

        MainIngredient FindRandomIngredient(string meal);
        List<Dish> ShowPossibleDishes(string ingr);
        Dish ShowRecipe(string rec);

    }
}
