using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

class Program
{
    private static TelegramBotClient botClient;
    private static Dictionary<long, UserData> users = new Dictionary<long, UserData>();
    
    static async Task Main(string[] args)
    {
        botClient = new TelegramBotClient("YOUR_BOT_TOKEN");
        
        var me = await botClient.GetMeAsync();
        Console.WriteLine($"Бот {me.Username} запущен!");
        
        botClient.OnMessage += Bot_OnMessage;
        botClient.OnCallbackQuery += Bot_OnCallbackQuery;
        
        botClient.StartReceiving();
        
        Console.ReadLine();
        botClient.StopReceiving();
    }

    private static async void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
    {
        var message = e.Message;
        if (message == null || message.Type != MessageType.Text)
            return;

        long chatId = message.Chat.Id;
        
        if (!users.ContainsKey(chatId))
        {
            users[chatId] = new UserData();
        }

        switch (message.Text)
        {
            case "/start":
                await SendWelcomeMessage(chatId);
                break;
            case "Начать викторину":
                users[chatId].CurrentQuestion = 0;
                users[chatId].Score = 0;
                await SendQuestion(chatId, 0);
                break;
            case "Мой счет":
                await botClient.SendTextMessageAsync(chatId, $"Ваш текущий счет: {users[chatId].Score} баллов");
                break;
            case "Сбросить прогресс":
                users[chatId].CurrentQuestion = 0;
                users[chatId].Score = 0;
                await botClient.SendTextMessageAsync(chatId, "Ваш прогресс сброшен. Можете начать викторину заново!");
                break;
            default:
                await botClient.SendTextMessageAsync(chatId, "Используйте кнопки для навигации.");
                break;
        }
    }

    private static async void Bot_OnCallbackQuery(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
    {
        var callbackQuery = e.CallbackQuery;
        long chatId = callbackQuery.Message.Chat.Id;
        int currentQuestion = users[chatId].CurrentQuestion;
        var question = QuizQuestions[currentQuestion];

        if (int.TryParse(callbackQuery.Data, out int selectedAnswer))
        {
            bool isCorrect = selectedAnswer == question.CorrectAnswer;
            
            if (isCorrect)
            {
                users[chatId].Score++;
                await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, "Правильно! ✅");
            }
            else
            {
                await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, "Неправильно! ❌");
            }

            // Отправляем объяснение
            await botClient.SendTextMessageAsync(
                chatId,
                $"{(isCorrect ? "✅ Верно!" : "❌ Неверно!")}\n\n{question.Explanation}",
                parseMode: ParseMode.Html);

            // Переходим к следующему вопросу или завершаем викторину
            users[chatId].CurrentQuestion++;
            
            if (users[chatId].CurrentQuestion < QuizQuestions.Count)
            {
                await SendQuestion(chatId, users[chatId].CurrentQuestion);
            }
            else
            {
                await botClient.SendTextMessageAsync(
                    chatId,
                    $"🎉 Викторина завершена!\nВаш результат: {users[chatId].Score} из {QuizQuestions.Count}",
                    replyMarkup: new ReplyKeyboardMarkup(new[]
                    {
                        new KeyboardButton("Начать викторину"),
                        new KeyboardButton("Мой счет")
                    }, resizeKeyboard: true));
            }
        }
    }

    private static async Task SendWelcomeMessage(long chatId)
    {
        var keyboard = new ReplyKeyboardMarkup(new[]
        {
            new[] { new KeyboardButton("Начать викторину") },
            new[] { new KeyboardButton("Мой счет"), new KeyboardButton("Сбросить прогресс") }
        }, resizeKeyboard: true);

        await botClient.SendTextMessageAsync(
            chatId,
            "Добро пожаловать в викторину по истории искусства! 🎨\n\n" +
            "Вы будете отвечать на вопросы о различных периодах и стилях искусства. " +
            "После каждого ответа вы получите объяснение с интересными фактами.\n\n" +
            "Нажмите «Начать викторину», чтобы приступить!",
            replyMarkup: keyboard);
    }

    private static async Task SendQuestion(long chatId, int questionIndex)
    {
        var question = QuizQuestions[questionIndex];
        
        var options = new List<InlineKeyboardButton>();
        for (int i = 0; i < question.Answers.Count; i++)
        {
            options.Add(InlineKeyboardButton.WithCallbackData(question.Answers[i], i.ToString()));
        }

        var inlineKeyboard = new InlineKeyboardMarkup(options);

        await botClient.SendTextMessageAsync(
            chatId,
            $"Вопрос {questionIndex + 1}/{QuizQuestions.Count}:\n\n{question.Text}",
            replyMarkup: inlineKeyboard);
    }

    private static List<Question> QuizQuestions = new List<Question>
    {
        new Question
        {
            Text = "Кто написал картину «Мона Лиза»?",
            Answers = new List<string> { "Пабло Пикассо", "Леонардо да Винчи", "Винсент ван Гог", "Микеланджело" },
            CorrectAnswer = 1,
            Explanation = "<b>Леонардо да Винчи</b> написал «Мону Лизу» между 1503 и 1519 годами. Это один из самых известных портретов в истории искусства. Интересный факт: картина также известна как «Джоконда» — от итальянского «La Gioconda»."
        },
        new Question
        {
            Text = "В каком стиле написана картина «Звёздная ночь» Ван Гога?",
            Answers = new List<string> { "Импрессионизм", "Постимпрессионизм", "Кубизм", "Сюрреализм" },
            CorrectAnswer = 1,
            Explanation = "«Звёздная ночь» — яркий пример <b>постимпрессионизма</b>. Ван Гог написал её в 1889 году, находясь в лечебнице. Картина известна своими выразительными мазками и эмоциональной насыщенностью."
        },
        new Question
        {
            Text = "Кто создал скульптуру «Давид»?",
            Answers = new List<string> { "Донателло", "Микеланджело", "Бернини", "Роден" },
            CorrectAnswer = 1,
            Explanation = "Мраморную статую <b>«Давид»</b> создал <b>Микеланджело</b> между 1501 и 1504 годами. Это шедевр эпохи Возрождения высотой 5,17 метров. Интересно, что Микеланджело было всего 26 лет, когда он начал работать над этой скульптурой."
        },
        new Question
        {
            Text = "Какое художественное движение ассоциируется с Сальвадором Дали?",
            Answers = new List<string> { "Сюрреализм", "Кубизм", "Фовизм", "Дадаизм" },
            CorrectAnswer = 0,
            Explanation = "<b>Сальвадор Дали</b> был одним из ведущих представителей <b>сюрреализма</b>. Его самые известные работы, такие как «Постоянство памяти» с плавящимися часами, стали символами этого направления."
        },
        new Question
        {
            Text = "В каком веке жил Рембрандт?",
            Answers = new List<string> { "XVI век", "XVII век", "XVIII век", "XIX век" },
            CorrectAnswer = 1,
            Explanation = "<b>Рембрандт</b> Харменс ван Рейн (1606–1669) жил в <b>XVII веке</b>. Он считается одним из величайших мастеров «золотого века» голландской живописи. Его известные работы включают «Ночной дозор» и многочисленные автопортреты."
        }
    };
}

class UserData
{
    public int CurrentQuestion { get; set; }
    public int Score { get; set; }
}

class Question
{
    public string Text { get; set; }
    public List<string> Answers { get; set; }
    public int CorrectAnswer { get; set; }
    public string Explanation { get; set; }
}
