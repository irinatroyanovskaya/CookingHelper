namespace TelegramCookingHelper.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using TelegramCookingHelper.Classes;

    internal sealed class Configuration : DbMigrationsConfiguration<TelegramCookingHelper.Classes.Context>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(TelegramCookingHelper.Classes.Context context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            var readData = new ReadData();

            foreach (var meal in readData.Meals)
            {
                context.Meals.AddOrUpdate(meal);
                context.SaveChanges();
            }

            foreach (var ing in readData.Ingredients)
            {
                context.Ingredients.AddOrUpdate(ing);
                context.SaveChanges();
            }

            foreach (var dish in readData.Dishes)
            {
                dish.MainIngredient = context.Ingredients.First(ing => ing.Name == dish.MainIngredient.Name);
                dish.Meal = context.Meals.First(m => m.Name == dish.Meal.Name);
                context.Dishes.AddOrUpdate(dish);
                context.SaveChanges();
            }
        }
    }
}
