using System;
using System.Net.Http;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Ваши API-ключи для WeatherAPI и OpenWeatherMap
        string weatherApiKey = "";
        string openWeatherApiKey = "";
        
        // URL для запросов
        string weatherApiUrl = $"https://api.weatherapi.com/v1/current.json?key={weatherApiKey}&q=London";
        string openWeatherApiUrl = $"https://api.openweathermap.org/data/2.5/weather?q=London&appid={openWeatherApiKey}&units=metric";

        // Создаём экземпляр HttpClient
        var httpClient = new HttpClient();

        // Выполняем запросы параллельно
        var task1 = httpClient.GetStringAsync(weatherApiUrl);
        var task2 = httpClient.GetStringAsync(openWeatherApiUrl);

        // Ожидаем выполнения обеих задач
        await Task.WhenAll(task1, task2);

        // Получаем результаты
        string weatherData1 = task1.Result;
        string weatherData2 = task2.Result;

        // Выводим результаты
        Console.WriteLine("WeatherAPI Response:");
        Console.WriteLine(weatherData1);
        Console.WriteLine("OpenWeatherMap Response:");
        Console.WriteLine(weatherData2);
    }
}
