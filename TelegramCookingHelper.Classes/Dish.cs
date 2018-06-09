using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramCookingHelper.Classes
{
    class Dish
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Meal Meal { get; set; }
        public MainIngredient MainIngredient { get; set; }
    }
}
