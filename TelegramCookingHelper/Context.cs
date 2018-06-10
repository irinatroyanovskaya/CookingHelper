using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramCookingHelper.Classes
{
    class Context:DbContext
    {
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<MainIngredient> Ingredients { get; set; }
        public DbSet<Meal> Meals { get; set; }
        public DbSet<User> Users { get; set; }

        public Context() : base("localsql")
        { }
    }
}
