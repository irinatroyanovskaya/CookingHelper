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

        public TelegramBotClient BotClient { get; set; } = new TelegramBotClient("555593986:AAGnkMf_Sl_ImOlrrSPgokDg3s19O15hzO0");
        public DatabaseRepository Repo { get; set; } = new DatabaseRepository();

        public BotRepository()
        {
            BotClient.OnMessage += BotClient_OnMessage;
            BotClient.OnCallbackQuery += BotClient_OnCallbackQuery;
            BotClient.StartReceiving();
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
            if (Repo.Context.Dishes.Any(d => d.Name == e.CallbackQuery.Data))
            {
                var dishName = e.CallbackQuery.Data;
                var keyboard = new ReplyKeyboardMarkup(new[]
                {
                    new[]
                    {
                        new KeyboardButton("Сохрани рецепт"),
                        new KeyboardButton("Верни главное меню")
                    }
                });
                BotClient.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, Repo.Context.Dishes.First(d => d.Name == dishName).Recipe, replyMarkup: keyboard);
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
                    var userName = e.Message.From.FirstName + e.Message.From.LastName;
                    if (Repo.Context.Users.Any(u => u.Name == userName))
                    {
                        if (Repo.Context.Users.First(u => u.Name == userName).FavouriteDishes == null || Repo.Context.Users.First(u => u.Name == userName).FavouriteDishes.Count == 0)
                            BotClient.SendTextMessageAsync(message.Chat.Id, "К сожалению, у вас нет сохраненных рецептов");
                        else
                        {
                            foreach (var dish in Repo.Context.Users.First(u => u.Name == userName).FavouriteDishes.ToList())
                                BotClient.SendTextMessageAsync(message.Chat.Id, dish.Name);
                        }
                    }
                    else
                    {
                        BotClient.SendTextMessageAsync(message.Chat.Id, "Вы пишете мне в первый раз, " +
                            "поэтому я зарегистрирую вас в базе, и у вас будет возможность сохранить понравившиеся рецепты");
                        Repo.CreateUser(userName);
                    }
                    break;

                case "Нет, покажи список блюд":
                    List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();
                    foreach (var dish in Repo.ShowPossibleDishes(_ingr))
                    {
                        var button = InlineKeyboardButton.WithCallbackData(dish.Name);
                        buttons.Add(button);
                    }
                    CreateOneRowInlineKeyboard(buttons, "Нажмите на блюдо, чтобы увидеть рецепт", message.Chat.Id);
                    break;

                case "Да, рассчитай заново":
                    GetIngredient(_meal, message.Chat.Id);
                    break;

                case "Верни главное меню":
                    ShowMainMenu(message.Chat.Id);
                    break;
            }
        }
    }
}
