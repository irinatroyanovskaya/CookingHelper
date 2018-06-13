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
        public TelegramBotClient BotClient { get; set; } = new TelegramBotClient("555593986:AAGnkMf_Sl_ImOlrrSPgokDg3s19O15hzO0");

        public BotRepository()
        {
            BotClient.OnMessage += BotClient_OnMessage;
            BotClient.OnCallbackQuery += BotClient_OnCallbackQuery;
            BotClient.StartReceiving();
        }

        private void BotClient_OnCallbackQuery(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            var selectedMeal = e.CallbackQuery.Data;
            var rnd = new Random();
            BotClient.AnswerCallbackQueryAsync(e.CallbackQuery.Id, rnd.Next(0, 10).ToString());
            BotClient.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, rnd.Next(0, 10).ToString());
            BotClient.SendPhotoAsync(e.CallbackQuery.Message.Chat.Id, "http://dom-eda.com/uploads/images/catalog/item/4b548ecd74/a89afde0b7_500.jpg");
            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                new[]
                {
                    new KeyboardButton("Да, рассчитай заново"),
                    new KeyboardButton("Нет, покажи список блюд")
                }
            });
            BotClient.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, "Рассчитать еще раз?", replyMarkup: keyboard);
        }

        private void BotClient_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var message = e.Message;
            switch (message.Text)
            {
                case "/start":
                    var keyboard = new ReplyKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            new KeyboardButton("Рассчитай ингредиент"),
                            new KeyboardButton("Покажи сохраненные рецепты")
                        }
                    });
                    BotClient.SendTextMessageAsync(message.Chat.Id, "бебебе", replyMarkup: keyboard);
                    break;
            }
        }
    }
}
