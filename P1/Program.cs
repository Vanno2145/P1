using System;
using Microsoft.Win32;

class Program
{
    const string registryPath = @"";

    static void Main()
    {
        Console.WriteLine("==== Настройки приложения ====");

        // Чтение настроек из реестра
        string username = ReadRegistryValue("Username", "Гость");
        string themeColor = ReadRegistryValue("ThemeColor", "Синий");

        Console.WriteLine($"Текущее имя пользователя: {username}");
        Console.WriteLine($"Текущий цвет темы: {themeColor}");

        Console.WriteLine("\nХотите изменить настройки? (y/n)");
        string input = Console.ReadLine()?.ToLower();

        if (input == "y")
        {
            Console.Write("Введите новое имя пользователя: ");
            username = Console.ReadLine();

            Console.Write("Введите цвет темы (например, Синий, Зелёный, Красный): ");
            themeColor = Console.ReadLine();

            WriteRegistryValue("Username", username);
            WriteRegistryValue("ThemeColor", themeColor);

            Console.WriteLine("✅ Настройки сохранены.");
        }

        Console.WriteLine("\nНажмите любую клавишу для выхода...");
        Console.ReadKey();
    }

    static string ReadRegistryValue(string key, string defaultValue)
    {
        using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey(registryPath))
        {
            if (regKey != null)
            {
                object value = regKey.GetValue(key);
                if (value != null)
                    return value.ToString();
            }
        }
        return defaultValue;
    }

    static void WriteRegistryValue(string key, string value)
    {
        using (RegistryKey regKey = Registry.CurrentUser.CreateSubKey(registryPath))
        {
            regKey.SetValue(key, value);
        }
    }
}
