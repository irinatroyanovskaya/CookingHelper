using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramCookingHelper.Classes
{
    public class BotRepository
    {
        MainIngredient _ingr;
        Meal _meal;
        Dish _dish;
        User _user;

        public TelegramBotClient BotClient { get; set; } = new TelegramBotClient("555593986:AAGnkMf_Sl_ImOlrrSPgokDg3s19O15hzO0");
        public DatabaseRepository Repo { get; set; } = new DatabaseRepository();

        public BotRepository()
        {
            BotClient.OnMessage += BotClient_OnMessage;
            BotClient.OnCallbackQuery += BotClient_OnCallbackQuery;
            BotClient.StartReceiving();
        }

        public void ShowSavedDishes(string userName, long chatId)
        {
            if (Repo.Context.Users.Any(u => u.Name == userName))
            {
                if (Repo.Context.SavedDishes.Where(sd => sd.User.Name == userName).Count() == 0)
                    BotClient.SendTextMessageAsync(chatId, "К сожалению, у вас нет сохраненных рецептов");
                else
                    CreateOneRowInlineKeyboard(CreateDishButtons(Repo.ShowSavedDishes(Repo.Context.Users.First(u => u.Name == userName))), "Нажмите на блюдо, чтобы увидеть рецепт", chatId);
            }
            else
            {
                BotClient.SendTextMessageAsync(chatId, "Вы пишете мне в первый раз, " +
                    "поэтому я зарегистрирую вас в базе, и у вас будет возможность сохранить понравившиеся рецепты");
                Repo.CreateUser(userName);
            }
        }

        public void ShowMainMenu(long chatId)
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            new KeyboardButton("Рассчитай ингредиент"),
                            new KeyboardButton("Покажи сохраненные рецепты")
                        }
                    });
            BotClient.SendTextMessageAsync(chatId, "Нажимай на кнопочки - будет весело!", replyMarkup: keyboard);
        }

        public void GetIngredient(Meal selectedMeal, long chatId)
        {
            _ingr = Repo.FindRandomIngredient(selectedMeal);
            BotClient.SendTextMessageAsync(chatId, _ingr.Name.ToUpper() + " стоит " + _ingr.Price + " рублей; можно купить здесь: " + _ingr.WhereToBuy);
            BotClient.SendPhotoAsync(chatId, _ingr.ImageReference);
            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                new[]
                {
                    new KeyboardButton("Да, рассчитай заново"),
                    new KeyboardButton("Нет, покажи список блюд")
                }
            });
            BotClient.SendTextMessageAsync(chatId, "Рассчитать еще раз?", replyMarkup: keyboard);
        }

        public List<InlineKeyboardButton> CreateDishButtons(List<Dish> dishes)
        {
            var dishButtons = new List<InlineKeyboardButton>();
            foreach (var dish in dishes)
            {
                var button = InlineKeyboardButton.WithCallbackData(dish.Name);
                dishButtons.Add(button);
            }
            return dishButtons;
        }

        public void CreateOneRowInlineKeyboard(List<InlineKeyboardButton> buttons, string textMessageToSend, long chatId)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(buttons);
            BotClient.SendTextMessageAsync(chatId, textMessageToSend, replyMarkup: inlineKeyboard);
        }

        private void BotClient_OnCallbackQuery(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            if (Repo.Context.Meals.Any(m => m.Name == e.CallbackQuery.Data))
            {
                var selectedMealName = e.CallbackQuery.Data;
                _meal = Repo.Context.Meals.First(m => m.Name == selectedMealName);
                GetIngredient(_meal, e.CallbackQuery.Message.Chat.Id);
            }
            if (Repo.Context.SavedDishes.Any(sd=>sd.Dish.Name==e.CallbackQuery.Data) && _user!=null)
            {
                var dishName = e.CallbackQuery.Data;
                _dish = Repo.Context.Dishes.First(d => d.Name == dishName);
                var keyboard = new ReplyKeyboardMarkup(new[]
                {
                    new[]
                    {
                        new KeyboardButton("Удали рецепт из сохранённых"),
                        new KeyboardButton("Вернись в список рецептов"),
                        new KeyboardButton("Верни главное меню")
                    }
                });
                BotClient.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, _dish.Recipe, replyMarkup: keyboard);
            }
            else if (Repo.Context.Dishes.Any(d => d.Name == e.CallbackQuery.Data))
            {
                var dishName = e.CallbackQuery.Data;
                _dish = Repo.Context.Dishes.First(d => d.Name == dishName);
                var keyboard = new ReplyKeyboardMarkup(new[]
                {
                    new[]
                    {
                        new KeyboardButton("Сохрани рецепт"),
                        new KeyboardButton("Верни главное меню")
                    }
                });
                BotClient.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, _dish.Recipe, replyMarkup: keyboard);
            }
        }

        private void BotClient_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var message = e.Message;
            switch (message.Text)
            {
                case "/start":
                    ShowMainMenu(message.Chat.Id);
                    break;

                case "Рассчитай ингредиент":
                    var inlineKeyboardMeals = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Первое блюдо"),
                            InlineKeyboardButton.WithCallbackData("Второе блюдо"),
                            InlineKeyboardButton.WithCallbackData("Закуска")
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Десерт"),
                            InlineKeyboardButton.WithCallbackData("Напиток"),
                            InlineKeyboardButton.WithCallbackData("Соус")
                        }
                    });
                    BotClient.SendTextMessageAsync(message.Chat.Id, "Выберите тип блюда из списка", replyMarkup: inlineKeyboardMeals);
                    break;

                case "Покажи сохраненные рецепты":
                    ShowSavedDishes(e.Message.From.FirstName + e.Message.From.LastName, message.Chat.Id);
                    _user = Repo.Context.Users.First(u => u.Name == message.From.FirstName + message.From.LastName);
                    break;

                case "Нет, покажи список блюд":
                    _user = null;
                    CreateOneRowInlineKeyboard(CreateDishButtons(Repo.ShowPossibleDishes(_ingr, _meal)), "Нажмите на блюдо, чтобы увидеть рецепт", message.Chat.Id);
                    break;

                case "Сохрани рецепт":
                    if (!Repo.Context.Users.Any(u => u.Name == e.Message.From.FirstName + e.Message.From.LastName))
                        Repo.CreateUser(e.Message.From.FirstName + e.Message.From.LastName);
                    Repo.SaveDish(_dish, Repo.Context.Users.First(u => u.Name == e.Message.From.FirstName + e.Message.From.LastName));
                    BotClient.SendTextMessageAsync(message.Chat.Id, "Рецепт сохранен!");
                    break;

                case "Да, рассчитай заново":
                    GetIngredient(_meal, message.Chat.Id);
                    break;

                case "Верни главное меню":
                    ShowMainMenu(message.Chat.Id);
                    break;

                case "Вернись в список рецептов":
                    ShowSavedDishes(e.Message.From.FirstName + e.Message.From.LastName, message.Chat.Id);
                    break;

                case "Удали рецепт из сохранённых":
                    Repo.DeleteDish(_dish, Repo.Context.Users.First(u => u.Name == e.Message.From.FirstName + e.Message.From.LastName));
                    BotClient.SendTextMessageAsync(message.Chat.Id, "Рецепт удален!");
                    break;
            }
        }
    }
}
